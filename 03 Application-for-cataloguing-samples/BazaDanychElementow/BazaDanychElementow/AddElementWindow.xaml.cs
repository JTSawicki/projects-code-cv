using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BazaDanychElementow
{
    /// <summary>
    /// Logika interakcji dla klasy AddElementWindow.xaml
    /// </summary>
    public partial class AddElementWindow : Window
    {
        ViewModels.MasterViewModel viewModel = ViewModels.MasterViewModel.GetMasterViewModel();
        List<Controls.ParameterInputControl> ParameterInputFileds = new List<Controls.ParameterInputControl>();
        public AddElementWindow()
        {
            InitializeComponent();
            // Inicjowanie kontekstu danych dla wiązań danych
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Funkcja wykorzystywana przez przyciski bez docelowej implementacji.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dummy_ButtonClick(object sender, RoutedEventArgs e)
        {
            new MyMaterialMessageBox("Brak implementacji funkcjonalności.", MyMaterialMessageBox.MessageBoxType.Error, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
        }

        /// <summary>
        /// Przycisk do wybierania klasy elementów.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectElementClass_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Kontrola czy nie ma już wybranej klasy elementów
            if(ParameterInputFileds.Count != 0)
            {
                MyMaterialMessageBox message = new MyMaterialMessageBox("Czy zmienić wybraną klasę?\nUsunie to wprowadzone dane.", MyMaterialMessageBox.MessageBoxType.Confirmation, MyMaterialMessageBox.MessageBoxButtons.YesNo);
                if(message.ShowDialog() == false)
                {
                    return;
                }
            }
            // Kontrola czy wybrano klasę
            if( ! (ElementClassesTree.SelectedItem is ViewModels.MasterClassTreeObject) &&
                ! (ElementClassesTree.SelectedItem is ViewModels.SubClassTreeObject) )
            {
                new MyMaterialMessageBox("Nie wybrano klasy elementu.", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                return;
            }

            // Wyszukiwanie klasy elementów
            string elementClassName;
            if(ElementClassesTree.SelectedItem is ViewModels.MasterClassTreeObject)
            {
                elementClassName = (ElementClassesTree.SelectedItem as ViewModels.MasterClassTreeObject).Name;
            }
            else
            {
                elementClassName = (ElementClassesTree.SelectedItem as ViewModels.SubClassTreeObject).Name;
            }
            DataTemplates.ElementClassTemplate elementClass = Data.ElementsPool.FindClassTemplate(elementClassName).Item3;

            // Ustawianie wartości pól GUI
            viewModel.addElementWindowViewModel.ChosenClassName = elementClassName;
            viewModel.addElementWindowViewModel.MainParameterName = elementClass.MainParameter.Item1;
            viewModel.addElementWindowViewModel.MainParameterUnit = GlobalFunctions.StringFunctions.ConnectTypeAndUnit(elementClass.MainParameter.Item2, elementClass.MainParameter.Item3);

            // Generowanie pól ustawień
            int paramIncrementer = 0;
            ParameterStackPanel.Children.Clear();
            ParameterInputFileds.Clear();
            foreach (KeyValuePair<string, Tuple<string, string>> param in elementClass.Parameters)
            {
                ParameterInputFileds.Add(new Controls.ParameterInputControl());
                ParameterInputFileds[paramIncrementer].viewModel.Name = param.Key;
                ParameterInputFileds[paramIncrementer].viewModel.Unit = GlobalFunctions.StringFunctions.ConnectTypeAndUnit(param.Value.Item1, param.Value.Item2);

                ParameterStackPanel.Children.Add(ParameterInputFileds[paramIncrementer]);

                paramIncrementer++; // Ta linia musi być na końcu pętli
            }
        }

        /// <summary>
        /// Przywraca domyślne parametry pól.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToDefault_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Ustawianie wartości pól
            viewModel.addElementWindowViewModel.ChosenClassName = "";
            viewModel.addElementWindowViewModel.ElementDescription = "";
            viewModel.addElementWindowViewModel.ElementCount = "";
            viewModel.addElementWindowViewModel.MainParameterName = "";
            viewModel.addElementWindowViewModel.MainParameterUnit = "";
            viewModel.addElementWindowViewModel.MainParameterValue = "";
            // Usuwanie pól parametrów dodatkowych
            ParameterInputFileds.Clear();
            ParameterStackPanel.Children.Clear();
        }

        /// <summary>
        /// Funkcja guzika zamykającego okno
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Dodaje element do zbioru danych.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddElementClass_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Sprawdzanie czy wybrano klasę obiektu
            if(viewModel.addElementWindowViewModel.ChosenClassName.Equals(""))
            {
                new MyMaterialMessageBox("Nie wybrano nawet klasy elementu XDD", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                return;
            }
            // Sprawdzanie numerycznej poprawności ilości elementów
            if(! GlobalFunctions.CheckFunctions.ValidateParameterType(viewModel.addElementWindowViewModel.ElementCount, "int"))
            {
                new MyMaterialMessageBox("Niepoprawna ilośc elementów.\nPowinna to być liczba typu int.", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                return;
            }
            // Sprawdzenie wypełnienia pola parametru głównego oraz czy nie wypełniono go pustymi znakami
            if(viewModel.addElementWindowViewModel.MainParameterValue.Trim().Equals(""))
            {
                new MyMaterialMessageBox("Niepodano parametru głównego lub podano znaki puste.", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                return;
            }
            // Sprawdzenie czy wypełniono wszystkie pola parametrów dodatkowych
            int paramCout = ParameterInputFileds.Count;
            for(int i=0; i<paramCout; i++)
            {
                if(ParameterInputFileds[i].viewModel.Value.Trim().Equals(""))
                {
                    new MyMaterialMessageBox("Nie podano parametru: " + ParameterInputFileds[i].viewModel.Name + "\nLub podano znaki puste.", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                    return;
                }
            }

            // Sprawdzanie czy nie istnieje już taki element w bazie(klasa i wartość parametru głównego)
            foreach(DataTemplates.ElementTemplate elem in Data.ElementsPool.Elements)
            {
                if( elem.ClassTemplate.Equals(viewModel.addElementWindowViewModel.ChosenClassName) &&
                    elem.MainParametrValue.Equals(viewModel.addElementWindowViewModel.MainParameterValue))
                {
                    MyMaterialMessageBox message = new MyMaterialMessageBox("Już istnieje element o takiej samej klasie i parametrze głównym.\nChcesz dodać ten element mimo to?", MyMaterialMessageBox.MessageBoxType.Confirmation, MyMaterialMessageBox.MessageBoxButtons.YesNo);
                    if (message.ShowDialog() == false)
                    {
                        return;
                    }
                    else
                    {
                        // Wyjście z pętli foreach
                        break;
                    }
                }
            }

            // Zbieranie parametrów dodawanego elementu
            string className = viewModel.addElementWindowViewModel.ChosenClassName;
            string mainValue = viewModel.addElementWindowViewModel.MainParameterValue;
            int elementCount = int.Parse(viewModel.addElementWindowViewModel.ElementCount);
            string elemDescription = viewModel.addElementWindowViewModel.ElementDescription;
            List<string> param = new List<string>();
            List<string> paramValues = new List<string>();
            for (int i = 0; i < paramCout; i++)
            {
                param.Add(ParameterInputFileds[i].viewModel.Name);
                paramValues.Add(ParameterInputFileds[i].viewModel.Value);
            }

            // Dodawanie elementu do zbioru elementów
            Data.ElementsPool.Elements.Add(new DataTemplates.ElementTemplate(className, mainValue, elementCount, param.ToArray(), paramValues.ToArray(), elemDescription));

            // Odświerzanie zależności listy klas
            viewModel.ElementEditUpdate();

            // Zmiana flagi edycji
            Data.ElementsPool.IsDatabaseEdited = true;
        }
    }
}
