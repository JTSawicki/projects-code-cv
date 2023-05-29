using System;
using System.Threading;
using LabServices.Exceptions;
using zhinst;
using Serilog;


namespace LabServices.MFIA
{
    public class MFIAInterface
    {
        // Wewnętrzne zmienne modółów sterownika MFIA
        private ziDotNET? _daq;
        private ziModule? _impedance;
        private ziModule? _sweeper;

        /// <summary>
        /// Funkcja łączy się z MFIA/LabOne
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FailToConnectException"></exception>
        private ziDotNET ConnectToMFIA()
        {
            Log.Debug("MFIAInterface - Conecting to MFIA ...");
            try
            {
                ziDotNET daq = new ziDotNET();
                daq.init("192.168.57.246", Convert.ToUInt16(8004), (ZIAPIVersion_enum)6);
                return daq;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "In MFIAInterface.ConnectToMFIA");
                throw new FailToConnectException("Fail to connext to MFIA", ex);
            }
        }

        /// <summary>
        /// Funkcja łączy się z modułem impedancji
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotConnectedException"></exception>
        /// <exception cref="FailToConnectException"></exception>
        private ziModule GetImpedanceModule()
        {
            Log.Debug("MFIAInterface - Conecting to MFIA impedance module ...");
            // Sprawdzenie czy wykonano inicjalizację
            if (_daq == null)
                throw new NotConnectedException("MFIAInterface not initialized");

            try
            {
                ziModule impedance = _daq.impedanceModule();
                impedance.setString("device", "dev3709");
                impedance.setInt("mode", 7);
                impedance.setString("path", "C:\\Users\\jasio\\AppData\\Roaming\\Zurich Instruments\\LabOne\\WebServer\\setting");
                impedance.execute();
                impedance.setInt("validation", 1);
                impedance.setString("filename", "last_compensation");
                return impedance;
            }
            catch(Exception ex)
            {
                Log.Error(ex, "In MFIAInterface.GetImpedanceModule");
                throw new FailToConnectException("Fail to get impedance from MFIA", ex);
            }
        }

