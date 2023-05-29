using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using LabServices.DataTemplates;
using System.Globalization;
using Serilog;

namespace LabServices.MFIA
{
    public static class MFIAStore
    {
        /// <summary>Lista zawierająca wszystkie pomiary </summary>
        private static List<MFIAMeasurement> _measurements = new List<MFIAMeasurement>();
        private static object _lockMeasurements = new object();
        /// <summary>
        /// Publiczny event wywoływany przy dodaniu nowego pomiaru oraz zerowaniu buforów
        /// Event może być wywoływany przez inne wątki - zabezpieczenie po stronie odbiorcy
        /// </summary>
        public static event EventHandler? NewMeasuermentEvent;

        /// <summary>Obecnie używane parametry inicjalizacji pomiaru</summary>
        private static LockedProperty<MFIASweeperInitData?> _currentMFIASweeperInitData = new LockedProperty<MFIASweeperInitData?>(null);
        /// <summary>
        /// Publiczny event wywoływany przy zmianie parametrów wykonywania pomiarów
        /// Event może być wywoływany przez inne wątki - zabezpieczenie po stronie odbiorcy
        /// </summary>
        public static event EventHandler? NewMeasurementParametersEvent;

        /// <summary>
        /// Funkcja służy do dodwania nowego pomiaru przez interfejs pomiarowy
        /// </summary>
        /// <param name="measurement">Wartość pomiaru</param>
        internal static void PushNewMeasurement(MFIAMeasurement measurement)
        {
            lock (_lockMeasurements)
            {
                _measurements.Add(measurement);
            }
            NewMeasuermentEvent?.Invoke(new object(), EventArgs.Empty);
        }

        /// <summary>
        /// Funkcja służy do zmiany informacji o parametrach wykonywania pomiarów
        /// </summary>
        /// <param name="initData">Obecnie nastawione parametry</param>
        internal static void SetMeasurementParameters(MFIASweeperInitData param)
        {
            _currentMFIASweeperInitData.Set(param);
            NewMeasurementParametersEvent?.Invoke(new object(), EventArgs.Empty);
        }

        /// <summary>
        /// Funkcja służy do uzyskiwania nieprzetworzonych n ostatnich pomiarów
        /// </summary>
        /// <param name="count">Ilość pozyskiwanych pomiarów</param>
        /// <returns>Lista z pomiarami. Jeżeli pomiarów było mniej niż n ma ona tylko liczbę możliwych do uzyskania pomiarów.</returns>
        public static List<MFIAMeasurement> TryGetLastMeasurements(int count)
        {
            List<MFIAMeasurement> lastMeasurements = new List<MFIAMeasurement>();
            if (count == 1)
            {
                lock (_lockMeasurements)
                {
                    if (_measurements.Count > 0)
                        lastMeasurements.Add(_measurements.Last());
                }
            }
            if (count > 1)
            {
                lock (_lockMeasurements)
                {
                    for (int i = _measurements.Count - 1; i > _measurements.Count - 1 - count; i--)
                    {
                        if (i < 0)
                            break;
                        lastMeasurements.Add(_measurements[i]);
                    }
                }
            }
            return lastMeasurements;
        }

        /// <summary>
        /// Zwraca ilość wykonanych pomiarów
        /// </summary>
        /// <returns></returns>
        public static int GetMeasurementsCount()
        {
            lock (_lockMeasurements)
            {
                return _measurements.Count;
            }
        }

        /// <summary>
        /// Funkcja służy do pozyskiwania wszyskich pomiarów
        /// </summary>
        /// <returns></returns>
        public static List<MFIAMeasurement> GetAllMeasurements()
        {
            lock (_lockMeasurements)
            {
                return new List<MFIAMeasurement>(_measurements);
            }
        }

        /// <summary>
        /// Funkcja służy do uzyskiwania informacji o parametrach wykonywania pomiarów
        /// </summary>
        /// <returns></returns>
        public static MFIASweeperInitData? GetCurrentParameters()
        {
            return _currentMFIASweeperInitData.Get();
        }

        /// <summary>
        /// Funkcja czyści listę przechowywanych pomiarów
        /// </summary>
        public static void ClearMeasurementPool()
        {
            lock (_lockMeasurements)
            {
                _measurements = new List<MFIAMeasurement>();
            }
            NewMeasuermentEvent?.Invoke(new object(), EventArgs.Empty);
        }

        /// <summary>
        /// Funkcja służy do generowania zawartości pliku zapisu dla danych pomiarowych
        /// </summary>
        /// <returns>Zawartość pliku CSV</returns>
        public static string GenerateCSV()
        {
            return GenerateSaveContent(',');
        }

        /// <summary>
        /// Funkcja służy do generowania zawartości pliku zapisu dla danych pomiarowych
        /// </summary>
        /// <returns>Zawartość pliku TSV</returns>
        public static string GenerateTSV()
        {
            return GenerateSaveContent('\t');
        }

