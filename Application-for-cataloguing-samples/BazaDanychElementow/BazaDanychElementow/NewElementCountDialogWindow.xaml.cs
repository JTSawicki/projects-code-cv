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
    /// Logika interakcji dla klasy NewElementCountDialogWindow.xaml
    /// </summary>
    public partial class NewElementCountDialogWindow : Window
    {
        public int NewCount;
        public NewElementCountDialogWindow(string ElementInfo)
        {
            InitializeComponent();
            ElementInfoTextBlock.Text = ElementInfo;
        }

        /// <summary>
        /// Sprawdzanie poprawności wprowadzonej wartości i jej zwracanie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExitAndReturnValue_ButtonClick(object sender, RoutedEventArgs e)
        {
            int newCount;
            // Sprawdzenie poprawności wprowadzonego parametru
            if(! int.TryParse(CountInputTextBox.Text, out newCount))
            {
                new MyMaterialMessageBox("Podałeś niepoprawną ilość elementów.\nPowinna to być liczba typu int.", MyMaterialMessageBox.MessageBoxType.Error, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                return;
            }
            else
            {
                // Zwracanie wartości i zamykanie okna
                this.DialogResult = true;
                NewCount = newCount;
                this.Close();
            }
        }
    }
}
