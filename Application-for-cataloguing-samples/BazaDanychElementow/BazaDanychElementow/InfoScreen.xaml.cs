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
    /// Logika interakcji dla klasy InfoScreen.xaml
    /// </summary>
    public partial class InfoScreen : Window
    {
        public InfoScreen()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenLicence(object sender, RoutedEventArgs e)
        {
            LicenceScreen licenceScreen = new LicenceScreen();
            licenceScreen.Show();
        }
    }
}
