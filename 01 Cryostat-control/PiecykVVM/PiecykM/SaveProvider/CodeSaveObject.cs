using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiecykM.SaveProvider
{
    /// <summary>
    /// Obiekt kontenerowy dla zapisu ustawień kodu oraz programu użytkownika
    /// </summary>
    public class CodeSaveObject
    {
        /// <summary>Zmienna na potrzeby oznaczenia wersji podczas serializacji do pliku</summary>
        public int SaveVersion { get; set; } = 1;
        public bool IsAutoPidActive { get; set; }
        public string ProgramCode { get; set; }

        public string ConnectionType { get; set; }
        public string MeasurementPrecision { get; set; }
        public string MeanFromSamples { get; set; }
        public string FrequencySegmentation { get; set; }
        public string SampleCount { get; set; }
        public string FrequencyStart { get; set; }
        public string FrequencyStop { get; set; }
        public string VoltageAmplitudeControlType { get; set; }
        public string VoltageAmplitude { get; set; }
        public string BiasVontageState { get; set; }
        public string BiasVoltage { get; set; }
        public string ControlerPeriod { get; set; }
        public string ControlerReadMultipler { get; set; }

        public CodeSaveObject(string programCode, string connectionType, string measurementPrecision, string meanFromSamples, string frequencySegmentation, string sampleCount, string frequencyStart, string frequencyStop, string voltageAmplitudeControlType, string voltageAmplitude, string biasVontageState, string biasVoltage, string controlerPeriod, string controlerReadMultipler, bool isAutoPidActive)
        {
            ProgramCode = programCode;
            ConnectionType = connectionType;
            MeasurementPrecision = measurementPrecision;
            MeanFromSamples = meanFromSamples;
            FrequencySegmentation = frequencySegmentation;
            SampleCount = sampleCount;
            FrequencyStart = frequencyStart;
            FrequencyStop = frequencyStop;
            VoltageAmplitudeControlType = voltageAmplitudeControlType;
            VoltageAmplitude = voltageAmplitude;
            BiasVontageState = biasVontageState;
            BiasVoltage = biasVoltage;
            ControlerPeriod = controlerPeriod;
            ControlerReadMultipler = controlerReadMultipler;
            IsAutoPidActive = isAutoPidActive;
        }
    }
}
