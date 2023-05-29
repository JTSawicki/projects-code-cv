using System.Threading;

namespace LabServices.DataTemplates
{
    /// <summary>
    /// Klasa realizująca podstawowe funkcjonalności kontrolera sprzętu
    /// </summary>
    public abstract class HardwareController
    {
        private LockedProperty<bool> _isActive;
        private LockedProperty<bool> _killSwitch;
        protected ControllerCommandPool _commandPool { get; private set; }

        protected HardwareController()
        {
            _isActive = new LockedProperty<bool>(false);
            _killSwitch = new LockedProperty<bool>(false);
            _commandPool = new ControllerCommandPool();
        }
        /// <summary>
        /// Funkcja inicjująca listę komend
        /// </summary>
        protected abstract void RegisterCommands();
        /// <summary>
        /// Funkcja inicjująca działanie kontrolera i uruchamiająca się raz na początku
        /// </summary>
        /// <param name="param">Obiekt inicjalizujący wątek</param>
        protected abstract void Init(object param);
        /// <summary>
        /// Jedna interacja nieskończonej pętli wątku
        /// </summary>
        protected abstract void LoopInteration();
        /// <summary>
        /// Funkcja kończąca działanie kontrolera i uruchamiana raz przy zakończeniu
        /// </summary>
        protected abstract void Finish();

        /// <summary>
        /// Pętla "MaterLoop" głównego wątku
        /// </summary>
        /// <param name="param">Obiekt inicjalizujący wątek</param>
        private void ThreadMain(object? param)
        {
            _isActive.Set(true);
            RegisterCommands();
            Init(param!);
            bool loopFlag = true;
            while(loopFlag)
            {
                LoopInteration();
                if(_killSwitch.Get())
                {
                    Finish();
                    loopFlag = false;
                }
            }
            _isActive.Set(false);
            _killSwitch.Set(false);
        }

        /// <summary>
        /// Funkcja uruchamia wątek kontrolera jeżeli ten nie jest jeszcze aktywny
        /// </summary>
        /// <param name="param">Parametry uruchomienia wątku</param>
        /// <param name="priority">Priorytet uruchomienia wątku</param>
        protected void InternalStartTask(object param, ThreadPriority priority = ThreadPriority.Normal)
        {
            if(!_isActive.Get())
            {
                Thread controllerThread = new Thread(ThreadMain);
                controllerThread.Priority = priority;
                controllerThread.Start(param);
            }
        }

        /// <summary>
        /// Funkcja zleca wyłączenie wątku kontrolera jeżeli ten jest aktywny
        /// </summary>
        protected void InternalKillTask()
        {
            if(_isActive.Get())
                _killSwitch.Set(true);
        }

        /// <summary>
        /// Zwraca informację czy wątek został włączony
        /// </summary>
        /// <returns></returns>
        protected bool InternalIsActive()
        {
            return _isActive.Get();
        }
    }
}
