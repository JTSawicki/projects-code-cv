using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BazaDanychElementow.GlobalFunctions;

namespace BazaDanychElementow.DataTemplates
{
    /// <summary>
    /// Szablon obiektu klasy elementu.
    /// </summary>
    public class ElementClassTemplate
    {
        public string MasterClassTemplate; //< Klasa nadrzędna
        public string Name; //< Nazwa klasy elementów
        public Tuple<string, string, string> MainParameter; //< Główny parametr obiektu(nazwa parametru, typ wartości, jednostka)
        public Dictionary<string, List<string>> ComboboxesParameterContent; //< Lista parametrów wybieranych w combobox (nazwa parametru : typy wartości)
        public Dictionary<string, Tuple<string, string>> Parameters; //< Parametry obiektu(nazwa parametru : typy wartości, jednostka)
        /*
         * Wspierane typy:
         *      - int
         *      - uint
         *      - float
         *      - string
         *      - fixedCombobox
         */

        // Stałe
        private const string ParameterSpaceReplacementForSerialization = "﹏";

        /// <summary>
        /// Domyślny konstruktor szablonu.
        /// </summary>
        /// <param name="name">Nazwa klasy elementów.</param>
        /// <param name="parametersNames">Nazwy parametrów obiektów w klasie.</param>
        /// <param name="parametersTypes">Nazwy typów parametrów w klasie.</param>
        /// <param name="masterName">Nazwa klasy nadrzędnej(nie występuje dziedziczenie parametrów).</param>
        public ElementClassTemplate(string name, Tuple<string, string, string> mainParameter, string[] parametersNames, string[] parametersTypes, string[] parametersUnit, string masterName = null, params KeyValuePair<string, List<string>>[] comboboxParametersContent)
        {
            MasterClassTemplate = masterName;
            Name = name;
            MainParameter = mainParameter;
            Parameters = new Dictionary<string, Tuple<string, string>>();
            for (int i=0; i<parametersNames.Length; i++)
            {
                Tuple<string, string> tmpTuple = new Tuple<string, string>(parametersTypes[i], parametersUnit[i]);
                Parameters.Add(parametersNames[i], tmpTuple);
            }
            ComboboxesParameterContent = new Dictionary<string, List<string>>();
            for (int i = 0; i < comboboxParametersContent.Length; i++)
            {
                ComboboxesParameterContent.Add(
                        comboboxParametersContent[i].Key,
                        comboboxParametersContent[i].Value
                    );
            }
        }

        /// <summary>
        /// Funkcja porównująca służąca na potrzeby sortowania.
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public int CompareTo(ElementClassTemplate elem)
        {
            // Porównanie nadrzędności
            if(MasterClassTemplate == null && elem.MasterClassTemplate != null)
            {
                return -1;
            }
            else if (MasterClassTemplate != null && elem.MasterClassTemplate == null)
            {
                return 1;
            }
            else
            {
                // Porównywanie nazwy
                return Name.CompareTo(elem.Name);
            }
        }

