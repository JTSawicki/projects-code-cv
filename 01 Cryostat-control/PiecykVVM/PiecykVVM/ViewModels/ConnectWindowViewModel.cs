using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabControlsWPF;
using Serilog;
using LabServices.Lumel;
using LabServices.MFIA;
using System.IO.Ports;
using PiecykVVM.Windows;
using System.Threading.Tasks;

namespace PiecykVVM.ViewModels
{
    /// <summary>
    /// ViewModel dla okna ConnectWindow.
    /// Jest to jedyny model, nie następuje przełączanie widoków.
    /// </summary>
    internal sealed partial class ConnectWindowViewModel : ObservableObject
    {
        public ConnectWindowViewModel(Action closeWindow)
        {
            CloseWindow = closeWindow;

            comList = new List<string> { };
            selectedCom = "";

            _defaultBaudrate = Baudrate.Baudrate_57600;
            defaultBaudrateInfo = "Default=" + ((int)_defaultBaudrate).ToString();
            baudrateList = Enum.GetValues(typeof(Baudrate))
                .Cast<int>()
                .Select(i => i.ToString())
                .ToList();
            selectedBaudrate = ((int)_defaultBaudrate).ToString();
            isCustomBaudrate = false;

            _defaultAddres = 1;
            defaultAddresInfo = "Default=" + _defaultAddres.ToString();
            List<string> tmpAddresList = new List<string>();
            for (int i = 1; i <= 256; i++) tmpAddresList.Add(i.ToString());
            addresList = tmpAddresList;
            selectedAddres = _defaultAddres.ToString();
            isCustomAddres = false;

            _defaultInformationUnit = InformationUnit.RTU_8N2;
            defaultInformationUnitInfo = "Default=" + _defaultInformationUnit.ToString();
            informationUnitList = Enum.GetNames(typeof(InformationUnit)).ToList();
            selectedInformationUnit = _defaultInformationUnit.ToString();
            isCustomInformationUnit = false;

            // Inicjowanie komend
            SearchComCommand = new RelayCommand(SearchComs);
            OpenSettingsCommand = new RelayCommand(OpenSettingsWindow);
            OpenLicenceCommand = new RelayCommand(OpenLicenceWindow);
            ConnectCommand = new RelayCommand(ConnectAndStartMainWindow, CanConnect);

            IsLumelConnected = true;
            IsMFIAConnected = true;
            IsArduinoConnected = false;

            this.PropertyChanged += ConnectCommandLockDedection;
        }

        public Action CloseWindow { get; set; }

        // Parametry połączenia z Lumel
        [ObservableProperty]
        List<string> comList;
        [ObservableProperty]
        string selectedCom;

        private Baudrate _defaultBaudrate;
        [ObservableProperty]
        string defaultBaudrateInfo;
        [ObservableProperty]
        List<string> baudrateList;
        [ObservableProperty]
        string selectedBaudrate;
        [ObservableProperty]
        bool isCustomBaudrate;

        private byte _defaultAddres;
        [ObservableProperty]
        string defaultAddresInfo;
        [ObservableProperty]
        List<string> addresList;
        [ObservableProperty]
        string selectedAddres;
        [ObservableProperty]
        bool isCustomAddres;

        private InformationUnit _defaultInformationUnit;
        [ObservableProperty]
        string defaultInformationUnitInfo;
        [ObservableProperty]
        List<string> informationUnitList;
        [ObservableProperty]
        string selectedInformationUnit;
        [ObservableProperty]
        bool isCustomInformationUnit;

        [ObservableProperty]
        bool isLumelConnected;
        [ObservableProperty]
        bool isMFIAConnected;
        [ObservableProperty]
        bool isArduinoConnected;

        public ICommand SearchComCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand OpenLicenceCommand { get; }

        /// <summary>
        /// Funkcja ustawiająca dostępne porty COM
        /// </summary>
        private void SearchComs()
        {
            ComList = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
            if(ComList.Count == 1)
            {
                SelectedCom = ComList[0];
            }
            IsLumelConnected = true;
        }

        /// <summary>
        /// Funkcja otwierająca okno ustawień
        /// </summary>
        private void OpenSettingsWindow()
        {
            MaterialMessageBox.NewFastMessage(MaterialMessageFastType.NotImplementedWarning, "Settings Window");
        }

        /// <summary>
        /// Funkcja otwierająca okno licencji
        /// </summary>
        private void OpenLicenceWindow()
        {
            LicenceWindow window = new LicenceWindow();
            window.Show();
        }

