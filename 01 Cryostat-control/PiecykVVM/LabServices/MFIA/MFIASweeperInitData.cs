namespace LabServices.MFIA
{
    public readonly struct MFIASweeperInitData
    {
        /// <summary>Typ podłączenia do próbki</summary>
        public ConnectionType ConnectionType { get; init; }
        /// <summary>Dokładność pomiaru</summary>
        public MeasurementPrecision MeasurementPrecision { get; init; }
        /// <summary>Z ilu pomiarów obliczać wartość średnią dla próbki</summary>
        public long MeanFromSamples { get; init; }
        /// <summary>Jak dzielić częstotliwość</summary>
        public FrequencySegmentation FrequencySegmentation { get; init; }
        /// <summary>Ile próbek w serii</summary>
        public long SampleCount { get; init; }
        /// <summary>Częstotliwość dla pierwszej próbki</summary>
        public double FrequencyStart { get; init; }
        /// <summary>Częstotliwość dla ostatniej próbki</summary>
        public double FrequencyStop { get; init; }
        /// <summary>Typ kontroli amplitudy napięcia</summary>
        public VoltageAmplitudeControlType VoltageAmplitudeControlType { get; init; }
        /// <summary>Amplituda napięcia przy sterowaniu ręcznym</summary>
        public double VoltageAmplitude { get; init; }
        /// <summary>Typ offsetu DC napięcia</summary>
        public BiasVontageState BiasVontageState { get; init; }
        /// <summary>Wartość offsetu DC napięcia</summary>
        public double BiasVoltage { get; init; }

        public override string ToString() =>
            $"MFIASweeperInitData{{ConnectionType:{ConnectionType},MeasurementPrecision:{MeasurementPrecision},MeanFromSamples:{MeanFromSamples},FrequencySegmentation:{FrequencySegmentation},SampleCount:{SampleCount},FrequencyStart:{FrequencyStart},FrequencyStop:{FrequencyStop},VoltageAmplitudeControlType:{VoltageAmplitudeControlType},VoltageAmplitude:{VoltageAmplitude},BiasVontageState:{BiasVontageState},BiasVontageState:{BiasVontageState}}}";

        public MFIASweeperInitData(ConnectionType connectionType, MeasurementPrecision measurementPrecision, long meanFromSamples, FrequencySegmentation frequencySegmentation, long sampleCount, double frequencyStart, double frequencyStop, VoltageAmplitudeControlType voltageAmplitudeControlType, double voltageAmplitude, BiasVontageState biasVontageState, double biasVoltage)
        {
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
        }
    }
}
