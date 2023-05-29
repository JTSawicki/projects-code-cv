using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BazaDanychElementow.DataTemplates;
using System.IO;

namespace BazaDanychElementow.Data
{
    public static class ElementsPool
    {
        // Ścieżki do plików zapisu
        private const string ElementsSaveFile = "Data\\Elements.txt";
        private const string ElementClassesSaveFile = "Data\\ElementClasses.txt";

        // Tablice danych
        public static List<ElementTemplate> Elements;
        public static List<ElementClassTemplate> MasterClasses;
        public static List<ElementClassTemplate> SubClasses;

        // Flaga danych czy było edytowane w czasie pracy programu. Flaga na potrzeby kontroli zapisu do pliku.
        public static bool IsDatabaseEdited = false;

        static ElementsPool()
        {
            // Wczytywanie klas obiektów
            string inputText = File.ReadAllText(ElementClassesSaveFile, Encoding.UTF8);
            Tuple<List<ElementClassTemplate>, List<ElementClassTemplate>>  Classes = ElementClassTemplate.Parse(inputText);
            MasterClasses = Classes.Item1;
            SubClasses = Classes.Item2;

            //Wczytywanie obiektów
            inputText = File.ReadAllText(ElementsSaveFile, Encoding.UTF8);
            Elements = ElementTemplate.Parse(inputText);

            // Wypisanie do konsoli na potrzeby debugowania przy tworzeniu programu
            WriteToConsole();
        }

        /// <summary>
        /// Funkcja zapisuje obecny stan do pliku.
        /// </summary>
        public static void SavePollToFile()
        {
            // Zapisywanie klas elementó
            List<string> ElementClassesRepresentations = new List<string>();
            foreach(ElementClassTemplate elem in MasterClasses)
            {
                ElementClassesRepresentations.Add(elem.SerializeToString());
            }
            foreach (ElementClassTemplate elem in SubClasses)
            {
                ElementClassesRepresentations.Add(elem.SerializeToString());
            }
            using (StreamWriter sw = File.CreateText(ElementClassesSaveFile))
            {
                foreach(string text in ElementClassesRepresentations)
                {
                    sw.WriteLine(text);
                }
            }

            // Zapisywanie elementów
            List<string> ElementRepresentations = new List<string>();
            foreach (ElementTemplate elem in Elements)
            {
                ElementRepresentations.Add(elem.SerializeToString());
            }
            using (StreamWriter sw = File.CreateText(ElementsSaveFile))
            {
                foreach (string text in ElementRepresentations)
                {
                    sw.WriteLine(text);
                }
            }

            // Zmiana flagi zedytowania
            IsDatabaseEdited = false;
        }

        /// <summary>
        /// Funkcja wypisująca zbiór elementów i klas do konsoli
        /// </summary>
        public static void  WriteToConsole()
        {
            Console.WriteLine("Master Classes ----- -----");
            foreach (ElementClassTemplate elem in MasterClasses)
            {
                Console.WriteLine(elem.SerializeToString());
            }
            Console.WriteLine("Sub Classes    ----- -----");
            foreach (ElementClassTemplate elem in SubClasses)
            {
                Console.WriteLine(elem.SerializeToString());
            }
            Console.WriteLine("Elements       ----- -----");
            foreach (ElementTemplate elem in Elements)
            {
                Console.WriteLine("Opis: " + elem.Description);
                Console.WriteLine(elem.SerializeToString());
            }
        }

        /// <summary>
        /// Funkcja służąca do znajdywania szablonu klasy na podstawie nazwy klasy.
        /// </summary>
        /// <param name="className">Nazwa klasy.</param>
        /// <param name="masterClassName">Nazwa klasy nadrzędnej. Jeżeli null to brak lub nie wiadomo.</param>
        /// <returns>(czy znaleziono, czy klasa jest klasą nadrzędną, szablon klasy)</returns>
        public static Tuple<bool, bool, ElementClassTemplate> FindClassTemplate(string className, string masterClassName = null)
        {
            // Szukanie dla nieokreślonej klasy nadrzędnej
            if (masterClassName == null)
            {
                foreach(ElementClassTemplate elem in MasterClasses)
                {
                    if(elem.Name.Equals(className))
                    {
                        return new Tuple<bool, bool, ElementClassTemplate>(true, true, elem);
                    }
                }
                foreach(ElementClassTemplate elem in SubClasses)
                {
                    if (elem.Name.Equals(className))
                    {
                        return new Tuple<bool, bool, ElementClassTemplate>(true, true, elem);
                    }
                }
            }
            // Szukanie dla określonej klasy nadrzędnej
            else if(masterClassName != null)
            {
                foreach (ElementClassTemplate elem in SubClasses)
                {
                    if (elem.Name.Equals(className) && elem.MasterClassTemplate.Equals(masterClassName))
                    {
                        return new Tuple<bool, bool, ElementClassTemplate>(true, true, elem);
                    }
                }
            }
            // Nie udało się nic znaleźć
            return new Tuple<bool, bool, ElementClassTemplate>(false, false, null);
        }
    }
}