        /// <summary>
        /// Funkcja generująca zawartość pliku zapisu
        /// </summary>
        /// <param name="delimiter">Znak odzielający od siebie wartości</param>
        /// <returns>Zawartość pliku zapisu</returns>
        private static string GenerateSaveContent(char delimiter)
        {
            // Tworzenie funkcji i ciągów formatujących
            Func<double, string> doubleFormater =
                number =>
                {
                    // Miejsca znaczące - nie używane ponieważ generowało często np. 12E-8
                    //return number.ToString("G6", CultureInfo.InvariantCulture);
                    // Wszystkie miejsca całkowite i 8 dziesiętnych
                    return number.ToString("0.00000000", CultureInfo.InvariantCulture);
                };
            string dateFormat = "yyyy-MM-dd";
            string timeFormat = "HH:mm:ss.fff";

            // Pobieranie danych
            List<MFIAMeasurement> measurements = MFIAStore.GetAllMeasurements();
            if (measurements.Count == 0)
                return "";

            // Tworzenie zmiennej wyjściowej
            StringBuilder result = new StringBuilder();

            // Tworzenie informacji nagłówkowej
            result.Append("Data");
            for (int i = 0; i < measurements.Count; i++)
                result.Append(delimiter + measurements[i].TimeStamp.ToString(dateFormat));
            result.Append(Environment.NewLine);

            result.Append("Czas");
            for (int i = 0; i < measurements.Count; i++)
                result.Append(delimiter + measurements[i].TimeStamp.ToString(timeFormat));
            result.Append(Environment.NewLine);

            result.Append("Temperatura");
            for (int i = 0; i < measurements.Count; i++)
                result.Append(delimiter + doubleFormater(measurements[i].Temperature));
            result.Append(Environment.NewLine);

            // Tworzenie informacji o wartościach pomiaru
            result.AppendLine("Częstotliwość" + delimiter + "ABS");
            for (int j = 0; j < measurements[0].Length; j++)
            {
                // Częstotliwość prubki
                result.Append(doubleFormater(measurements[0].Freq[j]));
                for (int i = 0; i < measurements.Count; i++)
                {
                    result.Append(delimiter + doubleFormater(measurements[i].ABS[j]));
                }
                result.Append(Environment.NewLine);
            }
            result.AppendLine("Częstotliwość" + delimiter + "Im");
            for (int j = 0; j < measurements[0].Length; j++)
            {
                // Częstotliwość prubki
                result.Append(doubleFormater(measurements[0].Freq[j]));
                for (int i = 0; i < measurements.Count; i++)
                {
                    result.Append(delimiter + doubleFormater(measurements[i].Im[j]));
                }
                result.Append(Environment.NewLine);
            }
            result.AppendLine("Częstotliwość" + delimiter + "Re");
            for (int j = 0; j < measurements[0].Length; j++)
            {
                // Częstotliwość prubki
                result.Append(doubleFormater(measurements[0].Freq[j]));
                for (int i = 0; i < measurements.Count; i++)
                {
                    result.Append(delimiter + doubleFormater(measurements[i].Re[j]));
                }
                result.Append(Environment.NewLine);
            }
            result.AppendLine("Częstotliwość" + delimiter + "Phase");
            for (int j = 0; j < measurements[0].Length; j++)
            {
                // Częstotliwość prubki
                result.Append(doubleFormater(measurements[0].Freq[j]));
                for (int i = 0; i < measurements.Count; i++)
                {
                    result.Append(delimiter + doubleFormater(measurements[i].Phase[j]));
                }
                result.Append(Environment.NewLine);
            }
            result.AppendLine("Częstotliwość" + delimiter + "TanDelta");
            for (int j = 0; j < measurements[0].Length; j++)
            {
                // Częstotliwość prubki
                result.Append(doubleFormater(measurements[0].Freq[j]));
                for (int i = 0; i < measurements.Count; i++)
                {
                    result.Append(delimiter + doubleFormater(measurements[i].TanDelta[j]));
                }
                result.Append(Environment.NewLine);
            }
            result.Remove(result.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            // Zwracanie wyniku
            return result.ToString();
        }

        /// <summary>
        /// Funkcja wczytuje dane pomiarowe z pliku TSV.
        /// Jeżeli wczytanie było udane obecne bufory zostają wyczyszczone i zamienione na wczytane wartości.
        /// </summary>
        /// <param name="fileContent">Zawartość pliku TSV</param>
        /// <returns>Czy udało się wczytać dane</returns>
        public static bool LoadFromTSV(string fileContent)
        {
            return LoadFromSaveFile(fileContent, '\t');
        }

        /// <summary>
        /// Funkcja wczytuje dane pomiarowe z pliku CSV.
        /// Jeżeli wczytanie było udane obecne bufory zostają wyczyszczone i zamienione na wczytane wartości.
        /// </summary>
        /// <param name="fileContent">Zawartość pliku CSV</param>
        /// <returns>Czy udało się wczytać dane</returns>
        public static bool LoadFromCSV(string fileContent)
        {
            return LoadFromSaveFile(fileContent, ',');
        }

        private static bool LoadFromSaveFile(string fileContent, char delimiter)
        {
            // Parametry parsera
            int headerLineCount = 8; // Liczba lini informacyjnych: Czas, Temperatura, Nagłówki
            string dateFormat = "yyyy-MM-dd";
            string timeFormat = "HH:mm:ss.fff";

            fileContent = fileContent.Trim();
            if (string.IsNullOrEmpty(fileContent))
            {
                Log.Error("MFIAStore.LoadFromSaveFile-Invalid empty data string");
                return false;
            }

            // Podział danych
            string[] lines = fileContent.Split('\n');
            if (lines.Length <= headerLineCount)
            {
                Log.Error("MFIAStore.LoadFromSaveFile-To little data for parser");
                return false;
            }
            List<string[]> fields = new List<string[]>(lines.Length);
            for (int i = 0; i < lines.Length; i++)
                fields.Add(lines[i].Trim().Split(delimiter));

            // Tworzenie wczytywanej listy pomiarów
            int measurementCount = fields[0].Length - 1; // Ilość pomiarów
            List<MFIAMeasurement> result = new List<MFIAMeasurement>(measurementCount);

            // Wczytywanie wartości globalnych
            int measurementLength = (fields.Count - headerLineCount) / 5; // Ilość punktów w 1 pomiarze
            double[] freq = new double[measurementLength];
            for (int j = 0; j < measurementLength; j++)
            {
                try
                {
                    freq[j] = double.Parse(fields[4 + j][0], CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"MFIAStore.LoadFromSaveFile-Invalid frequency string on position: [{4 + j + 1}, 1]");
                    return false;
                }
            }

            // Wczytywanie wartość per measurement
            for (int i = 0; i < measurementCount; i++)
            {
                DateTime timeStamp;
                try
                {
                    timeStamp = DateTime.ParseExact(
                    fields[0][i + 1] + " " + fields[1][i + 1],
                    dateFormat + " " + timeFormat,
                    CultureInfo.InvariantCulture
                    );
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"MFIAStore.LoadFromSaveFile-Invalid date for measurement: {i + 1}");
                    return false;
                }

                double temperature;
                try
                {
                    temperature = double.Parse(fields[2][i + 1], CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"MFIAStore.LoadFromSaveFile-Invalid temperature string on position: [3, {i + 2}]");
                    return false;
                }

                double[] abs = new double[measurementLength];
                double[] im = new double[measurementLength];
                double[] re = new double[measurementLength];
                double[] phase = new double[measurementLength];
                double[] tanDelta = new double[measurementLength];
                for (int j = 0; j < measurementLength; j++)
                {
                    int parsedParameter = 0;
                    try
                    {
                        abs[j] = double.Parse(fields[4 + parsedParameter + (measurementLength * parsedParameter) + j][i + 1], CultureInfo.InvariantCulture);
                        parsedParameter++;
                        im[j] = double.Parse(fields[4 + parsedParameter + (measurementLength * parsedParameter) + j][i + 1], CultureInfo.InvariantCulture);
                        parsedParameter++;
                        re[j] = double.Parse(fields[4 + parsedParameter + (measurementLength * parsedParameter) + j][i + 1], CultureInfo.InvariantCulture);
                        parsedParameter++;
                        phase[j] = double.Parse(fields[4 + parsedParameter + (measurementLength * parsedParameter) + j][i + 1], CultureInfo.InvariantCulture);
                        parsedParameter++;
                        tanDelta[j] = double.Parse(fields[4 + parsedParameter + (measurementLength * parsedParameter) + j][i + 1], CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"MFIAStore.LoadFromSaveFile-Invalid parameter string on position: [{4 + parsedParameter + (measurementLength * parsedParameter) + j + 1}, {i + 2}]");
                        return false;
                    }
                }

                result.Add(new MFIAMeasurement
                {
                    Freq = freq,
                    ABS = abs,
                    Im = im,
                    Re = re,
                    Phase = phase,
                    TanDelta = tanDelta,
                    TimeStamp = timeStamp,
                    Length = measurementLength,
                    Temperature = temperature
                });
            }

            // Zapisywanie wyniku parsowania
            ClearMeasurementPool();
            lock (_lockMeasurements)
            {
                _measurements = result;
            }
            NewMeasuermentEvent?.Invoke(new object(), EventArgs.Empty);
            return true;
        }
    }
}
