using System;
using System.Windows;
using Serilog;

namespace PiecykVVM.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        public ConnectWindow()
        {
            this.DataContext = new ViewModels.ConnectWindowViewModel( () => this.Close() );
            InitializeComponent();
            Log.Information("Start of ConnectWindow");
        }
    }
}
