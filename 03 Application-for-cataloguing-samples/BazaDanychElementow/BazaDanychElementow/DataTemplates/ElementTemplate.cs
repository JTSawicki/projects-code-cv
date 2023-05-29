using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BazaDanychElementow.GlobalFunctions;

namespace BazaDanychElementow.DataTemplates
{
    /// <summary>
    /// Szablon kontenera obiektu.
    /// </summary>
    public class ElementTemplate
    {
        public string ClassTemplate = null; //< Klasa nadrzędna
        public string MainParametrValue;
        public int Count; //< Ilość elementów
        public Dictionary<string, string> Parameters; //< Parametry obiektu(nazwa parametru : wartość)
        /*
         * Wspierane typy:
         *      - int
         *      - uint
         *      - float
         *      - string
         *      - fixedCombobox
         */
        public string Description; //< Słowny opis w textbox dodany do elementu
        // Stałe
        // private const string EndLineReplacementForSerialization = "⏎";
        // private const string ParameterSpaceReplacementForSerialization = "﹏";

        /// <summary>
        /// Domyślny konstruktor.
        /// </summary>
        /// <param name="classTemplate">Klasa elementu.</param>
        /// <param name="count">Ilość elementu.</param>
        /// <param name="parametersNames">Lista nazw parametrów.</param>
        /// <param name="parametersValues">Lista wartości parametrów.</param>
        public ElementTemplate(string classTemplate, string mainParametrValue, int count, string[] parametersNames, string[] parametersValues, string description)
        {
            ClassTemplate = classTemplate;
            MainParametrValue = mainParametrValue;
            Count = count;
            Description = description;

            Parameters = new Dictionary<string, string>();
            for (int i = 0; i < parametersNames.Length; i++)
            {
                Parameters.Add(parametersNames[i], parametersValues[i]);
            }
        }

        /// <summary>
        /// Funkcja porównująca służąca na potrzeby sortowania.
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public int CompareTo(ElementTemplate elem)
        {
            int classCompareResult = ClassTemplate.CompareTo(elem.ClassTemplate);
            // Porównanie nadrzędności
            if(classCompareResult != 0)
            {
                return classCompareResult;
            }
            else
            {
                return MainParametrValue.CompareTo(elem.MainParametrValue);
            }
        }

        /// <summary>
        /// Funkcja serjalizująca obiekt do zapisu do pliku do formy:
        /// NazwaElementu
        ///     Class:KlasaElementu
        ///     Count:IlośćElementów
        ///     parametr1 parametr2 parametr3 ...
        ///     wartość1 wartość2 wartość3 ...
        ///     Descripton:opis
        /// </summary>
        /// <returns></returns>
        public string SerializeToString()
        {
            string output = "";
            
            // Serializacja klasy elementu
            output += StringFunctions.SerializeCompression(ClassTemplate) + Environment.NewLine;
            // Serializacja parametru głównego
            output += "\tMainParam:" + StringFunctions.SerializeCompression(MainParametrValue) + Environment.NewLine;
            // Serializacja liczby elementów
            output += "\tCount:" + Count.ToString() + Environment.NewLine;
            // Serializacja parametrów
            List<string> tmpTypesList = new List<string>();
            output += "\t";
            foreach (KeyValuePair<string, string> entry in Parameters)
            {
                tmpTypesList.Add(entry.Value);
                output += StringFunctions.SerializeCompression(entry.Key) + " ";
            }
            output.Remove(output.Length - 1); // Usuwanie ostatniego znaku(spacji z wyjścia)
            output += Environment.NewLine + "\t";
            for (int i = 0; i < tmpTypesList.Count; i++)
            {
                output += StringFunctions.SerializeCompression(tmpTypesList[i]) + " ";
            }
            output.Remove(output.Length - 1); // Usuwanie ostatniego znaku(spacji z wyjścia)
            output += Environment.NewLine;
            // Serializacja opisu
            output += "\tDescripton:" + StringFunctions.SerializeCompression(Description) + Environment.NewLine;

            return output;
        }

        /// <summary>
        /// Funkcja parsuje zbiór danych string zawierający dowolną liczbę zserializowanych obiektów typu ElementTemplate.
        /// Obiekty muszą być po sobie bez przerw.
        /// Funkcja nie sprawdza błędów.
        /// </summary>
        /// <param name="serializedData">Dane do sparsowania.</param>
        /// <returns>Lista obiektów</returns>
        public static List<ElementTemplate> Parse(string serializedData)
        {
            // Przygotowywanie listy zwrotnej
            List<ElementTemplate> elemList = new List<ElementTemplate>();
            // Zmienne pomocnicze
            List<string> tmpList = new List<string>();

            // Dzielenie na linie
            string[] lines = serializedData.Split('\n');
            // Usuwanie białych znaków
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
            }

            // Wczytywanie wszystkich elementów
            for(int i=0; i<lines.Length-2; i+=7) // Jest odejmowane 2 od długości ze względu na indeksowanie(1) oraz zapisywanie przez write line(1 dodatkowy enter)
            {
                // Wczytywanie wartości jedno-łańcuchowych
                string elemClass = StringFunctions.SerializeDecompression(lines[i]);
                string mainParamValue = StringFunctions.SerializeDecompression(lines[i + 1].Replace("MainParam:", ""));
                int elemCount = int.Parse(lines[i + 2].Replace("Count:", ""));
                string elemDescription = StringFunctions.SerializeDecompression(lines[i + 5].Replace("Descripton:", ""));
                // Wczytywanie nazw parametrów
                tmpList.Clear();
                tmpList = StringFunctions.SplitBySpace(lines[i + 3]);
                if(lines[i+3].Equals(""))
                {
                    tmpList.Clear();
                }
                else
                {
                    for (int j = 0; j < tmpList.Count; j++)
                    {
                        tmpList[j] = StringFunctions.SerializeDecompression(tmpList[j]);
                    }
                }
                string[] elemParameters = tmpList.ToArray();
                // Wczytywanie wartości parametrów
                tmpList.Clear();
                tmpList = StringFunctions.SplitBySpace(lines[i + 4]);
                if(lines[i+4].Equals(""))
                {
                    tmpList.Clear();
                }
                else
                {
                    for (int j = 0; j < tmpList.Count; j++)
                    {
                        tmpList[j] = StringFunctions.SerializeDecompression(tmpList[j]);
                    }
                }
                string[] elemParametersValues = tmpList.ToArray();

                // Tworzenie obiektu zwrotnego
                elemList.Add(new ElementTemplate(elemClass, mainParamValue, elemCount, elemParameters, elemParametersValues, elemDescription));
            }

            return elemList;
        }
    }
}
