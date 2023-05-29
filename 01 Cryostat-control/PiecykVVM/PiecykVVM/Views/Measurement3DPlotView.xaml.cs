﻿using System;
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

namespace PiecykVVM.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Measurement3DPlotView.xaml
    /// </summary>
    public partial class Measurement3DPlotView : UserControl
    {
        public Measurement3DPlotView()
        {
            this.DataContext = new ViewModels.Measurement3DPlotViewModel(this.Dispatcher);
            InitializeComponent();
        }

        private void ResetCamera_ButtonClick(object sender, RoutedEventArgs e)
        {
            Plot3DControl.ResetCamera();
        }
    }
}
