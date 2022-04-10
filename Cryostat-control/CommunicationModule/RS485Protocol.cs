using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using Modbus.Device;

namespace Piecyk.CommunicationModule
{
    /// <summary>
    /// Klasa służąca do komunikacji przez protokół RS485.
    /// </summary>
    public class RS485Protocol
    {
        // Domyślne ustawienia protokołu komunikacji
        public static byte DefaultDeviceAddress = 1;
        public static int DefaultBaudrate = 9600;
        public static Parity CommParity = Parity.None;
        public static StopBits CommStopBits = StopBits.Two;
        public static int CommDataBits = 8;

        // Zmienne stanu portu komunikacji
        public string currentPort { get; private set; }
        public int currentBaudrate { get; private set; }
        public byte currentAddress { get; private set; }
        public bool isPortOpen { get; private set; }

        private SerialPort serialPort;
        private IModbusMaster modbusMaster;
        private static RS485Protocol masterObject = null;

        /// <summary>
        /// Domyślny konstruktor obiektu komunikacji.
        /// </summary>
        private RS485Protocol()
        {
            currentPort = "";
            serialPort = null;
            modbusMaster = null;
            isPortOpen = false;
        }

        ~RS485Protocol()
        {
            // Zamykanie połączenia jeżeli jest aktywne
            if (isPortOpen)
                ClosePort();
        }

        /// <summary>
        /// Domyślna funkcja do pozyskiwania dostępu do obiektu
        /// </summary>
        /// <returns>Obiekt RS485Communication</returns>
        public static RS485Protocol GetRS485Protocol()
        {
            if (masterObject == null)
            {
                masterObject = new RS485Protocol();
            }

            return masterObject;
        }

        /// <summary>
        /// Funkcja służąca do inicjowania połączenia
        /// </summary>
        /// <param name="com_">Port COM urządzenia np. "COM3"</param>
        /// <param name="baudrate_">Baudrate połączenia</param>
        /// <param name="address_">Adres urządzenia z którym następuje połączenie</param>
        public void connectToPort(string com_, int baudrate_, byte address_)
        {
            // Sprawdzenie czy nie ma już aktywnego połączenia i jeżeli tak to zamknięcie go
            if (serialPort != null)
                ClosePort();
            // Nawiązanie połączenia
            serialPort = new SerialPort(com_);
            serialPort.BaudRate = baudrate_;
            serialPort.DataBits = CommDataBits;
            serialPort.Parity = CommParity;
            serialPort.StopBits = CommStopBits;
            serialPort.Open();
            modbusMaster = ModbusSerialMaster.CreateRtu(serialPort);
            isPortOpen = true;
            // Zmiana obecnych wartości połączenia
            currentPort = com_;
            currentBaudrate = baudrate_;
            currentAddress = address_;
        }

        /// <summary>
        /// Funkcja przerywająca połączenie z urządzeniem
        /// </summary>
        public void ClosePort()
        {
            // Łapanie wyjątku na wypadek gdyby nastąpiło zerwanie połączenia, a program prubował zamknąć port
            try
            {
                serialPort.Close();
            } catch {; }
            isPortOpen = false;
        }

        /// <summary>
        /// Funkcja pisząca do rejestru
        /// </summary>
        /// <param name="registerAddress">Numer rejestru</param>
        /// <param name="value">Wartość wpisywana</param>
        /// <returns>Czy udało się wykonać zapis</returns>
        public bool WriteToRegister(ushort registerAddress_, ushort value_)
        {
            if (!isPortOpen) return false;
            try
            {
                modbusMaster.WriteSingleRegister(currentAddress, registerAddress_, value_);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Funkcja odczytująca wartość rejestru
        /// </summary>
        /// <param name="registerAddress">Numer rejestru</param>
        /// <returns>Wartość rejestru lub 65535 w przypadku błędu</returns>
        public ushort ReadRegister(ushort registerAddress_)
        {
            if (!isPortOpen) return 65535;
            try
            {
                ushort[] data = modbusMaster.ReadHoldingRegisters(currentAddress, registerAddress_, 1);
                return data[0];
            }
            catch
            {
                return 65535;
            }
        }
    }
}