        /// <summary>
        /// Funkcja otwierająca okno główne i wywołująca nawiązanie połączenia z sterownikami sprzętu
        /// </summary>
        private void ConnectAndStartMainWindow()
        {
            Log.Information("ConnectWindowViewModel.ConnectAndStartMainWindow-Establishing a connection with the hardware");

            // Nawiązywanie połączeń z urządzeniami
            if (IsLumelConnected) ConnectToLumelController();
            if (IsMFIAConnected) ConnectToMFIAController();
            if (IsArduinoConnected) ConnectToArduinoController();

            // Uruchamianie okna
            MainWindow window = new MainWindow();
            window.Show();
            CloseWindow();
        }

        /// <summary>
        /// Funkcja sprawdzająca czy spełniono wymagania nawiązania połączenia ze sprzętem
        /// </summary>
        private bool CanConnect()
        {
            if (IsLumelConnected == true && (
                    string.IsNullOrEmpty(SelectedCom) ||
                    string.IsNullOrEmpty(SelectedAddres) ||
                    string.IsNullOrEmpty(SelectedBaudrate)
                ))
                return false;
            return true;
        }

        /// <summary>
        /// Funkcja nadzorująca blokadę przycisku połączenia ze sprzętem
        /// </summary>
        private void ConnectCommandLockDedection(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsLumelConnected) ||
                e.PropertyName == nameof(SelectedCom) ||
                e.PropertyName == nameof(SelectedAddres) ||
                e.PropertyName == nameof(SelectedBaudrate)
                )
            {
                (ConnectCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Funkcja nawiązująca połączenie z sterownikiem Lumel
        /// </summary>
        private void ConnectToLumelController()
        {
            // Wybieranie wartości parametrów
            Baudrate baudrate;
            if (IsCustomBaudrate)
                baudrate = (Baudrate)int.Parse(SelectedBaudrate);
            else
                baudrate = _defaultBaudrate;

            byte addres;
            if (IsCustomAddres)
                addres = byte.Parse(SelectedAddres);
            else
                addres = _defaultAddres;

            object? informationUnit_obj;
            int dataBits; Parity parity; StopBits stopBits;
            if (IsCustomInformationUnit)
                Enum.TryParse(typeof(InformationUnit), SelectedInformationUnit, out informationUnit_obj);
            else
                informationUnit_obj = _defaultInformationUnit;
            if(informationUnit_obj == null || informationUnit_obj is not InformationUnit)
            {
                Log.Error("ConnectWindowViewModel.ConnectToLumelController-Problem with parsing InformationUnit");
                throw new ArgumentException($"Problem with parsing InformationUnit");
            }
            InformationUnit informationUnit = (InformationUnit)informationUnit_obj;
            if(informationUnit == InformationUnit.RTU_8N2)
            {
                dataBits = 8;
                parity = Parity.None;
                stopBits = StopBits.Two;
            }
            else if(informationUnit == InformationUnit.RTU_8E1)
            {
                dataBits = 8;
                parity = Parity.Even;
                stopBits = StopBits.One;
            }
            else if(informationUnit == InformationUnit.RTU_8O1)
            {
                dataBits = 8;
                parity = Parity.Odd;
                stopBits = StopBits.One;
            }
            else if(informationUnit == InformationUnit.RTU_8N1)
            {
                dataBits = 8;
                parity = Parity.None;
                stopBits = StopBits.One;
            }
            else
            {
                Log.Error("ConnectWindowViewModel.ConnectToLumelController-Unsupported InformationUnit value");
                throw new Exception("Unsupported InformationUnit value");
            }

            // Uruchamianie kontrolera
            LumelControllerInitData initData = new LumelControllerInitData
            {
                Com = SelectedCom,
                Budrate = baudrate,
                Addres = addres,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits
            };
            LumelController.StartController(initData);
        }

        /// <summary>
        /// Funkcja nawiązująca połączenie z mostkiem pomiarowym impedancji MFIA
        /// </summary>
        private void ConnectToMFIAController()
        {
            MFIAController.StartController();
        }

        /// <summary>
        /// Funkcja nawiązująca połączenie z czujnikiem arduino
        /// </summary>
        private void ConnectToArduinoController()
        {
            MaterialMessageBox.NewFastMessage(MaterialMessageFastType.NotImplementedWarning, "ArduinoController");
        }
    }
}
