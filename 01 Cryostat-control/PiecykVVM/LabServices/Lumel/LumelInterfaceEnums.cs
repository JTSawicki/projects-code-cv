namespace LabServices.Lumel
{
    /// <summary>Adresy używanych rejestrów  sterownika Lumel Re72</summary>
    public enum LumelRe72Registers : ushort
    {
        CurrentTemperature = 4006,
        TargetTemperature = 4084,
        Pid1ParameterP = 4043,
        Pid1ParameterI = 4044,
        Pid1ParameterD = 4045,
        MainSensorType = 4015
    }

    /// <summary>Baudrate magistrali modbus</summary>
    public enum Baudrate : int
    {
        Baudrate_4800 = 4800,
        Baudrate_9600 = 9600,
        Baudrate_19200 = 19200,
        Baudrate_38400 = 38400,
        Baudrate_57600 = 57600
    }

    /// <summary>Jednostka informacji używana na magistrali modbus</summary>
    public enum InformationUnit : ushort
    {
        RTU_8N2 = 1,
        RTU_8E1 = 2,
        RTU_8O1 = 3,
        RTU_8N1 = 4
    }

    /// <summary>Typ sensora temperatury podpiętego do sterownika Lumel RE72</summary>
    public enum TemperatureSensor : ushort
    {
        /// <summary>Wartość oznacza że jeszcze nie odczytano lub nie ustawiono wartości</summary>
        Undefined = 9999,
        PT100 = 0,
        PT1000 = 1,
        Fe_CuNi = 2,
        Cu_CuNi = 3,
        NiCr_NiAl = 4,
        PtRh10_Pt = 5,
        PtRh13_Pt = 6,
        PtRh30_PtRh6 = 7,
        NiCr_CuNi = 8,
        NiCrSi_NiSi = 9,
        Chromel_Kopel = 10,
        // Current_0_20 = 11,
        // Current_4_20 = 12,
        // Voltage_0_5 = 13,
        // Voltage_0_10 = 14,
    }
}
