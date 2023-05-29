namespace LabServices.MFIA
{
    /// <summary>
    /// Dokładność pomiaru
    /// daq.setInt("/dev3709/system/impedance/precision", MeasurementPrecision);
    /// </summary>
    public enum MeasurementPrecision : long
    {
        Low = 0,
        High = 1,
        VeryHigh = 2
    }

    /// <summary>
    /// Segmentacja częstotliwości pomiaru
    /// sweeper.setInt("xmapping", FrequencySegmentation);
    /// </summary>
    public enum FrequencySegmentation : long
    {
        Linear = 0,
        Logarytmic = 1
    }

    /// <summary>
    /// Metoda podłączenia próbki 2/4 kable
    /// daq.setInt("/dev3709/imps/0/mode", ConnectionType);
    /// </summary>
    public enum ConnectionType : long
    {
        FourWireTerminal = 0,
        TwoWireTerminal = 1
    }

    /// <summary>
    /// Jaki typ kontroli zakresu wartości napięcia i prądu
    /// daq.setInt("/dev3709/imps/0/auto/inputrange", VoltageRangeControlType);
    /// daq.setDouble("/dev3709/imps/0/current/range", 0.0011);
    /// daq.setDouble("/dev3709/imps/0/voltage/range", 0.301);
    /// </summary>
    public enum VoltageRangeControlType : long
    {
        Auto = 1,
        Manual = 0
    }

    /// <summary>
    /// Jaki typ kontroli amplitudy napięcia
    /// daq.setInt("/dev3709/imps/0/auto/output", VoltageAmplitudeControlType);
    /// daq.setDouble("/dev3709/imps/0/output/amplitude", 0.3);
    /// </summary>
    public enum VoltageAmplitudeControlType : long
    {
        Manual = 0,
        Auto = 1
    }

    /// <summary>
    /// Czy element stały napięcia ma być włączony
    /// daq.setInt("/dev3709/imps/0/bias/enable", BiasVontageState);
    /// daq.setDouble("/dev3709/imps/0/bias/value", 0.0001); - może być ujemny
    /// </summary>
    public enum BiasVontageState : long
    {
        OFF = 0,
        ON = 1
    }
}
