using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanychElementow.ViewModels
{
    class MasterViewModel
    {
        private static MasterViewModel masterObject = null;


        public static MasterViewModel GetMasterViewModel()
        {
            if (masterObject == null)
            {
                masterObject = new MasterViewModel();
            }

            return masterObject;
        }

        public ElementClassTreeViewModel elementClassTreeViewModel { get; private set; }
        public DataTypeViewModel dataTypeViewModel { get; private set; }
        public AddElementClassWindowViewModel addElementClassWindowViewModel { get; private set; }
        public AddElementWindowViewModel addElementWindowViewModel { get; private set; }
        public MainWindowViewModel mainWindowViewModel { get; private set; }
        private MasterViewModel()
        {
            elementClassTreeViewModel = new ElementClassTreeViewModel();
            dataTypeViewModel = new DataTypeViewModel();
            addElementClassWindowViewModel = new AddElementClassWindowViewModel();
            addElementWindowViewModel = new AddElementWindowViewModel();
            mainWindowViewModel = new MainWindowViewModel();
        }

        /// <summary>
        /// Funkcja odświerzająca wszystkie powiązania danych związane z listą klas elementów
        /// </summary>
        public void ElementClassEditUpdate()
        {
            elementClassTreeViewModel.GenerateTree();
            mainWindowViewModel.GenerateMasterClassListForCombobox();
            mainWindowViewModel.GenerateSubClassListForCombobox();
        }

        /// <summary>
        /// Funkcja odświerzająca wszystkie powiązania danych związane z listą elementów
        /// </summary>
        public void ElementEditUpdate()
        {
            mainWindowViewModel.GenerateElementTree();
        }
    }
}
