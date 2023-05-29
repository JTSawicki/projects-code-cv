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

using MaterialDesignThemes;

namespace BazaDanychElementow
{
    /// <summary>
    /// Logika interakcji dla klasy MyMaterialMessageBox.xaml
    /// </summary>
    public partial class MyMaterialMessageBox : Window
    {
        public MyMaterialMessageBox(string message, MessageBoxType type, MessageBoxButtons buttons, string messageBoxTitle = "default")
        {
            InitializeComponent();
            messageText.Text = message;
            this.Title = messageBoxTitle;

            // Ustawianie domyślnej wartości na wypadek zamknięcia przyciskiem paska windows
            // this.DialogResult = false;

            // Ustawianie widocznych przycisków
            if (buttons == MessageBoxButtons.Ok)
            {
                YesButton.Visibility = Visibility.Collapsed;
                NoButton.Visibility = Visibility.Collapsed;
            }
            if (buttons == MessageBoxButtons.YesNo)
            {
                OkButton.Visibility = Visibility.Collapsed;
            }

            // Ustawianie tytułu i ikony okna
            if (type == MessageBoxType.Info)
            {
                if (messageBoxTitle == "default") this.Title = "Info";
                this.Icon = new BitmapImage(new Uri("pack://siteOfOrigin:,,,/Icons/information.ico"));
            }
            else if (type == MessageBoxType.Confirmation)
            {
                if (messageBoxTitle == "default") this.Title = "Confirmation";
                this.Icon = new BitmapImage(new Uri("pack://siteOfOrigin:,,,/Icons/confirmation.ico"));
            }
            else if (type == MessageBoxType.Warning)
            {
                if (messageBoxTitle == "default") this.Title = "Warning";
                this.Icon = new BitmapImage(new Uri("pack://siteOfOrigin:,,,/Icons/warning.ico"));
            }
            else if (type == MessageBoxType.Error)
            {
                if (messageBoxTitle == "default") this.Title = "Error";
                this.Icon = new BitmapImage(new Uri("pack://siteOfOrigin:,,,/Icons/error.ico"));
            }
        }

        // Wyliczenie służące do określania widocznych przycisków
        public enum MessageBoxButtons
        {
            YesNo,
            Ok
        }
        // Wyliczenie służące do określania typu powiadomienia
        public enum MessageBoxType
        {
            Info,
            Confirmation,
            Warning,
            Error
        }

        // Funkcje przycisków okna dialogowego
        private void yesButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        private void noButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void okButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
