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

namespace BazaDanychElementow.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy ParameterInputControl.xaml
    /// </summary>
    public partial class ParameterInputControl : UserControl
    {
        public ParameterInputControlViewModel viewModel = new ParameterInputControlViewModel();
        public ParameterInputControl()
        {
            InitializeComponent();
            // Inicjowanie kontekstu danych dla wiązań danych
            this.DataContext = viewModel;
        }
    }
}
