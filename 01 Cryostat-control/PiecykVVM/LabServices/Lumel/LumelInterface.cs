using System;
using System.IO.Ports;
using NModbus;
using NModbus.Serial;
using LabServices.Exceptions;
using Serilog;

namespace LabServices.Lumel
{
    /// <summary>
    /// Klasa udostępnia interfejs komunikacji z sterownikiem Lumel przy pomocy protokołu modbus RTU
    /// Klasa zakłada użycia połączenia USB->COM->MODBUS RTU
    /// </summary>
    public class LumelInterface
    {
        // Zmienne klasy
        private SerialPort? _serialPort;
        private IModbusMaster? _modbusMaster;
        private byte _lumelAddres = 1; // Wartość jest nadpisywana w Connect
        private bool _isActive = false;
        public bool IsActive { get { return _isActive; } }

        /// <summary>
        /// Funkcja nawiązuje połączenie z sterownikiem Lumel przez złącze USB->COM->MODBUS RTU
        /// </summary>
        /// <param name="port">Nazwa portu np. "COM1"</param>
        /// <param name="baudrate">Baudrate połączenia</param>
        /// <param name="addres">Adres sterownika Lumel w magistrali MODBUS</param>
        /// <param name="dataBits"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        public void Connect(string port, int baudrate, byte addres, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.Two)
        {
            Log.Information($"LumelInterface-Connecting {port},{baudrate},{addres},{dataBits},{parity},{stopBits}");
            if (_serialPort != null)
                return;

            // Ustawianie zmiennych globalnych
            _lumelAddres = addres;
            _isActive = true;

            // Konfiguracja połączenia z portem COM
            _serialPort = new SerialPort(port);
            _serialPort.BaudRate = baudrate;
            _serialPort.DataBits = dataBits;
            _serialPort.Parity = parity;
            _serialPort.StopBits = stopBits;
            _serialPort.Open();
            Log.Debug("LumelInterface-ConnectedToSerial");

            // Nawiązywanie połączenia z magistralą modbus RTU
            ModbusFactory factory = new ModbusFactory();
            _modbusMaster = factory.CreateRtuMaster(_serialPort);
            Log.Debug("LumelInterface-ConnectedToModbus");
        }

        /// <summary>
        /// Funkcja rozwiązuje połączenie z urządzeniem
        /// </summary>
        public void ClosePort()
        {
            _isActive = false;
            if( _modbusMaster != null )
                _modbusMaster.Dispose();
            if( _serialPort != null )
                _serialPort.Close();
        }

        /// <summary>
        /// Zapisuje wartości do długości max 123 kolejnych rejestrów
        /// </summary>
        /// <param name="startAddress">Adres pierwszego rejestru</param>
        /// <param name="values">Zapiswane wartości</param>
        /// <exception cref="ToMuchDataException"></exception>
        public void WriteMultipleRegisters(ushort startAddress, ushort[] values)
        {
            Log.Debug($"LumelInterface-WriteMultipleRegisters:{startAddress},{values.Length}");
            if (values.Length > 123)
                throw new ToMuchDataException($"LumelInterface.WriteMultipleRegisters passed data length: {values.Length}, but max is 123");
            if (_modbusMaster != null)
                _modbusMaster.WriteMultipleRegisters(_lumelAddres, startAddress, values);
        }

        /// <summary>
        /// Zapisuje wartość do rejestru
        /// </summary>
        /// <param name="registerAddress">Adres rejestru</param>
        /// <param name="value">Wartość</param>
        public void WriteSingleRegister(ushort registerAddress, ushort value)
        {
            if (_modbusMaster != null)
                _modbusMaster.WriteSingleRegister(_lumelAddres, registerAddress, value);
        }

        /// <summary>
        /// Funkcja odczytuje wartość kolejnych rejestrów
        /// </summary>
        /// <param name="startAddress">Adres pierwszego rejestru</param>
        /// <param name="numberOfRegisters">Liczba rejestrów</param>
        /// <returns>Wartości rejestrów lub pusta tablica w przypadku braku inicjalizacji modbus</returns>
        public ushort[] ReadMultipleRegisters(ushort startAddress, ushort numberOfRegisters)
        {
            Log.Debug($"LumelInterface-ReadMultiple:{startAddress},{numberOfRegisters}");
            if (_modbusMaster != null)
                return _modbusMaster.ReadHoldingRegisters(_lumelAddres, startAddress, numberOfRegisters);
            return new ushort[0];
        }

        /// <summary>
        /// Odczytuje wartość 1 rejestru
        /// </summary>
        /// <param name="registerAddress">Adres rejestru</param>
        /// <returns>Wartość rejestru lub null w przypadku braku inicjalizacji modbus</returns>
        public ushort? ReadSingleRegister(ushort registerAddress)
        {
            if (_modbusMaster != null)
                return _modbusMaster.ReadHoldingRegisters(_lumelAddres, registerAddress, 1)[0];
            return null;
        }
    }
}
