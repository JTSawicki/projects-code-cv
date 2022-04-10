using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piecyk.CommunicationModule
{
    /// <summary>
    /// Klasa której obiekty służą do inicjowania procesu silnika komunikacji.
    /// </summary>
    class CommunicationEngineInitData
    {
        public String COMPort { get; private set; } //< Nazwa portu COM urządzenia
        public int Baudrate { get; private set; } //< Baudrate połączenia
        public byte DeviceAddress { get; private set; } //< Adres urządzenia

        public int EngineFrequency { get; private set; } //< Częstotliwość wykonywania działań przez silnik. Czas podawany w ms. 1s = 1000ms
        public int EngineReadMultipler { get; private set; } //< Co ile cyklów silnika wykonać odczyt. Częstotliwość odczytu = EngineFrequency * EngineReadMultipler

        public CommunicationEngineInitData(String COMPort_, int Baudrate_, byte DeviceAddress_, int EngineFrequency_, int EngineReadMultipler_)
        {
            COMPort = COMPort_;
            Baudrate = Baudrate_;
            DeviceAddress = DeviceAddress_;
            EngineFrequency = EngineFrequency_;
            EngineReadMultipler = EngineReadMultipler_;
        }
    }
}
