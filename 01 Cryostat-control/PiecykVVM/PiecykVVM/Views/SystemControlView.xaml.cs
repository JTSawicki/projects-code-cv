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
using PiecykVVM.ViewModels;

namespace PiecykVVM.Views
{
    /// <summary>
    /// Logika interakcji dla klasy SystemControlView.xaml
    /// </summary>
    public partial class SystemControlView : UserControl
    {
        private SystemControlViewModel ViewModel;
        public SystemControlView()
        {
            InitializeComponent();
            ViewModel = new SystemControlViewModel(this.Dispatcher);
            this.DataContext = ViewModel;
            this.Loaded += UserControlLoaded;
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.GetAutoPidPool = PidMenu.GetPidPool;
            ViewModel.SetAutoPidPool = PidMenu.SetPidPool;
        }
    }
}
