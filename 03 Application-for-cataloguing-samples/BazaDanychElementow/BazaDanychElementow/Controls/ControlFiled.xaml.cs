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
    /// Logika interakcji dla klasy ControlFiled.xaml
    /// </summary>
    public partial class ControlFiled : UserControl
    {
        public ControlFiledViewModel viewModel = new ControlFiledViewModel();
        public ControlFiled()
        {
            InitializeComponent();
            // Inicjowanie kontekstu danych dla wiązań danych
            this.DataContext = viewModel;
        }
    }
}