        /// <summary>
        /// Funkcja serjalizująca obiekt do zapisu do pliku do formy:
        /// NazwaKlasy
        ///     Master:NazwaKlasyNadrzędnej
        ///     MainParam MainParamType MainParamUnit
        ///     parametr1 parametr2 parametr3 ...
        ///     typ1 typ2 typ3 ...
        ///     jednostka1 jednostka2 jednostka3 ...
        ///     comboboxParam1 comboboxParam2 comboboxParam3 ...
        ///     comboboxValue1ForParam1 comboboxValue2ForParam1 comboboxValue3ForParam1 ...
        ///     ...
        /// </summary>
        /// <returns>Zserializowana klasa;</returns>
        public string SerializeToString()
        {
            string output = "";
            // Serializacja nazwy klasy
            output += StringFunctions.SerializeCompression(Name) + Environment.NewLine;

            // Serializacja klasy master jeżeli istnieje
            if(MasterClassTemplate != null)
            {
                output += "\tMaster:" + StringFunctions.SerializeCompression(MasterClassTemplate) + Environment.NewLine;
            } else
            {
                output += "\tMaster:nullMasterClassTemplate" + Environment.NewLine;
            }

            // Serializacja parametru głównego
            output += "\tMainParameter:" + StringFunctions.SerializeCompression(MainParameter.Item1) + " " + MainParameter.Item2 + " " + StringFunctions.SerializeCompression(MainParameter.Item3) + Environment.NewLine;

            // Serjalizacja list parametrów
            List<string> tmpTypesList = new List<string>();
            List<string> tmpUnitsList = new List<string>();
            output += "\t";
            foreach (KeyValuePair<string, Tuple<string, string>> entry in Parameters)
            {
                tmpTypesList.Add(entry.Value.Item1);
                tmpUnitsList.Add(entry.Value.Item2);
                output += StringFunctions.SerializeCompression(entry.Key) + " ";
            }
            output.Remove(output.Length - 1); // Usuwanie ostatniego znaku(spacji z wyjścia)
            output += Environment.NewLine + "\t";
            for(int i=0; i<tmpTypesList.Count; i++)
            {
                output += tmpTypesList[i] + " ";
            }
            output.Remove(output.Length - 1); // Usuwanie ostatniego znaku(spacji z wyjścia)
            output += Environment.NewLine + "\t";
            for (int i = 0; i < tmpTypesList.Count; i++)
            {
                output += StringFunctions.SerializeCompression(tmpUnitsList[i]) + " ";
            }
            output.Remove(output.Length - 1); // Usuwanie ostatniego znaku(spacji z wyjścia)
            output += Environment.NewLine;

            // Serializacja list danych dla combobox
            List<List<string>> tmpComboContentList = new List<List<string>>();
            output += "\t";
            foreach (KeyValuePair<string, List<string>> entry in ComboboxesParameterContent)
            {
                output += entry.Key + " ";
                tmpComboContentList.Add(entry.Value);
            }
            output.Remove(output.Length - 1); // Usuwanie ostatniego znaku(spacji z wyjścia)
            for (int i=0; i<tmpComboContentList.Count; i++)
            {
                output += Environment.NewLine + "\t";
                for(int j=0; j<tmpComboContentList[i].Count; j++)
                {
                    output += tmpComboContentList[i][j] + " ";
                }
                output.Remove(output.Length - 1); // Usuwanie ostatniego znaku(spacji z wyjścia)
            }

            // Zwracanie wartości
            return output;
        }

        /// <summary>
        /// Funkcja parsuje zbiór danych string zawierający dowolną liczbę zserializowanych obiektów typu ElementClassTemplate.
        /// Obiekty muszą być po sobie bez przerw.
        /// Funkcja nie sprawdza błędów.
        /// </summary>
        /// <param name="serializedData">Dane do sparsowania.</param>
        /// <returns>(Szablony klas nadrzędnych, szablony klas podrzędnych)</returns>
        public static Tuple<List<ElementClassTemplate>, List<ElementClassTemplate>> Parse(string serializedData)
        {
            // Przygotowywanie list do zwrotu
            List<ElementClassTemplate> MasterClasses = new List<ElementClassTemplate>();
            List<ElementClassTemplate> SubClasses = new List<ElementClassTemplate>();
            // Zmienne pomocnicze
            List<string> tmpList = new List<string>();
            // Dzielenie na linie
            string[] lines = serializedData.Split('\n');
            // Usuwanie białych znaków
            for(int i=0; i<lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
            }
            int breakPoint = 0;
            // Wczytywanie wszystkich elementów
            // #todo nie ma wczytywania elementów list combobox
            for(int i=0; i<=lines.Length-2; i+=7) // Jest odejmowane 2 od długości ze względu na indeksowanie(1) oraz zapisywanie przez write line(1 dodatkowy enter)
            {
                breakPoint++;
                string name = StringFunctions.SerializeDecompression(lines[i]);

                string masterName;
                if (lines[i+1].Equals("Master:nullMasterClassTemplate"))
                {
                    masterName = null;
                }
                else
                {
                    masterName = StringFunctions.SerializeDecompression(lines[i+1].Replace("Master:", ""));
                }

                tmpList.Clear();
                tmpList = StringFunctions.SplitBySpace(lines[i + 2]);
                Tuple<string, string, string> mainParameter = new Tuple<string, string, string>(StringFunctions.SerializeDecompression(tmpList[0]), tmpList[1], StringFunctions.SerializeDecompression(tmpList[2]));

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
                string[] parametersNames = tmpList.ToArray();

                string[] parametersTypes = StringFunctions.SplitBySpace(lines[i + 4]).ToArray();

                tmpList.Clear();
                tmpList = StringFunctions.SplitBySpace(lines[i + 5]);
                if(lines[i+5].Equals(""))
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
                string[] parametersUnits = tmpList.ToArray();

                if (masterName == null)
                {
                    MasterClasses.Add(new ElementClassTemplate(name, mainParameter, parametersNames, parametersTypes, parametersUnits));
                }
                else
                {
                    SubClasses.Add(new ElementClassTemplate(name, mainParameter, parametersNames, parametersTypes, parametersUnits, masterName));
                }
            }

            // Zwracanie wczytanych klas elementów
            return new Tuple<List<ElementClassTemplate>, List<ElementClassTemplate>>(MasterClasses, SubClasses);
        }
    }
}
