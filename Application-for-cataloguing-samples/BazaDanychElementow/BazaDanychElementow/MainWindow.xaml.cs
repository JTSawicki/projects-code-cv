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
using System.Windows.Navigation;
using System.Windows.Shapes;

using BazaDanychElementow.ViewModels;
using BazaDanychElementow.DataTemplates;

namespace BazaDanychElementow
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModels.MasterViewModel viewModel = ViewModels.MasterViewModel.GetMasterViewModel();
        public MainWindow()
        {
            InitializeComponent();
            // Inicjowanie kontekstu danych dla wiązań danych
            this.DataContext = viewModel;
        }

        private void OpenProgramInfo(object sender, RoutedEventArgs e)
        {
            // Console.WriteLine();
            InfoScreen infoScreen = new InfoScreen();
            infoScreen.Show();
        }

        private void OpenAddElementClassWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            AddElementClassWindow addElementClassWindow = new AddElementClassWindow();
            addElementClassWindow.Show();
        }

        private void OpenAddElementWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            AddElementWindow addElementWindow = new AddElementWindow();
            addElementWindow.Show();
        }

        private void SaveDatabaseToFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            Data.ElementsPool.SavePollToFile();
        }

        /// <summary>
        /// Obsługa wybrania elementu z listy elementów.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementsTree_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            DisplayElementInfo();
        }

        private void DisplayElementInfo()
        {
            // Sprawdzenie czy nie został element odznaczony(odświerzenie drzewa)
            if (!(ElementsTree.SelectedItem is ElementTreeObject))
            {
                ElementInfoTextBoxFlow.Blocks.Clear();
                ChangeCountOfElementButton.IsEnabled = false;
                RemoveElementButton.IsEnabled = false;
                return;
            }
            // Ustawianie widoczności przycisków
            ChangeCountOfElementButton.IsEnabled = true;
            RemoveElementButton.IsEnabled = true;

            // Znajdywanie wybranego elementu i jego klasy
            ElementTreeObject selectedTreeElement = ElementsTree.SelectedItem as ElementTreeObject;
            ElementClassTemplate selectedElementClass = Data.ElementsPool.FindClassTemplate(selectedTreeElement.ElementClass).Item3;
            ElementTemplate selectedElement = new ElementTemplate("", "", 0, new string[] { }, new string[] { }, ""); // Przypisanie bez sęsu w celu usunięcia błędów
            foreach (ElementTemplate elem in Data.ElementsPool.Elements)
            {
                if (elem.ClassTemplate.Equals(selectedTreeElement.ElementClass) &&
                    elem.MainParametrValue.Equals(selectedTreeElement.MainParamValue))
                {
                    selectedElement = elem;
                    break;
                }
            }

            // Generowanie wyświetlanego tekstu
            ElementInfoTextBoxFlow.Blocks.Clear();
            Paragraph paragraphBuffer; //< Bufor dla generowania nowych paragafów
            Run runParagraphBuffer; //< Bufor dla generowania tekstu

            // Klasa elementu
            paragraphBuffer = new Paragraph();
            paragraphBuffer.TextAlignment = TextAlignment.Center;
            runParagraphBuffer = new Run();
            runParagraphBuffer.Text = selectedElement.ClassTemplate;
            paragraphBuffer.Inlines.Add(new Bold(runParagraphBuffer));
            ElementInfoTextBoxFlow.Blocks.Add(paragraphBuffer);
            // Parametr główny elementu
            paragraphBuffer = new Paragraph();
            runParagraphBuffer = new Run();
            runParagraphBuffer.Text = selectedElementClass.MainParameter.Item1 + ": ";
            paragraphBuffer.Inlines.Add(new Bold(runParagraphBuffer));
            runParagraphBuffer = new Run();
            runParagraphBuffer.Text = selectedElement.MainParametrValue + " " + GlobalFunctions.StringFunctions.ConnectTypeAndUnit(selectedElementClass.MainParameter.Item2, selectedElementClass.MainParameter.Item3);
            paragraphBuffer.Inlines.Add(runParagraphBuffer);
            ElementInfoTextBoxFlow.Blocks.Add(paragraphBuffer);
            // Ilość elementu
            paragraphBuffer = new Paragraph();
            runParagraphBuffer = new Run();
            runParagraphBuffer.Text = "Ilość: ";
            paragraphBuffer.Inlines.Add(new Bold(runParagraphBuffer));
            runParagraphBuffer = new Run();
            runParagraphBuffer.Text = selectedElement.Count.ToString();
            paragraphBuffer.Inlines.Add(runParagraphBuffer);
            ElementInfoTextBoxFlow.Blocks.Add(paragraphBuffer);
            if (selectedElementClass.MasterClassTemplate != null)
            {
                // Ilość elementu
                paragraphBuffer = new Paragraph();
                runParagraphBuffer = new Run();
                runParagraphBuffer.Text = "Klasa nadrzędna: ";
                paragraphBuffer.Inlines.Add(new Bold(runParagraphBuffer));
                runParagraphBuffer = new Run();
                runParagraphBuffer.Text = selectedElementClass.MasterClassTemplate;
                paragraphBuffer.Inlines.Add(runParagraphBuffer);
                ElementInfoTextBoxFlow.Blocks.Add(paragraphBuffer);
            }
            // Wypisywanie parametrów dodatkowych
            if (selectedElement.Parameters.Keys.Count > 0)
            {
                paragraphBuffer = new Paragraph();
                runParagraphBuffer = new Run();
                runParagraphBuffer.Text = "Parametry dodatkowe:";
                paragraphBuffer.Inlines.Add(new Bold(runParagraphBuffer));
                ElementInfoTextBoxFlow.Blocks.Add(paragraphBuffer);
                foreach (string param in selectedElement.Parameters.Keys)
                {
                    paragraphBuffer = new Paragraph();
                    runParagraphBuffer = new Run();
                    runParagraphBuffer.Text = "\t" + param + ": ";
                    paragraphBuffer.Inlines.Add(new Bold(runParagraphBuffer));
                    runParagraphBuffer = new Run();
                    runParagraphBuffer.Text = "\t" + selectedElement.Parameters[param] + " "
                        + GlobalFunctions.StringFunctions.ConnectTypeAndUnit(selectedElementClass.Parameters[param].Item1, selectedElementClass.Parameters[param].Item2);
                    paragraphBuffer.Inlines.Add(runParagraphBuffer);
                    ElementInfoTextBoxFlow.Blocks.Add(paragraphBuffer);
                }
            }
            // Wypisywanie opisu elementu
            paragraphBuffer = new Paragraph();
            runParagraphBuffer = new Run();
            runParagraphBuffer.Text = "Opis elementu:";
            paragraphBuffer.Inlines.Add(new Bold(runParagraphBuffer));
            ElementInfoTextBoxFlow.Blocks.Add(paragraphBuffer);
            paragraphBuffer = new Paragraph();
            runParagraphBuffer = new Run();
            runParagraphBuffer.Text = selectedElement.Description;
            paragraphBuffer.Inlines.Add(runParagraphBuffer);
            ElementInfoTextBoxFlow.Blocks.Add(paragraphBuffer);
        }

        public void MasterClassCombobox_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.mainWindowViewModel.GenerateSubClassListForCombobox();
            viewModel.mainWindowViewModel.ElementSubClass_ComboboxSelectedItem = null;
            SubClassChoiceCombobox.IsEnabled = true;
        }

        /// <summary>
        /// Funkcja przycisku włączającego filtrowanie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filter_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Zmienne przechowywujące parametry filtra
            List<string> ElementClassesList;
            string ValueFilter;
            string CountFilter;
            string ValueFilterContent;
            int CountFilterContent;

            // Wszystkie klasy wybrane - brak filtra
            if(viewModel.mainWindowViewModel.ElementMasterClass_ComboboxSelectedItem == null)
            {
                new MyMaterialMessageBox("Nie wybrano filtra klasy :(", MyMaterialMessageBox.MessageBoxType.Error, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                return;
            }
            else
            {
                if (viewModel.mainWindowViewModel.ElementMasterClass_ComboboxSelectedItem.Equals(ViewModels.MainWindowViewModel.ClassComobox_All))
                {
                    ElementClassesList = null;
                }
                // Wybrano klasę master ale nie wybrano pod klasy
                else if (viewModel.mainWindowViewModel.ElementSubClass_ComboboxSelectedItem == null)
                {
                    new MyMaterialMessageBox("Nie wybrano filtra podklasy :(", MyMaterialMessageBox.MessageBoxType.Error, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                    return;
                }
                else
                {
                    if (viewModel.mainWindowViewModel.ElementSubClass_ComboboxSelectedItem.Equals(ViewModels.MainWindowViewModel.ClassComobox_All))
                    {
                        ElementClassesList = new List<string>();
                        ElementClassesList.Add(viewModel.mainWindowViewModel.ElementMasterClass_ComboboxSelectedItem);
                        foreach (ElementClassTemplate elem in Data.ElementsPool.SubClasses)
                        {
                            if (elem.MasterClassTemplate.Equals(viewModel.mainWindowViewModel.ElementMasterClass_ComboboxSelectedItem))
                            {
                                ElementClassesList.Add(elem.Name);
                            }
                        }
                    }
                    // Wybrano konkretną klasę
                    else
                    {
                        ElementClassesList = new List<string>(new string[] { viewModel.mainWindowViewModel.ElementSubClass_ComboboxSelectedItem });
                    }
                }
            }

            // Kontrola filtra parametru
            if (! viewModel.mainWindowViewModel.ValueFilter_ComboboxSelectedItem.Equals("brak"))
            {
                ValueFilter = viewModel.mainWindowViewModel.ValueFilter_ComboboxSelectedItem;
                ValueFilterContent = viewModel.mainWindowViewModel.ValueFilterValue;
            }
            else
            {
                ValueFilter = null;
                ValueFilterContent = " ";
            }

            // Kontrola filtru ilości
            if (!viewModel.mainWindowViewModel.CountFilter_ComboboxSelectedItem.Equals("brak"))
            {
                CountFilter = viewModel.mainWindowViewModel.CountFilter_ComboboxSelectedItem;
                // Sprawdzenie poprawności numerycznej podanego parametru
                if(! int.TryParse(viewModel.mainWindowViewModel.CoutFilterValue, out CountFilterContent))
                {
                    new MyMaterialMessageBox("Podałeś niepoprawną wartość w filtrze ilości.\nPowinna to być liczba typu int.", MyMaterialMessageBox.MessageBoxType.Error, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                    return;
                }
            }
            else
            {
                CountFilter = null;
                CountFilterContent = 0;
            }

            // Uruchamianie filtracji i generowania drzewa elementów
            viewModel.mainWindowViewModel.GenerateElementTree(ElementClassesList, ValueFilter, CountFilter, ValueFilterContent, CountFilterContent);
        }

        /// <summary>
        /// Funkcja przycisku wyłączającego filtry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAll_ButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.mainWindowViewModel.GenerateElementTree();
        }

        /// <summary>
        /// Funkcja przycisku odpowiadający za usuwanie elementu z listy elementów.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveElement_ButtonClick(object sender, RoutedEventArgs e)
        {
            ElementTreeObject selectedTreeElement = ElementsTree.SelectedItem as ElementTreeObject;

            // Upewnienie się czy usunąć element
            MyMaterialMessageBox message = new MyMaterialMessageBox("Czy na pewno chcesz usunąć element: " + selectedTreeElement.ElementClass + ": " + selectedTreeElement.MainParamShowcase,
                MyMaterialMessageBox.MessageBoxType.Confirmation, MyMaterialMessageBox.MessageBoxButtons.YesNo);
            if (message.ShowDialog() == false)
            {
                return;
            }

            // Znajdywanie elementu
            ElementTemplate selectedElement = new ElementTemplate("", "", 0, new string[] { }, new string[] { }, ""); // Inicjalizacja tymczasowa.
            foreach(ElementTemplate elem in Data.ElementsPool.Elements)
            {
                if( selectedTreeElement.ElementClass.Equals(elem.ClassTemplate) &&
                    selectedTreeElement.MainParamValue.Equals(elem.MainParametrValue))
                {
                    selectedElement = elem;
                }
            }
            // Usuwanie obiektu z listy
            Data.ElementsPool.Elements.Remove(selectedElement);
            // Ustawianie flagi edycji
            Data.ElementsPool.IsDatabaseEdited = true;
            // Odświeżanie elementów zależnych od listy elementów
            viewModel.ElementEditUpdate();
        }

        /// <summary>
        /// Funkcja przycisku służącego do zmieniania ilości elementów.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeElementCount_ButtonClick(object sender, RoutedEventArgs e)
        {
            ElementTreeObject selectedTreeElement = ElementsTree.SelectedItem as ElementTreeObject;
            NewElementCountDialogWindow dialogWindow = new NewElementCountDialogWindow(selectedTreeElement.ElementClass + ": " + selectedTreeElement.MainParamShowcase);
            if(dialogWindow.ShowDialog() == true)
            {
                // Znajdywanie indeksu elementu
                int Index = 0;
                for(int i=0; i<Data.ElementsPool.Elements.Count; i++)
                {
                    if (selectedTreeElement.ElementClass.Equals(Data.ElementsPool.Elements[i].ClassTemplate) &&
                        selectedTreeElement.MainParamValue.Equals(Data.ElementsPool.Elements[i].MainParametrValue))
                    {
                        Index = i;
                        break;
                    }
                }
                // Zmiana liczby elementów
                Data.ElementsPool.Elements[Index].Count = dialogWindow.NewCount;
                // Zmiana flagi edycji
                Data.ElementsPool.IsDatabaseEdited = true;
                // Odświeżenie wyświetlanego tekstu
                DisplayElementInfo();
            }
        }

        /// <summary>
        /// Nadpisanie funkcji zamykającej okno.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Sprawdzenie czy zapisano zmiany w bazie danych
            if(Data.ElementsPool.IsDatabaseEdited)
            {
                MyMaterialMessageBox message = new MyMaterialMessageBox("Nie zapisałeś zmian wprowadzonych w bazie. Zapisać?", MyMaterialMessageBox.MessageBoxType.Confirmation, MyMaterialMessageBox.MessageBoxButtons.YesNo);
                if(message.ShowDialog() == true)
                {
                    Data.ElementsPool.SavePollToFile();
                }
            }
            // Domyślna funkcja zamykająca okno
            base.OnClosing(e);
        }
    }
}
