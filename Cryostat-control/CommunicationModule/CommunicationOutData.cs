using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piecyk.CommunicationModule
{
    class CommunicationOutData
    {
        // Parametry PID
        public ushort[] PIDParameters { get; private set; } //< Obecne nastawy PID.
        public ushort SetTemperature { get; private set; } //< Obecny nastaw Temperatury.
        public ushort Temperature { get; private set; } //< Obecny pomiar temperatury
        public DateTime Timestamp { get; private set; } //< Znacznik czasu

        /// <summary>
        /// Domyślny konstruktor.
        /// </summary>
        /// <param name="PIDParameters_"></param>
        /// <param name="SetTemperature_"></param>
        /// <param name="Temperature_"></param>
        /// <param name="Timestamp_"></param>
        public CommunicationOutData(ushort[] PIDParameters_, ushort SetTemperature_, ushort Temperature_, DateTime Timestamp_)
        {
            PIDParameters = (ushort[]) PIDParameters_.Clone();
            SetTemperature = SetTemperature_;
            Temperature = Temperature_;
            Timestamp = Timestamp_;
        }

        /// <summary>
        /// Konstruktor na potrzeby inicjowania zmiennej. Nie należy go używać do przesyłania danych bo to nie ma sęsu.
        /// </summary>
        public CommunicationOutData()
        {
            PIDParameters = new ushort[] { 0, 0, 0 };
            SetTemperature = 0;
            Temperature = 0;
            Timestamp = DateTime.Now;
        }
    }
}
