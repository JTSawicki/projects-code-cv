using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using BazaDanychElementow.DataTemplates;

namespace BazaDanychElementow.ViewModels
{
    public class MainWindowViewModel : MyObservableObject
    {
        // Zmienne dla powiązań z GUI
        public ObservableCollection<ElementTreeObject> ElementTree { get; set; }
        List<string> ElementMasterClass_ComboboxList_; //< Lista nazw klas dla elementu combobox
        string ElementMasterClass_ComboboxSelectedItem_; //< Wybrany element z Combobox
        List<string> ElementSubClass_ComboboxList_; //< Lista nazw klas dla elementu combobox
        string ElementSubClass_ComboboxSelectedItem_; //< Wybrany element z Combobox
        List<string> FilterSelect_ComboboxList_; //< Lista parametrów filtrujących dla combobox służących do ustawiania filtru
        string CountFilter_ComboboxSelectedItem_; //< Wybrany element w combobox od filtru
        string ValueFilter_ComboboxSelectedItem_; //< Wybrany element w combobox od filtru
        string ValueFilterValue_ = ""; //< Input filtra wartości od elementu textbox
        string CoutFilterValue_ = ""; //< Input filtra ilościowego od elementu textbox


        // Stałe funkcyjne
        public const string ClassComobox_All = "Wszystkie klasy\u2800";

        public MainWindowViewModel()
        {
            ElementTree = new ObservableCollection<ElementTreeObject>();
            GenerateElementTree();
            ElementMasterClass_ComboboxList_ = new List<string>();
            GenerateMasterClassListForCombobox();
            ElementSubClass_ComboboxList_ = new List<string>();

            CountFilter_ComboboxSelectedItem_ = ClassComobox_All;
            ValueFilter_ComboboxSelectedItem_ = ClassComobox_All;

            FilterSelect_ComboboxList_ = new List<string>(new string[] {
                "brak",
                "<",
                "<=",
                "=",
                ">=",
                ">"
            });
        }

        /// <summary>
        /// Funkcja generująca listę klas nadrzędnych elementów  dla elementu combobox
        /// </summary>
        public void GenerateMasterClassListForCombobox()
        {
            List<string> tmpClassesNamesBuffor = new List<string>(new string[] {ClassComobox_All});
            foreach(ElementClassTemplate elem in Data.ElementsPool.MasterClasses)
            {
                tmpClassesNamesBuffor.Add(elem.Name);
            }
            ElementMasterClass_ComboboxList = tmpClassesNamesBuffor;
        }

        /// <summary>
        /// Funkcja generująca listę klas podrzędnych elementów  dla elementu combobox
        /// </summary>
        public void GenerateSubClassListForCombobox()
        {
            // Kontrola dla bezpieczeństwa kodu
            if (ElementMasterClass_ComboboxSelectedItem == null ||
                ElementMasterClass_ComboboxSelectedItem.Equals(ClassComobox_All))
            {
                return;
            }
            List<string> tmpClassesNamesBuffor = new List<string>(new string[] { ClassComobox_All });
            foreach (ElementClassTemplate elem in Data.ElementsPool.SubClasses)
            {
                if (elem.MasterClassTemplate.Equals(ElementMasterClass_ComboboxSelectedItem))
                {
                    tmpClassesNamesBuffor.Add(elem.Name);
                }
            }
            ElementSubClass_ComboboxList = tmpClassesNamesBuffor;
        }