        /// <summary>
        /// Funkcja tworzy sweeper
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotConnectedException"></exception>
        /// <exception cref="FailToConnectException"></exception>
        private ziModule GetSweeper()
        {
            Log.Debug("MFIAInterface - Conecting to MFIA sweeper ...");
            // Sprawdzenie czy wykonano inicjalizację
            if (_daq == null)
                throw new NotConnectedException("MFIAInterface not initialized");
            try
            {
                ziModule sweeper = _daq.sweeper();
                sweeper.setString("device", "dev3709");
                sweeper.setString("gridnode", "/dev3709/oscs/0/freq");
                sweeper.setInt("historylength", 100);
                sweeper.setInt("bandwidthoverlap", 1);
                sweeper.setInt("order", 8);
                // sweeper.setString("save/directory", "C:\\Users\\jasio\\Documents\\Zurich Instruments\\LabOne\\WebServer");
                return sweeper;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "In MFIAInterface.GetSweeper");
                throw new FailToConnectException("Fail to get sweeper from MFIA", ex);
            }
        }

        /// <summary>
        /// Funkcja nawiązuje połączenie z MFIA i wstępnie inicjalizuje modóły mostka pomiarowego
        /// </summary>
        /// <exception cref="NotConnectedException"></exception>
        /// <exception cref="FailToConnectException"></exception>
        public void ConnectAndInit()
        {
            Log.Information("MFIAInterface - connecting");
            _daq = ConnectToMFIA();
            _impedance = GetImpedanceModule();
            _sweeper = GetSweeper();

            // Zmienna do wstępnej inicjalizacji wykonywanej w celu uniknięcia potencjalnego pomiaru w stanie nieustalonym
            MFIASweeperInitData initLowPrecision = new MFIASweeperInitData
            {
                ConnectionType = ConnectionType.FourWireTerminal,
                MeasurementPrecision = MeasurementPrecision.Low,
                MeanFromSamples = 5,
                FrequencySegmentation = FrequencySegmentation.Linear,
                SampleCount = 2,
                FrequencyStart = 1000,
                FrequencyStop = 2000,
                VoltageAmplitudeControlType = VoltageAmplitudeControlType.Auto,
                VoltageAmplitude = 0.00001,
                BiasVontageState = BiasVontageState.OFF,
                BiasVoltage = 0
            };
            SetSweeperParameters(initLowPrecision);
        }

        /// <summary>
        /// Funkcja ustawia parametry wywołania pomiaru
        /// </summary>
        /// <param name="param"></param>
        /// <exception cref="NotConnectedException"></exception>
        public void SetSweeperParameters(MFIASweeperInitData param)
        {
            Log.Information("MFIAInterface - seting sweeper parameters");
            // Sprawdzenie czy wykonano inicjalizację
            if (_daq == null || _sweeper == null)
                throw new NotConnectedException("MFIAInterface not initialized");
            // Nastawianie parametrów
            _daq.setInt("/dev3709/imps/0/mode", (long)param.ConnectionType);
            SetMeasurementPrecision(param.MeasurementPrecision);
            _sweeper.setInt("averaging/sample", param.MeanFromSamples);
            _sweeper.setInt("xmapping", (long)param.FrequencySegmentation);
            _sweeper.setInt("samplecount", param.SampleCount);
            _sweeper.setDouble("start", param.FrequencyStart);
            _sweeper.setDouble("stop", param.FrequencyStop);
            _daq.setInt("/dev3709/imps/0/auto/output", (long)param.VoltageAmplitudeControlType);
            if(param.VoltageAmplitudeControlType == VoltageAmplitudeControlType.Manual)
                _daq.setDouble("/dev3709/imps/0/output/amplitude", param.VoltageAmplitude);
            _daq.setInt("/dev3709/imps/0/bias/enable", (long)param.BiasVontageState);
            if(param.BiasVontageState == BiasVontageState.ON)
                _daq.setDouble("/dev3709/imps/0/bias/value", param.BiasVoltage);
            MFIAStore.SetMeasurementParameters(param);
        }

        /// <summary>
        /// Funkcja automatyzuje nastawienie parametru dokładności pomiaru
        /// </summary>
        /// <param name="measurementPrecision"></param>
        /// <exception cref="NotConnectedException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private void SetMeasurementPrecision(MeasurementPrecision measurementPrecision)
        {
            // Sprawdzenie czy wykonano inicjalizację
            if (_daq == null || _sweeper == null)
                throw new NotConnectedException("MFIAInterface not initialized");
            // Nastawianie precyzji
            switch (measurementPrecision)
            {
                case MeasurementPrecision.Low:
                    _daq.setInt("/dev3709/system/impedance/precision", 0);
                    _sweeper.setDouble("settling/inaccuracy", 0.01);
                    _sweeper.setDouble("averaging/tc", 5);
                    _sweeper.setDouble("averaging/time", 0.01);
                    _sweeper.setDouble("bandwidth", 100);
                    _sweeper.setDouble("maxbandwidth", 1000);
                    _sweeper.setDouble("omegasuppression", 60);
                    break;
                case MeasurementPrecision.High:
                    _daq.setInt("/dev3709/system/impedance/precision", 1);
                    _sweeper.setDouble("settling/inaccuracy", 0.01);
                    _sweeper.setDouble("averaging/tc", 15);
                    _sweeper.setDouble("averaging/time", 0.1);
                    _sweeper.setDouble("bandwidth", 10);
                    _sweeper.setDouble("maxbandwidth", 100);
                    _sweeper.setDouble("omegasuppression", 80);
                    break;
                case MeasurementPrecision.VeryHigh:
                    _daq.setInt("/dev3709/system/impedance/precision", 2);
                    _sweeper.setDouble("settling/inaccuracy", 0.0001);
                    _sweeper.setDouble("averaging/tc", 25);
                    _sweeper.setDouble("averaging/time", 1);
                    _sweeper.setDouble("bandwidth", 1);
                    _sweeper.setDouble("maxbandwidth", 10);
                    _sweeper.setDouble("omegasuppression", 120);
                    break;
                default:
                    throw new NotImplementedException($"Missing implementation of SetMeasurementPrecision for {(long)measurementPrecision}");
            }
        }

        /// <summary>
        /// Funkcja służy do wywołania akcji pomiaru
        /// </summary>
        /// <exception cref="NotConnectedException"></exception>
        public void MakeMeasurement()
        {
            Log.Information("MFIAInterface - make measuremet");
            // Sprawdzenie czy wykonano inicjalizację
            if (_sweeper == null)
                throw new NotConnectedException("MFIAInterface not initialized");

            // Wywoływanie połączenia z modułem impedancji
            String path = String.Format("/{0}/imps/0/sample", "dev3709");
            _sweeper.subscribe(path);// Sybskrybowanie strumienia danych
            _sweeper.execute();

            // Oczekiwanie na zakończenie pomiarów
            while (!_sweeper.finished())
            {
                Thread.Sleep(50);
                // double progress = sweeper.progress() * 100;
                // SweepingPrecentage.Set(progress);
            }
            // Pozyskiwanie czasu zakończenia pomiaru
            DateTime measurementEndTime = DateTime.Now;
            // Pozyskiwanie danych z sweepera
            Lookup lookup = _sweeper.read();
            double[] freq = lookup[path][0].sweeperImpedanceWaves[0].frequency;
            double[] absz = lookup[path][0].sweeperImpedanceWaves[0].absz;
            double[] imagz = lookup[path][0].sweeperImpedanceWaves[0].imagz;
            double[] realz = lookup[path][0].sweeperImpedanceWaves[0].realz;
            double[] phasez = lookup[path][0].sweeperImpedanceWaves[0].phasez;
            // Obliczanie tangensa delty
            double[] tanDelta = new double[realz.Length];
            for(int i = 0; i < realz.Length; i++)
                tanDelta[i] = realz[i] / imagz[i];

            // Pozyskiwanie temperatury
            double measurementTemperature = LabServices.Lumel.LumelStore.GetCurrentTemperature();

            // Zwracanie pomiaru
            MFIAMeasurement measurement = new MFIAMeasurement
            {
                Freq = freq,
                ABS = absz,
                Im = imagz,
                Re = realz,
                Phase = phasez,
                TanDelta = tanDelta,
                TimeStamp = measurementEndTime,
                Length = freq.Length,
                Temperature = measurementTemperature
            };
            MFIAStore.PushNewMeasurement(measurement);
        }
    }
}
