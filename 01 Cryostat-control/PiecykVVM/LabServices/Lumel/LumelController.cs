using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using LabServices.DataTemplates;
using Serilog;

namespace LabServices.Lumel
{
    /// <summary>
    /// Klasa kontrolera procesowego sterownika temperatury Lumel Re72
    /// Implementacja komend oraz funkcji RegisterCommands w LumelControllerCommands.cs
    /// </summary>
    public sealed partial class LumelController : HardwareController
    {
        /// <summary>Zmienna przechowywująca kolejkę komend</summary>
        private ConcurrentQueue<ControllerCommandData> _controllerCommands = new ConcurrentQueue<ControllerCommandData>();
        /// <summary>Instancja interfejsu komunikacji z mostkiem pomiarowym MFIA</summary>
        private LumelInterface _lumelInterface = new LumelInterface();
        /// <summary>Okres wykonania jednego obiegu pętli kontrolera</summary>
        private long _enginePeriod = 200;
        /// <summary>Skumulowane opóźnienie pętli głównej w ms</summary>
        private long _engineLag = 0;
        /// <summary>Co ile okreasów pętli kontrolera odczytać wskazanie temperatury</summary>
        private long _engineReadMultipler = 5;
        /// <summary>Zmienna odliczająca kiedy wykonać odczyt temperatury(okres co _enginePeriod * _engineReadMultipler)</summary>
        private long _engineReadCounter = 0;
        /// <summary>Zamontowany obecnie sensor temperatury</summary>
        private TemperatureSensor _temperatureSensor = TemperatureSensor.Undefined;
        /// <summary>Blokada czasowa dla wywoływania komend</summary>
        private TimeLockFlag _timeLock = new TimeLockFlag();
        /// <summary>Pula wartości automatycznych pid. Null gdy auto pid nieaktywny</summary>
        private LumelAutoPidPool? _autoPidPool = null;

        protected override void Init(object param)
        {
            Log.Information("LumelController-Init Thread");
            // Inicjalizacja kontrolera
            LumelControllerInitData initData = (LumelControllerInitData)param;
            _lumelInterface.Connect(
                port: initData.Com,
                baudrate: (int)initData.Budrate,
                addres: initData.Addres,
                dataBits: initData.DataBits,
                parity: initData.Parity,
                stopBits: initData.StopBits
                );

            // Odczytanie wartości stanu początkowego
            ushort[] pid1Parameters = _lumelInterface.ReadMultipleRegisters((ushort)LumelRe72Registers.Pid1ParameterP, 3);
            ushort? sensor = _lumelInterface.ReadSingleRegister((ushort)LumelRe72Registers.MainSensorType);
            ushort? currentTemperature = _lumelInterface.ReadSingleRegister((ushort)LumelRe72Registers.CurrentTemperature);
            ushort? targetTemperature = _lumelInterface.ReadSingleRegister((ushort)LumelRe72Registers.TargetTemperature);

            // Zapisywanie wartości odczytów
            _temperatureSensor = (TemperatureSensor)sensor!.Value;
            LumelStore.SetLumelPidValue(new LumelPidValue
            {
                ParamP = pid1Parameters[0],
                ParamI = pid1Parameters[1],
                ParamD = pid1Parameters[2]
            });
            LumelStore.SetCurrentTemperature(
                LumelDataConverter.ConvertTemperatureUshortToDouble(currentTemperature!.Value, _temperatureSensor)
                );
            LumelStore.SetTargetTemperature(
                LumelDataConverter.ConvertTemperatureUshortToDouble(targetTemperature!.Value, _temperatureSensor)
                );
        }

