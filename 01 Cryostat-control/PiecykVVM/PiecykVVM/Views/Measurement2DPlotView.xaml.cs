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
    /// Logika interakcji dla klasy Measurement2DPlotView.xaml
    /// </summary>
    public partial class Measurement2DPlotView : UserControl
    {
        public Measurement2DPlotView()
        {
            InitializeComponent();
            this.DataContext = new Measurement2DPlotViewModel(this.Dispatcher);
        }
    }
}
