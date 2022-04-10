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
    /// Logika interakcji dla klasy AddElementClassWindow.xaml
    /// </summary>
    public partial class AddElementClassWindow : Window
    {
        ViewModels.MasterViewModel viewModel = ViewModels.MasterViewModel.GetMasterViewModel();
        List<Controls.ControlFiled> ParametersFileds = new List<Controls.ControlFiled>();
        public AddElementClassWindow()
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

        private void AddParameterFileds_ButtonClick(object sender, RoutedEventArgs e)
        {
            GenerateParameterFileds();
        }

        /// <summary>
        /// Przywraca domyślne parametry pól.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToDefault_ButtonClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("To default" + viewModel.addElementClassWindowViewModel.HasMasterClass.ToString());
            // Zerowanie pól
            viewModel.addElementClassWindowViewModel.HasMasterClass = false;
            viewModel.addElementClassWindowViewModel.MainParameterName = "";
            viewModel.addElementClassWindowViewModel.MainParameterType = "";
            viewModel.addElementClassWindowViewModel.MainParameterUnit = "";
            viewModel.addElementClassWindowViewModel.parameterCount = "0";
            viewModel.addElementClassWindowViewModel.ClassName = "";
            // Generowanie pustej listy parametrów
            GenerateParameterFileds();
        }

        private void GenerateParameterFileds()
        {
            ParametersFileds.Clear();
            ParameterStackPanel.Children.Clear();
            int additionalParametersCout = int.Parse(viewModel.addElementClassWindowViewModel.parameterCount);
            for(int i=0; i<additionalParametersCout; i++)
            {
                ParametersFileds.Add(new Controls.ControlFiled());
                ParametersFileds[i].viewModel.ParameterInfo = "Parametr " + i.ToString();
                ParameterStackPanel.Children.Add(ParametersFileds[i]);
            }
        }

        /// <summary>
        /// Dodaje element do zbioru danych.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddElementClass_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Kontrola poprawności wprowadzonych parametrów
            // Nazwa
            if(viewModel.addElementClassWindowViewModel.ClassName.Equals(""))
            {
                new MyMaterialMessageBox("Niepoprawna nazwa klasy elementu (brak danej).", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                return;
            }
            // Parametr główny
            if (viewModel.addElementClassWindowViewModel.MainParameterName.Equals("") ||
                viewModel.addElementClassWindowViewModel.MainParameterType.Equals("") ||
                viewModel.addElementClassWindowViewModel.MainParameterUnit.Equals(""))
            {
                new MyMaterialMessageBox("Niepoprawne dane parametru głównego (brak danej).", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                return;
            }
            // Parametry dodatkowe
            int parameterCount = int.Parse(viewModel.addElementClassWindowViewModel.parameterCount);
            for (int i=0; i<parameterCount; i++)
            {
                if (ParametersFileds[i].viewModel.Name.Equals("") ||
                    ParametersFileds[i].viewModel.Type.Equals("") ||
                    ParametersFileds[i].viewModel.Unit.Equals(""))
                {
                    new MyMaterialMessageBox("Niepoprawne dane parametru dodatkowego (brak danej): " + ParametersFileds[i].viewModel.ParameterInfo, MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                    return;
                }
            }
            // Poprawność klasy nadrzędnej
            if (viewModel.addElementClassWindowViewModel.HasMasterClass &&
                !(ElementClassesTree.SelectedItem is ViewModels.MasterClassTreeObject) )
            {
                new MyMaterialMessageBox("Wybrano niepoprawną klasę nadrzędną z drzewa klas.", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                return;
            }

            // Sprawdzenie czy nie występuje już taka sama klasa(identyczna nazwa klasy i klasy nadrzędnej)
            if(viewModel.addElementClassWindowViewModel.HasMasterClass)
            {
                foreach(DataTemplates.ElementClassTemplate elemClass in Data.ElementsPool.SubClasses)
                {
                    if(elemClass.Name.Equals(viewModel.addElementClassWindowViewModel.ClassName) && elemClass.MasterClassTemplate.Equals((ElementClassesTree.SelectedItem as ViewModels.MasterClassTreeObject).Name) )
                    {
                        new MyMaterialMessageBox("Już istnieje taka klasa.", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                        return;
                    }
                }
            }
            else
            {
                foreach (DataTemplates.ElementClassTemplate elemClass in Data.ElementsPool.MasterClasses)
                {
                    if (elemClass.Name.Equals(viewModel.addElementClassWindowViewModel.ClassName) )
                    {
                        new MyMaterialMessageBox("Już istnieje taka klasa.", MyMaterialMessageBox.MessageBoxType.Warning, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                        return;
                    }
                }
            }

            // Zbieranie parametrów
            string name = viewModel.addElementClassWindowViewModel.ClassName;
            Tuple<string, string, string> mainParameter = new Tuple<string, string, string>(
                viewModel.addElementClassWindowViewModel.MainParameterName,
                viewModel.addElementClassWindowViewModel.MainParameterType,
                viewModel.addElementClassWindowViewModel.MainParameterUnit
                );
            List<string> parametersNames = new List<string>();
            List<string> parametersTypes = new List<string>();
            List<string> parametersUnits = new List<string>();
            for (int i = 0; i < parameterCount; i++)
            {
                parametersNames.Add(ParametersFileds[i].viewModel.Name);
                parametersTypes.Add(ParametersFileds[i].viewModel.Type);
                parametersUnits.Add(ParametersFileds[i].viewModel.Unit);
            }

            // Dodawanie elementu
            if (viewModel.addElementClassWindowViewModel.HasMasterClass)
            {
                string masterClass = (ElementClassesTree.SelectedItem as ViewModels.MasterClassTreeObject).Name;
                Data.ElementsPool.SubClasses.Add(new DataTemplates.ElementClassTemplate(name, mainParameter, parametersNames.ToArray(), parametersTypes.ToArray(), parametersUnits.ToArray(), masterClass) );
            }
            else
            {
                Data.ElementsPool.MasterClasses.Add(new DataTemplates.ElementClassTemplate(name, mainParameter, parametersNames.ToArray(), parametersTypes.ToArray(), parametersUnits.ToArray()));
            }

            // Odświerzanie zależności listy klas elementów w GUI
            viewModel.ElementClassEditUpdate();

            // Zmiana flagi edycji
            Data.ElementsPool.IsDatabaseEdited = true;
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
    }
}
