using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using BazaDanychElementow.DataTemplates;


namespace BazaDanychElementow.ViewModels
{
    public class ElementClassTreeViewModel : MyObservableObject
    {
        public ObservableCollection<MasterClassTreeObject> MasterClasses { get; set; }
        public ElementClassTreeViewModel()
        {
            // Generowanie drzewa
            MasterClasses = new ObservableCollection<MasterClassTreeObject>();
            GenerateTree();
        }

        /// <summary>
        /// Funkcja służąca do generowania / odświerzanai drzewa klas.
        /// </summary>
        public void GenerateTree()
        {
            MasterClasses.Clear();
            List<SubClassTreeObject> tmpClassList = new List<SubClassTreeObject>(); //< Zmienna pomocnicza
            foreach (ElementClassTemplate masterClass in Data.ElementsPool.MasterClasses)
            {
                tmpClassList.Clear();
                // Znajdywanie klas podrzędnych
                foreach (ElementClassTemplate subClass in Data.ElementsPool.SubClasses)
                {
                    if (subClass.MasterClassTemplate.Equals(masterClass.Name))
                    {
                        tmpClassList.Add(new SubClassTreeObject(subClass.Name, subClass.MasterClassTemplate));
                    }
                }
                MasterClasses.Add(new MasterClassTreeObject(masterClass.Name, tmpClassList.ToArray()));
            }
        }
    }

    public sealed class MasterClassTreeObject
    {
        public string Name { get; private set; } //< Nazwa klasy obiektów
        public ObservableCollection<SubClassTreeObject> SubClasses { get; set; }

        public MasterClassTreeObject(string name, params SubClassTreeObject[] subClasses)
        {
            Name = name;
            SubClasses = new ObservableCollection<SubClassTreeObject>(subClasses);
        }
    }

    public sealed class SubClassTreeObject
    {
        public string Name { get; private set; } //< Nazwa klasy obiektów
        public string MasterName { get; private set; } //< Nazwa nadrzędnej klasy obiektów

        public SubClassTreeObject(string name, string masterName)
        {
            Name = name;
            MasterName = masterName;
        }
    }
}
