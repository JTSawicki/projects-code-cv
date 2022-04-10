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
    /// Logika interakcji dla klasy LicenceScreen.xaml
    /// </summary>
    public partial class LicenceScreen : Window
    {
        public LicenceScreen()
        {
            InitializeComponent();
            string FileText = System.IO.File.ReadAllText("Data\\Licencja.txt");
            LicenceTextBlock.Text = FileText;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
