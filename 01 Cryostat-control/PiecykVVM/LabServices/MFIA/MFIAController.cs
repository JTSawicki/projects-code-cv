using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using LabServices.DataTemplates;
using Serilog;

namespace LabServices.MFIA
{
    /// <summary>
    /// Klasa kontrolera procesowego mostka pomiarowego MFIA
    /// Implementacja komend oraz funkcji RegisterCommands w MFIAControllerCommands.cs
    /// </summary>
    public sealed partial class MFIAController : HardwareController
    {

        /// <summary>Zmienna przechowywująca kolejkę komend</summary>
        private ConcurrentQueue<ControllerCommandData> _controllerCommands = new ConcurrentQueue<ControllerCommandData>();
        /// <summary>Instancja interfejsu komunikacji z mostkiem pomiarowym MFIA</summary>
        private MFIAInterface _mfiaInterface = new MFIAInterface();

        private MFIAController() : base() { }

        protected override void Init(object param)
        {
            Log.Information("MFIAController-Init Thread");
            _mfiaInterface.ConnectAndInit();
        }

        protected override void LoopInteration()
        {
            Thread.Sleep(50);
            while(_controllerCommands.Count > 0)
            {
                ControllerCommandData commandData;
                bool dequeueFlag = _controllerCommands.TryDequeue(out commandData);
                if(dequeueFlag)
                {
                    Log.Information($"MFIAController-Executed:{commandData.CommandNumber}");
                    _commandPool.ExecuteCommand(commandData.CommandNumber, commandData.ParamList);
                }
            }
        }

        protected override void Finish()
        {
            Log.Information("MFIAController-Finish Thread");
        }

        // Statyczne funkcje dostępu do obiektu
        // ----------------------------------------------------------------------------------------------------

        /// <summary>Zmienna przechowywująca jedyną instancję klasy</summary>
        private static MFIAController _mfiaController = new MFIAController();

        /// <summary>
        /// Funkcja uruchamia wątek kontrolera jeżeli nie jest już aktywny
        /// </summary>
        public static void StartController()
        {
            _mfiaController.InternalStartTask(new object());
        }

        /// <summary>
        /// Funkcja zleca wyłączenie wątku kontrolera jeżeli ten jest aktywny
        /// </summary>
        public static void StopController()
        {
            _mfiaController.InternalKillTask();
        }

        /// <summary>
        /// Zwraca informację czy wątek został włączony
        /// </summary>
        /// <returns></returns>
        public static bool IsActive()
        {
            return _mfiaController.InternalIsActive();
        }

        /// <summary>
        /// Wysyła komendę do wykonania przez kontroler
        /// </summary>
        /// <param name="command">ID komendy</param>
        /// <param name="param">Lista parametrów</param>
        public static void PushCommand(MFIAControllerCommands command, List<object>? param = null)
        {
            if(param == null)
                param = new List<object>();
            _mfiaController._controllerCommands.Enqueue(new ControllerCommandData((ushort)command, param));
        }
    }
}
