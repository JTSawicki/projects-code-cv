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

namespace Piecyk.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy CommandParameterInputFiled.xaml
    /// </summary>
    public partial class CommandParameterInputFiled : UserControl
    {
        public CommandParameterInputFiledViewModel viewModel = new CommandParameterInputFiledViewModel();
        public CommandParameterInputFiled()
        {
            InitializeComponent();
            // Inicjowanie kontekstu danych dla wiązań danych
            this.DataContext = viewModel;
        }
    }
}
