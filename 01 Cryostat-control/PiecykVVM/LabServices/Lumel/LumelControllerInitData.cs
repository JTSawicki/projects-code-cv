using System.IO.Ports;

namespace LabServices.Lumel
{
    /// <summary>
    /// Obiekt służący do inicjalizacji kontrolera Lumel
    /// </summary>
    public readonly struct LumelControllerInitData
    {
        /// <summary>Nazwa używanego portu COM</summary>
        public string Com { get; init; }
        /// <summary>Baudrate podłączenia sterownika Lumel</summary>
        public Baudrate Budrate { get; init; }
        /// <summary>Adres sterownika Lumel na magistrali Modbus</summary>
        public byte Addres { get; init; }
        /// <summary>Ilość bitów danych</summary>
        public int DataBits { get; init; }
        /// <summary>Ilość bitów parzystości</summary>
        public Parity Parity { get; init; }
        /// <summary>Ilość bitów stopu</summary>
        public StopBits StopBits { get; init; }

        public LumelControllerInitData(string com, Baudrate budrate, byte addres, int dataBits, Parity parity, StopBits stopBits)
        {
            Com = com;
            Budrate = budrate;
            Addres = addres;
            DataBits = dataBits;
            Parity = parity;
            StopBits = stopBits;
        }
    }
}