        protected override void LoopInteration()
        {
            // Tworzenie licznika odmierzającego czas
            Stopwatch loopWatch = Stopwatch.StartNew();

            // Obsługa odczytu wartości temperatury
            _engineReadCounter++;
            if(_engineReadCounter >= _engineReadMultipler)
            {
                _engineReadCounter = 0;
                ushort? ushortCurrentTemperature = _lumelInterface.ReadSingleRegister((ushort)LumelRe72Registers.CurrentTemperature);
                double doubleCurrentTemperature = LumelDataConverter.ConvertTemperatureUshortToDouble(
                    ushortCurrentTemperature!.Value,
                    _temperatureSensor
                    );
                LumelStore.SetCurrentTemperature(doubleCurrentTemperature);
            }

            // Obsługa auto pid
            if (_autoPidPool != null)
            {
                LumelPidValue newPidValue = _autoPidPool.GetValue(LumelStore.GetTargetTemperature());
                LumelPidValue oldPidValue = LumelStore.GetCurrentLumelPidValue();
                if (!newPidValue.Equals(oldPidValue))
                {
                    ushort[] ushortNewPidValue = {
                            newPidValue.ParamP,
                            newPidValue.ParamI,
                            newPidValue.ParamD
                        };
                    _lumelInterface.WriteMultipleRegisters((ushort)LumelRe72Registers.Pid1ParameterP, ushortNewPidValue);
                    LumelStore.SetLumelPidValue(newPidValue);
                }
            }

            // Obsługa komend
            if (!_timeLock.IsLocked())
                while (_controllerCommands.Count > 0)
                {
                    // Przerwanie gdy czas wykonania pętli przekracza oczekiwany okres(zwiększa precyzję punktów temperatury)
                    if (loopWatch.ElapsedMilliseconds > _enginePeriod)
                        break;
                    // Odczyt i wywołanie komendy
                    ControllerCommandData commandData;
                    bool dequeueFlag = _controllerCommands.TryDequeue(out commandData);
                    if (dequeueFlag)
                    {
                        Log.Information($"LumelController-Executed:{commandData.CommandNumber}");
                        _commandPool.ExecuteCommand(commandData.CommandNumber, commandData.ParamList);
                    }
                }

            // Wykrycie zgubienia przebiegu pętli
            if (_engineLag > _enginePeriod)
            {
                long lostPeriods = _engineLag / _enginePeriod;
                long totalLostTime = _engineLag - (_engineLag % _enginePeriod);
                _engineLag = _engineLag % _enginePeriod;
                Log.Error($"LumelController-LoopInteration LostPeriods:{lostPeriods},TotalLostTime:{totalLostTime}ms");
            }

            // Obsługa przerwania(wykonywane w celu zapewnienia jak największej dokładności okresu wykonania pętli)
            long loopExecutionTime = loopWatch.ElapsedMilliseconds;
            if (loopExecutionTime + _engineLag < _enginePeriod)
            {
                _engineLag = 0;
                Thread.Sleep((int)(_enginePeriod - (loopExecutionTime + _engineLag)));
                return;
            }
            else
            {
                _engineLag = loopExecutionTime + _engineLag - _enginePeriod;
                Thread.Sleep((int)_enginePeriod);
                return;
            }
        }

        protected override void Finish()
        {
            Log.Information("LumelController-Finish Thread");
            // Ustawianie bezpiecznej temperatury
            ushort safeTargetTemperature = LumelDataConverter.ConvertTemperatureDoubleToUshort(
                15,
                _temperatureSensor
                );
            _lumelInterface.WriteSingleRegister(
                (ushort)LumelRe72Registers.TargetTemperature,
                safeTargetTemperature
                );
            // Zamykanie połączenia
            _lumelInterface.ClosePort();
        }

        // Statyczne funkcje dostępu do obiektu
        // ----------------------------------------------------------------------------------------------------

        /// <summary>Zmienna przechowywująca jedyną instancję klasy</summary>
        private static LumelController _lumelController = new LumelController();

        /// <summary>
        /// Funkcja uruchamia wątek kontrolera jeżeli nie jest już aktywny
        /// </summary>
        public static void StartController(object param)
        {
            if (param is not LumelControllerInitData)
            {
                Log.Error("Bad parameter in LumleController.StartController, not LumelControllerInitData type");
                throw new ArgumentException("Not LumelControllerInitData type");
            }
            _lumelController.InternalStartTask(param);
        }

        /// <summary>
        /// Funkcja zleca wyłączenie wątku kontrolera jeżeli ten jest aktywny
        /// </summary>
        public static void StopController()
        {
            _lumelController.InternalKillTask();
        }

        /// <summary>
        /// Zwraca informację czy wątek został włączony
        /// </summary>
        /// <returns></returns>
        public static bool IsActive()
        {
            return _lumelController.InternalIsActive();
        }

        /// <summary>
        /// Wysyła komendę do wykonania przez kontroler
        /// </summary>
        /// <param name="command">ID komendy</param>
        /// <param name="param">Lista parametrów</param>
        public static void PushCommand(LumelControllerCommands command, List<object>? param = null)
        {
            if (param == null)
                param = new List<object>();
            Log.Verbose("Register command {command}");
            _lumelController._controllerCommands.Enqueue(new ControllerCommandData((ushort)command, param));
        }
    }
}
