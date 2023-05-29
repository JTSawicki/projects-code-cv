using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabServices.Lumel
{
    /// <summary>
    /// Klasa zawiera konwertery pomiędzy zmiennymi Lumel, a wartościami domyślnymi
    /// </summary>
    public static class LumelDataConverter
    {
        /// <summary>
        /// Funkcja konwertuje wartość temperatury z double(°C) na ushort(kodowanie LUMEL)
        /// </summary>
        /// <param name="value">Konwertowana wartość temperatury w kodowaniu LUMEL</param>
        /// <param name="sensor">Zamontowany w sterowniku sensor temperatury</param>
        /// <returns>Wartość przekonwertowana lub graniczna przy przekroczeniu</returns>
        public static ushort ConvertTemperatureDoubleToUshort(double value, TemperatureSensor sensor)
        {
            // Pobieranie ograniczeń temperatury
            Tuple<int, int> Limits = GetTemperatureLimits(sensor);

            // Kontrola ograniczeń
            if (value < Limits.Item1) value = Limits.Item1;
            else if (value > Limits.Item2) value = Limits.Item2;

            // Konwersja
            ushort outValue = (ushort)(Math.Abs(value) * 10);
            if (value < 0) outValue = (ushort)(65536 - outValue);
            return outValue;
        }

        /// <summary>
        /// Funkcja konwertuje wartość temperatury z ushort(kodowanie LUMEL) na double(°C)
        /// </summary>
        /// <param name="value">Konwertowana wartość temperatury w kodowaniu LUMEL</param>
        /// <param name="sensor">Zamontowany w sterowniku sensor temperatury</param>
        /// <returns>Wartość przekonwertowana</returns>
        public static double ConvertTemperatureUshortToDouble(ushort value, TemperatureSensor sensor)
        {
            // Pobieranie ograniczeń temperatury
            Tuple<int, int> Limits = GetTemperatureLimits(sensor);

            // Kontrola ograniczenia górnego i jeżeli przekroczone zmiana na liczbę ujemną według kodowania LUMEL
            double outValue;
            if (value > (ushort)Limits.Item2)
            {
                value = (ushort)(65536 - value);
                outValue = (-1) * value;
            }
            else
            {
                outValue = value;
            }
            return outValue / 10;
        }

        /// <summary>
        /// Funkcja zwraca limity temperatury w °C dla danego typu sensora temperatury
        /// Czujniki prądowe i napięciowe obecnie nie wspierane
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns>Tuple(DolneOgraniczenie, GórneOgraniczenie)</returns>
        /// <exception cref="ArgumentOutOfRangeException">Dla niewspieranych typów sensora temperatury</exception>
        public static Tuple<int, int> GetTemperatureLimits(TemperatureSensor sensor) => sensor switch
        {
            TemperatureSensor.PT100 => new Tuple<int, int>(-200, 850),
            TemperatureSensor.PT1000 => new Tuple<int, int>(-200, 850),
            TemperatureSensor.Fe_CuNi => new Tuple<int, int>(-100, 1200),
            TemperatureSensor.Cu_CuNi => new Tuple<int, int>(-100, 400),
            TemperatureSensor.NiCr_NiAl => new Tuple<int, int>(-100, 1372),
            TemperatureSensor.PtRh10_Pt => new Tuple<int, int>(0, 1767),
            TemperatureSensor.PtRh13_Pt => new Tuple<int, int>(0, 1767),
            TemperatureSensor.PtRh30_PtRh6 => new Tuple<int, int>(0, 1767),
            TemperatureSensor.NiCr_CuNi => new Tuple<int, int>(-100, 1000),
            TemperatureSensor.NiCrSi_NiSi => new Tuple<int, int>(-1000, 1300),
            TemperatureSensor.Chromel_Kopel => new Tuple<int, int>(-100, 800),
            _ => throw new ArgumentOutOfRangeException(nameof(sensor), $"Not expected direction value: {sensor}")
        };
    }
}