        /// <summary>
        /// Funkcja generuje nową zawartość drzewa ElementTree przy uwzględnieniu filtrów.
        /// </summary>
        /// <param name="ElementClassesList">Lista klas akceptowanych. (null - wszystkie klasy)</param>
        /// <param name="ValueFilter">Filtr głównego parametru: ["<", "<=", "=", ">=", ">", null] (null - brak filtra)</param>
        /// <param name="CountFilter">Filtr ilościowy          : ["<", "<=", "=", ">=", ">", null] (null - brak filtra)</param>
        /// <param name="ValueFilterContent">Wartość porównawcza dla filtra parametru głównego</param>
        /// <param name="CountFilterContent">Wartość porównawcza dla filtra ilościowego</param>
        public void GenerateElementTree(List<string> ElementClassesList = null, string ValueFilter = null, string CountFilter = null, string ValueFilterContent = "", int CountFilterContent = 0)
        {
            // #todo brak implementacji filtra

            ElementTree.Clear();
            List<ElementTreeObject> ElementsToAdd = new List<ElementTreeObject>(); //< Buffor tymczasowy
            bool ElementAddingFlag = false; //< Flaga symbolizująca czy dodać element do bazy

            for (int i=0; i<Data.ElementsPool.Elements.Count; i++)
            {
                ElementAddingFlag = false;

                // Kontrolowanie filtrów
                if (ElementClassesList == null)
                {
                    ElementAddingFlag = GlobalFunctions.CheckFunctions.ElementFilter(Data.ElementsPool.Elements[i], ValueFilter, CountFilter, ValueFilterContent, CountFilterContent);
                }
                else
                {
                    foreach(string className in ElementClassesList)
                    {
                        if(Data.ElementsPool.Elements[i].ClassTemplate.Equals(className))
                        {
                            ElementAddingFlag = GlobalFunctions.CheckFunctions.ElementFilter(Data.ElementsPool.Elements[i], ValueFilter, CountFilter, ValueFilterContent, CountFilterContent);
                            break;
                        }
                    }
                }

                // Dodawanie elementu do tymczasowej tablicy
                if(ElementAddingFlag)
                {
                    // Znajdywanie szablonu klasy elementu
                    ElementClassTemplate currentElemClassTemplate = Data.ElementsPool.FindClassTemplate(Data.ElementsPool.Elements[i].ClassTemplate).Item3;
                    // Generowanie tekstu parametru głównego do wyświetlania przy elemencie
                    string ElementValueShowcase = Data.ElementsPool.Elements[i].MainParametrValue + " "
                        + GlobalFunctions.StringFunctions.ConnectTypeAndUnit(currentElemClassTemplate.MainParameter.Item2, currentElemClassTemplate.MainParameter.Item3);

                    ElementsToAdd.Add(new ElementTreeObject(
                        Data.ElementsPool.Elements[i].ClassTemplate,
                        Data.ElementsPool.Elements[i].MainParametrValue,
                        ElementValueShowcase
                        ));
                }
            }

            // Dodawanie zakresu elementów
            foreach(ElementTreeObject elem in ElementsToAdd)
            {
                ElementTree.Add(elem);
            }
        }

        public List<string> ElementMasterClass_ComboboxList
        {
            get { return ElementMasterClass_ComboboxList_; }
            set
            {
                ElementMasterClass_ComboboxList_ = value;
                OnPropertyChanged("ElementMasterClass_ComboboxList");
            }
        }

        public string ElementMasterClass_ComboboxSelectedItem
        {
            get { return ElementMasterClass_ComboboxSelectedItem_; }
            set
            {
                ElementMasterClass_ComboboxSelectedItem_ = value;
                OnPropertyChanged("ElementMasterClass_ComboboxSelectedItem");
            }
        }

        public List<string> ElementSubClass_ComboboxList
        {
            get { return ElementSubClass_ComboboxList_; }
            set
            {
                ElementSubClass_ComboboxList_ = value;
                OnPropertyChanged("ElementSubClass_ComboboxList");
            }
        }

        public string ElementSubClass_ComboboxSelectedItem
        {
            get { return ElementSubClass_ComboboxSelectedItem_; }
            set
            {
                ElementSubClass_ComboboxSelectedItem_ = value;
                OnPropertyChanged("ElementSubClass_ComboboxSelectedItem");
            }
        }

        public List<string> FilterSelect_ComboboxList
        {
            get { return FilterSelect_ComboboxList_; }
            set
            {
                FilterSelect_ComboboxList_ = value;
                OnPropertyChanged("FilterSelect_ComboboxList");
            }
        }

        public string CountFilter_ComboboxSelectedItem
        {
            get { return CountFilter_ComboboxSelectedItem_; }
            set
            {
                CountFilter_ComboboxSelectedItem_ = value;
                OnPropertyChanged("CountFilter_ComboboxSelectedItem");
            }
        }

        public string ValueFilter_ComboboxSelectedItem
        {
            get { return ValueFilter_ComboboxSelectedItem_; }
            set
            {
                ValueFilter_ComboboxSelectedItem_ = value;
                OnPropertyChanged("ValueFilter_ComboboxSelectedItem");
            }
        }

        public string ValueFilterValue
        {
            get { return ValueFilterValue_; }
            set
            {
                ValueFilterValue_ = value;
                OnPropertyChanged("ValueFilterValue");
            }
        }

        public string CoutFilterValue
        {
            get { return CoutFilterValue_; }
            set
            {
                CoutFilterValue_ = value;
                OnPropertyChanged("CoutFilterValue");
            }
        }
    }

    public class ElementTreeObject
    {
        public string ElementClass { get; private set; } //< Klasa elementu
        public string MainParamValue { get; private set; } //< Wartość głównego parametru (rzeczywiste pole do identyfikacji elementu)
        public string MainParamShowcase { get; private set; } //< Wartość parametru głównego do wyświetlenia np. (10 MOhm)

        public ElementTreeObject(string elementClass, string mainParamValue, string mainParamShowcase)
        {
            ElementClass = elementClass;
            MainParamValue = mainParamValue;
            MainParamShowcase = mainParamShowcase;
        }
    }
}
