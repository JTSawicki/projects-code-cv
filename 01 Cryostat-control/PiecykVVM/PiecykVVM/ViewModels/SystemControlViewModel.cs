using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using LabControlsWPF;
using LabControlsWPF.TextEditor;
using LabControlsWPF.Exceptions;
using LabControlsWPF.AutoPid;
using LabControlsWPF.Plot2D;
using LabServices.Lumel;
using LabServices.MFIA;
using PiecykM.SaveProvider;
using PiecykM.CodeProcesor;
using PiecykM.DataConverters;
using PiecykM.Exceptions;
using Microsoft.Win32;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PiecykVVM.ViewModels
{
    internal sealed partial class SystemControlViewModel : ObservableObject
    {
        public SystemControlViewModel(Dispatcher viewDispatcher)
        {
            ViewDispatcher = viewDispatcher;

            // Inicjowanie wykresu
            plotModel = new MultiSeriesPlotModel(
                title: "Przewidywany przebieg",
                xLabel: "Czas",
                yLabel: "Temperatura [°C]",
                series: new List<SeriesInitData>
                {
                    new SeriesInitData(
                        (int)PlotSeriesId.EstimatedTemperature,
                        "Przebieg temperatury",
                        OxyPlot.OxyColors.Orange,
                        SeriesType.Line),
                    new SeriesInitData(
                        (int)PlotSeriesId.Measurements,
                        "Pomiary",
                        OxyPlot.OxyColors.Red,
                        SeriesType.Scatter)
                });

            // Subskrybowanie danych
            CodeInterpreter.NewCurrentlyInterpretedLineEvent += NewCurrentlyInterpretedLineEvent;

            // Inicjowanie komend
            LoadCodeCommand = new RelayCommand(LoadCode);
            SaveCodeCommand = new RelayCommand(SaveCode);
            LoadPidCommand = new RelayCommand(LoadPid);
            SavePidCommand = new RelayCommand(SavePid);
            LoadMeasurementDataFromFileCommand = new RelayCommand(LoadMeasurementDataFromFile);
            //CheckCodeCommand = new AsyncRelayCommand(CheckCode, AsyncRelayCommandOptions.None);
            CheckCodeCommand = new RelayCommand(CheckCode);
            RunCodeCommand = new RelayCommand(RunCode);
            HideErrorInfoBlockCommand = new RelayCommand(HideErrorInfoBlock);
            SelectSaveFileCommand = new RelayCommand(SelectSaveFile);
        }

        private Dispatcher ViewDispatcher;

        /// <summary>Zmienna blokująca interfejs podczas wykonywania kodu</summary>
        [ObservableProperty]
        private bool isEditInterfaceEnabled = true;

        [ObservableProperty]
        private MultiSeriesPlotModel plotModel;

        [ObservableProperty]
        private string programCode = "# Grupy komend: func, lumel, mfia" + Environment.NewLine;
        /// <summary>
        /// Obecnie wykonywana linia kodu.
        /// Linia -1 oznacza brak wykonywania lini kodu
        /// </summary>
        [ObservableProperty]
        private int currentlyExecutedLine = -1;
        [ObservableProperty]
        private HintPool editorHintPool = GenerateHintPool();

        [ObservableProperty]
        private bool isAutoPid = false;
        public Action<AutoPidPool>? SetAutoPidPool;
        public Func<AutoPidPool>? GetAutoPidPool;

        [ObservableProperty]
        private string errorText = "";
        [ObservableProperty]
        private Brush errorTextBrush = new SolidColorBrush(Colors.Red);
        [ObservableProperty]
        private Visibility errorTextVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private List<string> connectionTypeList = new List<string> { "4 przewody", "2 przewody" };
        private Dictionary<string, ConnectionType> _connectionTypeMap = new Dictionary<string, ConnectionType>
        {
            ["4 przewody"] = ConnectionType.FourWireTerminal,
            ["2 przewody"] = ConnectionType.TwoWireTerminal
        };
        [ObservableProperty]
        private string selectedConnectionType = "4 przewody";
        [ObservableProperty]
        private List<string> measurementPrecisionList = new List<string> { "Niska", "Średnia", "Wysoka" };
        private Dictionary<string, MeasurementPrecision> _measurementPrecisionMap = new Dictionary<string, MeasurementPrecision>
        {
            ["Niska"] = MeasurementPrecision.Low,
            ["Średnia"] = MeasurementPrecision.High,
            ["Wysoka"] = MeasurementPrecision.VeryHigh
        };
        [ObservableProperty]
        private string selectedMeasurementPrecision = "Średnia";
        [ObservableProperty]
        private string selectedMeanFromSamples = "5";
        [ObservableProperty]
        private List<string> frequencySegmentationList = new List<string> { "Log10", "Lin"};
        private Dictionary<string, FrequencySegmentation> _frequencySegmentationMap = new Dictionary<string, FrequencySegmentation>
        {
            ["Log10"] = FrequencySegmentation.Logarytmic,
            ["Lin"] = FrequencySegmentation.Linear
        };
        [ObservableProperty]
        private string selectedFrequencySegmentation = "Lin";
        [ObservableProperty]
        private string selectedSampleCount = "100";
        [ObservableProperty]
        private string selectedFrequencyStart = "1.00E+02";
        [ObservableProperty]
        private string selectedFrequencyStop = "1.00E+06";
        [ObservableProperty]
        private List<string> voltageAmplitudeControlTypeList = new List<string> { "Automatyczna", "Ręczna" };
        private Dictionary<string, VoltageAmplitudeControlType> _voltageAmplitudeControlMap = new Dictionary<string, VoltageAmplitudeControlType>
        {
            ["Automatyczna"] = VoltageAmplitudeControlType.Auto,
            ["Ręczna"] = VoltageAmplitudeControlType.Manual
        };
        [ObservableProperty]
        private string selectedVoltageAmplitudeControlType = "Automatyczna";
        [ObservableProperty]
        private string selectedVoltageAmplitude = "0.001";
        [ObservableProperty]
        private List<string> biasVontageStateList = new List<string> { "Wyłączony", "Wewnętrzny" };
        private Dictionary<string, BiasVontageState> _biasVontageStateMap = new Dictionary<string, BiasVontageState>
        {
            ["Wyłączony"] = BiasVontageState.OFF,
            ["Wewnętrzny"] = BiasVontageState.ON,
        };
        [ObservableProperty]
        private string selectedBiasVontageState = "Wyłączony";
        [ObservableProperty]
        private string selectedBiasVoltage = "0.000";

        [ObservableProperty]
        private string selectedControlerPeriod = "200";
        [ObservableProperty]
        private string selectedControlerReadMultipler = "5";
        [ObservableProperty]
        private string selectedOutputFile = "";
        [ObservableProperty]
        private string selectedOutputFileInfo = "Nie wybrano";
        [ObservableProperty]
        private Brush selectedOutputFileInfoBrush = new SolidColorBrush(Colors.Red);

        public RelayCommand LoadCodeCommand { get; }
        public RelayCommand SaveCodeCommand { get; }
        public RelayCommand LoadPidCommand { get; }
        public RelayCommand SavePidCommand { get; }
        public RelayCommand CheckCodeCommand { get; }
        public RelayCommand RunCodeCommand { get; }
        public RelayCommand LoadMeasurementDataFromFileCommand { get; }
        public RelayCommand SelectSaveFileCommand { get; }
        public RelayCommand HideErrorInfoBlockCommand { get; }

        /// <summary>
        /// Funkcja wczytuje z pliku kod i ustawienia jego wywołania
        /// </summary>
        private void LoadCode()
        {
            // Wybór pliku zapisu
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Plik zapisu kodu programu",
                Filter = "Piecyk save (*.PCode)|*.PCode|All files (*.*)|*.*",
                InitialDirectory = SaveManager.AppFolder_UserData,
                ValidateNames = true
            };
            // Deserializacja jeżeli wybrano plik
            if(openFileDialog.ShowDialog() == true)
            {
                string jsonSaveString = File.ReadAllText(openFileDialog.FileName);
                CodeSaveObject? saveObject =
                    JsonSerializer.Deserialize<CodeSaveObject>(jsonSaveString);
                // Sprawdzenei poprawności deserializacji
                if(saveObject == null)
                {
                    MaterialMessageBox.NewFastMessage(MaterialMessageFastType.BadUserInputWarning, "Niepoprawny lub uszkodzony plik zapisu.");
                    return;
                }
                // Ustawianie parametrów
                this.ProgramCode = saveObject.ProgramCode;
                this.SelectedConnectionType = saveObject.ConnectionType;
                this.SelectedMeasurementPrecision = saveObject.MeasurementPrecision;
                this.SelectedMeanFromSamples = saveObject.MeanFromSamples;
                this.SelectedFrequencySegmentation = saveObject.FrequencySegmentation;
                this.SelectedSampleCount = saveObject.SampleCount;
                this.SelectedFrequencyStart = saveObject.FrequencyStart;
                this.SelectedFrequencyStop = saveObject.FrequencyStop;
                this.SelectedVoltageAmplitudeControlType = saveObject.VoltageAmplitudeControlType;
                this.SelectedVoltageAmplitude = saveObject.VoltageAmplitude;
                this.SelectedBiasVontageState = saveObject.BiasVontageState;
                this.SelectedBiasVoltage = saveObject.BiasVoltage;
                this.SelectedControlerPeriod = saveObject.ControlerPeriod;
                this.SelectedControlerReadMultipler = saveObject.ControlerReadMultipler;
                this.IsAutoPid = saveObject.IsAutoPidActive;
            }
        }

        /// <summary>
        /// Funkcja zapisuje do pliku kod i ustawienia jego wywołania
        /// </summary>
        private void SaveCode()
        {
            string jsonSaveString = GetCodeSave(true);
            // Okno wyboru pliku
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Plik zapisu kodu programu",
                FileName = SaveManager.GetExampleSaveFileName("PiecykProgram", "PCode"),
                AddExtension = true,
                Filter = "Piecyk save (*.PCode)|*.PCode|All files (*.*)|*.*",
                InitialDirectory = SaveManager.AppFolder_UserData,
                ValidateNames = true
            };
            // Zapis danych jeżeli wybrano plik
            if(saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, jsonSaveString);
            }
        }

        /// <summary>
        /// Funkcja zwraca string zapisu programu
        /// </summary>
        /// <param name="writeIntended">Czy plik ma nyć czytelny dla człowieka(obecne wcięcia, spacje itp.)</param>
        /// <returns>String zapisu</returns>
        private string GetCodeSave(bool writeIntended)
        {
            // Tworzenie obiektu do serializacji
            CodeSaveObject saveObject = new CodeSaveObject(
                programCode: this.ProgramCode,
                connectionType: this.SelectedConnectionType,
                measurementPrecision: this.SelectedMeasurementPrecision,
                meanFromSamples: this.SelectedMeanFromSamples,
                frequencySegmentation: this.SelectedFrequencySegmentation,
                sampleCount: this.SelectedSampleCount,
                frequencyStart: this.SelectedFrequencyStart,
                frequencyStop: this.SelectedFrequencyStop,
                voltageAmplitudeControlType: this.SelectedVoltageAmplitudeControlType,
                voltageAmplitude: this.SelectedVoltageAmplitude,
                biasVontageState: this.SelectedBiasVontageState,
                biasVoltage: this.SelectedBiasVoltage,
                controlerPeriod: this.SelectedControlerPeriod,
                controlerReadMultipler: this.SelectedControlerReadMultipler,
                isAutoPidActive: this.IsAutoPid
                );
            // Serializacja
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = writeIntended
            };
            return JsonSerializer.Serialize(saveObject, serializerOptions);
        }

        /// <summary>
        /// Funkcja wczytuje z pliku ustawienia auto pid
        /// </summary>
        private void LoadPid()
        {
            if (SetAutoPidPool == null)
                return;
            // Wybór pliku zapisu
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Plik zapisu parametrów auto pid",
                Filter = "Piecyk save (*.PPid)|*.PPid|All files (*.*)|*.*",
                InitialDirectory = SaveManager.AppFolder_UserData,
                ValidateNames = true
            };
            // Deserializacja jeżeli wybrano plik
            if (openFileDialog.ShowDialog() == true)
            {
                string jsonSaveString = File.ReadAllText(openFileDialog.FileName);
                AutoPidPool? saveObject =
                    JsonSerializer.Deserialize<AutoPidPool>(jsonSaveString);
                // Sprawdzenei poprawności deserializacji
                if (saveObject == null)
                {
                    MaterialMessageBox.NewFastMessage(MaterialMessageFastType.BadUserInputWarning, "Niepoprawny lub uszkodzony plik zapisu");
                    return;
                }
                SetAutoPidPool(saveObject);
            }
        }

        /// <summary>
        /// Funkcja zapisuje do pliku ustawienia auto pid
        /// </summary>
        private void SavePid()
        {
            if (GetAutoPidPool == null)
                return;
            string jsonSaveString = GetAutoPidSave(true);
            // Okno wyboru pliku
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Plik zapisu parametrów auto pid",
                FileName = SaveManager.GetExampleSaveFileName("PiecykAutoPidParameters", "PPid"),
                AddExtension = true,
                Filter = "Piecyk save (*.PPid)|*.PPid|All files (*.*)|*.*",
                InitialDirectory = SaveManager.AppFolder_UserData,
                ValidateNames = true
            };
            // Zapis danych jeżeli wybrano plik
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, jsonSaveString);
            }
        }

        /// <summary>
        /// Funkcja zwraca string zapisu autopid
        /// </summary>
        /// <param name="writeIntended">Czy plik ma nyć czytelny dla człowieka(obecne wcięcia, spacje itp.)</param>
        /// <returns>String zapisu lub pusty jeżeli brak dostępu</returns>
        private string GetAutoPidSave(bool writeIntended)
        {
            if (GetAutoPidPool == null)
                return "";
            // Serializacja
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = writeIntended
            };
            return JsonSerializer.Serialize(GetAutoPidPool(), serializerOptions);
        }

        /// <summary>
        /// Funkcja sprawdza poprawność kodu parametrów i rysuje wykres estymacji przebiegu
        /// </summary>
        private void CheckCode()
        {
            InternalCheckProgram();
        }

        /// <summary>
        /// Funkcja wywołuje kod
        /// </summary>
        private void RunCode()
        {
            // Kontrola poprawności danych wejściowych
            bool isProgramGodToRun = InternalCheckProgram();
            if(!isProgramGodToRun)
            {
                MaterialMessageBox.NewFastMessage(MaterialMessageFastType.BadUserInputWarning, "Ze względu na niepoprawnie wprowadzone dane nie można uruchomić kodu 😢");
                return;
            }

            // Czyszczenie buforów pomiarów z poprzedniego programu
            if (MaterialMessageBox.NewFastMessage(MaterialMessageFastType.ConfirmActionInfo, "Uruchomienie programu.\nWyczyści to bufory pomiarów.", true) == true)
            {
                MFIAStore.ClearMeasurementPool();
            }
            else
                return;

            // Logowanie wywołania kodu
            Log.Information("SystemControlViewModel.RunCode-Executing code");
            Log.Information($"SystemControlViewModel.RunCode-Program: {GetCodeSave(false)}");
            if(IsAutoPid)
            {
                Log.Information($"SystemControlViewModel.RunCode-AutoPid ON: {GetAutoPidSave(false)}");
            }
            // Konstruowanie obiektu ustawień sweepera
            MFIASweeperInitData sweeperInitData = new MFIASweeperInitData(
                connectionType: _connectionTypeMap[SelectedConnectionType],
                measurementPrecision: _measurementPrecisionMap[SelectedMeasurementPrecision],
                meanFromSamples: long.Parse(SelectedMeanFromSamples),
                frequencySegmentation: _frequencySegmentationMap[SelectedFrequencySegmentation],
                sampleCount: long.Parse(SelectedSampleCount),
                frequencyStart: (double)NumericConverters.StringToNumber(SelectedFrequencyStart, ConvertableNumericTypes.Double)!,
                frequencyStop: (double)NumericConverters.StringToNumber(SelectedFrequencyStop, ConvertableNumericTypes.Double)!,
                voltageAmplitudeControlType: _voltageAmplitudeControlMap[SelectedVoltageAmplitudeControlType],
                voltageAmplitude: (double)NumericConverters.StringToNumber(SelectedVoltageAmplitude, ConvertableNumericTypes.Double)!,
                biasVontageState: _biasVontageStateMap[SelectedBiasVontageState],
                biasVoltage: (double)NumericConverters.StringToNumber(SelectedBiasVoltage, ConvertableNumericTypes.Double)!
                );
            MFIAController.PushCommand(
                MFIAControllerCommands.SetSweeperParameters,
                new List<object>
                {
                    sweeperInitData
                });

            // Wysyłanie ustawień do kontrolera sterownika lumel
            LumelController.PushCommand(
                LumelControllerCommands.SetLumelEnginePeriod,
                new List<object>
                {
                    NumericConverters.StringToNumber(SelectedControlerPeriod, ConvertableNumericTypes.Long)!
                });
            LumelController.PushCommand(
                LumelControllerCommands.SetLumelEngineReadMultipler,
                new List<object>
                {
                    NumericConverters.StringToNumber(SelectedControlerReadMultipler, ConvertableNumericTypes.Long)!
                });

            // Wysyłanie ustawień auto pid
            if(IsAutoPid)
            {
                AutoPidPool tmpPool = GetAutoPidPool!.Invoke();
                Tuple<ushort, ushort, ushort> tmpDefaultPid = tmpPool.GetDefaultPid();
                SortedList<double, Tuple<ushort, ushort, ushort>> tmpPidPool = tmpPool.GetPidPool();

                LumelPidValue defaultPid = new LumelPidValue(tmpDefaultPid.Item1, tmpDefaultPid.Item2, tmpDefaultPid.Item3);
                List<KeyValuePair<double, LumelPidValue>> pidPool = new List<KeyValuePair<double, LumelPidValue>>();
                foreach (double key in tmpPidPool.Keys)
                {
                    Tuple<ushort, ushort, ushort> tmpPid = tmpPidPool[key];
                    LumelPidValue pid = new LumelPidValue(tmpPid.Item1, tmpPid.Item2, tmpPid.Item3);
                    pidPool.Add(new KeyValuePair<double, LumelPidValue>(key, pid));
                }
                LumelAutoPidPool pool = new LumelAutoPidPool(pidPool, defaultPid);
                LumelController.PushCommand(
                    LumelControllerCommands.SetAutoPid,
                    new List<object> { pool }
                    );
            }
            else
            {
                // Wyłączanie autopid jeżeli jest włączony
                LumelController.PushCommand(LumelControllerCommands.SetAutoPid);
            }
            // Uruchamianie wątku interpretera programu
            List<CodeCommandContainer> code = CodePreprocessor.ProcessCode(ProgramCode);
            CodeInterpreter.Start(code);
        }

        /// <summary>
        /// Funkcja wczytuje zapisane dane pomiarowe z pliku do buforów.
        /// Pododuje to nadpisanie zawartości buforów.
        /// </summary>
        private void LoadMeasurementDataFromFile()
        {
            bool? loadConfirmation = MaterialMessageBox.NewFastMessage(
                MaterialMessageFastType.ConfirmActionInfo,
                "Wczytanie pomiarów z pliku.\nSpowoduje to nadpisanie obecnych buforów danych pomiarowych.",
                true);
            if (loadConfirmation!.Value == false)
                return;

            // Wybieranie pliku zapisu i wczytywanie danych
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Plik zapisu pomiarów",
                Filter = "Piecyk save (*.tsv)|*.tsv|Piecyk save (*.csv)|*.csv",
                InitialDirectory = SaveManager.AppFolder_UserData,
                ValidateNames = true
            };
            if(openFileDialog.ShowDialog() == true)
            {
                string saveFileName = openFileDialog.FileName;
                string saveFileContent = SaveManager.ReadFile(saveFileName);
                bool conversionSuccesFlag;
                if (saveFileName.Substring(saveFileName.Length - 3).Equals("tsv"))
                    conversionSuccesFlag = MFIAStore.LoadFromTSV(saveFileContent);
                else if (saveFileName.Substring(saveFileName.Length - 3).Equals("csv"))
                    conversionSuccesFlag = MFIAStore.LoadFromCSV(saveFileContent);
                else
                {
                    MaterialMessageBox.NewFastMessage(MaterialMessageFastType.BadUserInputWarning, "Wybrano niepoprawny plik zapisu.\nAkceptowane pliki: \"csv\", \"tsv\".");
                    return;
                }

                // Sprawdzenie czy dane zostały wczytane
                if (!conversionSuccesFlag)
                {
                    MaterialMessageBox.NewFastMessage(MaterialMessageFastType.InternalError, "Nie udało się wczytać pliku danych.");
                    return;
                }
                else
                {
                    MaterialMessageBox.NewFastMessage(MaterialMessageFastType.Information, "Wczytano dane.");
                    return;
                }
            }
        }

        /// <summary>
        /// Funkcja ukrywa blok informacji o błędach kodu
        /// </summary>
        private void HideErrorInfoBlock()
        {
            ErrorTextVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Funkcja uruchamia okno wyboru pliku zapisu pomiarów
        /// </summary>
        private void SelectSaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Plik zapisu pomiarów",
                FileName = SaveManager.GetExampleSaveFileName("Measurements", "tsv"),
                AddExtension = true,
                Filter = "Piecyk save (*.tsv)|*.tsv|Piecyk save (*.csv)|*.csv|All files (*.*)|*.*",
                InitialDirectory = SaveManager.AppFolder_UserData,
                ValidateNames = true
            };
            // Zapis wybranego pliku
            if (saveFileDialog.ShowDialog() == true)
            {
                SelectedOutputFileInfoBrush = new SolidColorBrush(Colors.Black);
                SelectedOutputFile = saveFileDialog.FileName;
                // Skracanie wyświetlanej nazwy
                if (saveFileDialog.FileName.Length > 60)
                {
                    SelectedOutputFileInfo =
                        saveFileDialog.FileName.Substring(0, 20) +
                        " ... " +
                        saveFileDialog.FileName.Substring(saveFileDialog.FileName.Length - 30);
                }
                else
                    SelectedOutputFileInfo = saveFileDialog.FileName;
            }
        }

        /// <summary>
        /// Funkcja sprawdza poprawność wprowadzonych przez użytkownika parametrów auto pid
        /// </summary>
        /// <returns>Zwraca pusty string jeżeli poprawne. Jeżeli nie zawiera błęy do wyświetlenia</returns>
        private string CheckAutoPidInputCorrectness()
        {
            StringBuilder outputMessage = new StringBuilder();

            if(!IsAutoPid)
            {
                // Test niepotrzebny
                goto AutoPidCheckBreak;
            }

            if (GetAutoPidPool != null)
            {
                // Testowanie poprawności danych wejściowych
                AutoPidPool? tmpPool = GetAutoPidPool.Invoke();
                if (tmpPool == null)
                {
                    outputMessage.AppendLine("Wadliwe wiązanie danych autopid.");
                    goto AutoPidCheckBreak;
                }
                Tuple<ushort, ushort, ushort> defaultPid;
                SortedList<double, Tuple<ushort, ushort, ushort>> pool;
                try
                {
                    defaultPid = tmpPool.GetDefaultPid();
                    pool = tmpPool.GetPidPool();
                }
                catch (BadUserInputException ex)
                {
                    outputMessage.AppendLine("Autopid | " + ex.Message);
                    goto AutoPidCheckBreak;
                }

                // Testowanie limitów danych
                if (defaultPid.Item1 > 9999 ||
                    defaultPid.Item2 > 9999 ||
                    defaultPid.Item3 > 9999 )
                    outputMessage.AppendLine("Autopid | Niepoprawne wartości domyślnej nastawy pid");
                if(pool.Count == 0)
                    outputMessage.AppendLine("Autopid | UWAGA! Wprowadzono zerową liczbę punktów autopid");
                foreach (double temperature in pool.Keys)
                    if (pool[temperature].Item1 > 9999 ||
                        pool[temperature].Item2 > 9999 ||
                        pool[temperature].Item3 > 9999 )
                        outputMessage.AppendLine($"Autopid | Niepoprawne wartości nastawy auto pid dla temperatury: {temperature}");

                // Testowanie zerowej części proporcjonalnej PID
                if(defaultPid.Item1 == 0)
                    outputMessage.AppendLine("Autopid | Domyślna nastawa auto pid ma zerową część proporcjonalną. System bezwładny dla tego zakresu");
                foreach (double temperature in pool.Keys)
                    if (pool[temperature].Item1 == 0)
                        outputMessage.AppendLine($"Autopid | Nastawa auto pid dla temperatury: {temperature} ma zerową część proporcjonalną. System bezwładny dla tego zakresu");
            }
            else
                outputMessage.AppendLine("Brak możliwości pobrania danych autopid.");
            // Zamykanie testu
            AutoPidCheckBreak:;
            return outputMessage.ToString();
        }

        /// <summary>
        /// Funckja sprawdza poprawność wprowadzonych parametrów wywołania
        /// </summary>
        /// <returns></returns>
        private string CheckParameters()
        {
            // Tworzenie wiadomości zwrotnej
            StringBuilder outputMessage = new StringBuilder();

            // Kontrola poprawności ustawień wywołania
            object? checker = NumericConverters.StringToNumber(SelectedFrequencyStart, ConvertableNumericTypes.Double);
            if (checker == null)
                outputMessage.AppendLine("Parametry | Niepoprawny literał parametru: \"Początkowa częstotliwość\"");

            checker = NumericConverters.StringToNumber(SelectedFrequencyStop, ConvertableNumericTypes.Double);
            if (checker == null)
                outputMessage.AppendLine("Parametry | Niepoprawny literał parametru: \"Końcowa częstotliwość\"");

            checker = NumericConverters.StringToNumber(SelectedMeanFromSamples, ConvertableNumericTypes.Long);
            if(checker != null)
            {
                if (!NumericConverters.CheckNumberLimits(checker, 1L, null, ConvertableNumericTypes.Long))
                    outputMessage.AppendLine("Parametry | Niepoprawna wartość parametru: \"Średnia z próbek\"");
            }
            else
                outputMessage.AppendLine("Parametry | Niepoprawny literał parametru: \"Średnia z próbek\"");

            checker = NumericConverters.StringToNumber(SelectedControlerPeriod, ConvertableNumericTypes.Long);
            if (checker != null)
            {

                if (!NumericConverters.CheckNumberLimits(checker, 100L, 60000L, ConvertableNumericTypes.Long))
                    outputMessage.AppendLine($"Parametry | Niepoprawna wartość parametru: \"Okres działania kontrolera\"");
            }
            else
                outputMessage.AppendLine("Parametry | Niepoprawny literał parametru: \"Okres działania kontrolera\"");

            checker = NumericConverters.StringToNumber(SelectedControlerReadMultipler, ConvertableNumericTypes.Long);
            if (checker != null)
            {

                if (!NumericConverters.CheckNumberLimits(checker, 1L, null, ConvertableNumericTypes.Long))
                    outputMessage.AppendLine("Parametry | Niepoprawna wartość parametru: \"Częstość pomiaru temperatury\"");
            }
            else
                outputMessage.AppendLine("Parametry | Niepoprawny literał parametru: \"Częstość pomiaru temperatury\"");

            checker = NumericConverters.StringToNumber(SelectedSampleCount, ConvertableNumericTypes.Long);
            if (checker != null)
            {

                if (!NumericConverters.CheckNumberLimits(checker, 1L, null, ConvertableNumericTypes.Long))
                    outputMessage.AppendLine("Parametry | Niepoprawna wartość parametru: \"Ilość punktów segmentacji\"");
            }
            else
                outputMessage.AppendLine("Parametry | Niepoprawny literał parametru: \"Ilość punktów segmentacji\"");

            checker = NumericConverters.StringToNumber(SelectedVoltageAmplitude, ConvertableNumericTypes.Double);
            if (checker == null)
                outputMessage.AppendLine("Parametry | Niepoprawny literał parametru: \"Wybrana amplituda napięcia\"");

            checker = NumericConverters.StringToNumber(SelectedBiasVoltage, ConvertableNumericTypes.Double);
            if (checker == null)
                outputMessage.AppendLine("Parametry | Niepoprawny literał parametru: \"Wybrane napięcie Bias DC\"");

            // Kontrola poprawności ścieżki zapisu
            bool possiblePath = SelectedOutputFile.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            if (!possiblePath || string.IsNullOrEmpty(SelectedOutputFile))
                outputMessage.AppendLine("Parametry | Niepoprawna ścieżka pliku zapisu pomiarów");
            if (File.Exists(SelectedOutputFile))
                outputMessage.AppendLine("Parametry | Plik o podanej ścieżce zapisu już istnieje");

            return outputMessage.ToString();
        }

        /// <summary>
        /// Funckja sprawdza poprawność programu i ustawień wprowadzonych przez użytkownika
        /// </summary>
        /// <returns>Czy program jest poprawny</returns>
        private bool InternalCheckProgram()
        {
            // Tworzenie wiadomości zwrotnej
            StringBuilder errorMessage = new StringBuilder();
            // Kontrola kodu programu
            try
            {
                List<CodeCommandContainer> commands = CodePreprocessor.ProcessCode(programCode);
            }
            catch (CodePreprocessorErrorException ex)
            {
                errorMessage.AppendLine(ex.Message);
            }

            // Kontrola parametrów auto pid
            string autoPidCheckMessage = CheckAutoPidInputCorrectness();
            if (!string.IsNullOrEmpty(autoPidCheckMessage))
                errorMessage.AppendLine(autoPidCheckMessage);

            // Kontrola parametrów wywołania
            string parametersCheckMessage = CheckParameters();
            if (!string.IsNullOrEmpty(parametersCheckMessage))
                errorMessage.AppendLine(parametersCheckMessage);

            // Wyświetlanie błędu jeżeli jest taki
            if (errorMessage.Length > 0)
            {
                ErrorText = errorMessage.ToString();
                ErrorTextVisibility = Visibility.Visible;
                ErrorTextBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            else
            {
                ErrorText = "Twój program jest poprawny";
                ErrorTextVisibility = Visibility.Visible;
                ErrorTextBrush = new SolidColorBrush(Colors.Green);
                return true;
            }
        }

        /// <summary>
        /// Funkcja odświeżająca obecnie wykonywaną linię kodu i ustawiająca blokadę dla interfejsu na czas interpretacji kodu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCurrentlyInterpretedLineEvent(object? sender, EventArgs e)
        {
            ViewDispatcher.Invoke(() =>
            {
                int newCurrentlyExecutedLine = CodeInterpreter.GetCurrentlyInterpretedLine();
                // Blokowanie interfejsu
                if (CurrentlyExecutedLine == -1 &&
                    newCurrentlyExecutedLine != -1)
                    IsEditInterfaceEnabled = false;
                if (newCurrentlyExecutedLine == -1)
                {
                    // Zakończenie programu/pomiarów
                    string dataToSave;
                    if (SelectedOutputFile.Substring(SelectedOutputFile.Length - 3).Equals("csv"))
                        dataToSave = MFIAStore.GenerateCSV();
                    else
                        dataToSave = MFIAStore.GenerateTSV();
                    if(!string.IsNullOrEmpty(dataToSave))
                    {
                        SaveManager.WritetoFile(SelectedOutputFile, dataToSave);
                        MaterialMessageBox.NewFastMessage(MaterialMessageFastType.Information, "Program zakończył działanie.\nPomiary zapisane.");
                    }
                    else
                        MaterialMessageBox.NewFastMessage(MaterialMessageFastType.Information, "Program zakończył działanie.\nNie wykonano pomiarów brak pliku zapisu.");
                    IsEditInterfaceEnabled = true;
                }
                // Zmiana wykonywanej lini kodu
                CurrentlyExecutedLine = newCurrentlyExecutedLine;
            });
        }

        /// <summary>
        /// Funkcja generuje pulę podpowiedzi dla edytora tekstu.
        /// </summary>
        /// <returns></returns>
        private static HintPool GenerateHintPool()
        {
            HintPool pool = new HintPool();
            foreach(string group in CommandMaster.GetGroupNames())
            {
                List<MyCompletionData> commandCompletionData = new List<MyCompletionData>();
                CommandGroup commandGroup = CommandMaster.GetCommandGroup(group);
                foreach(string command in commandGroup.GetCommandNames())
                {
                    commandCompletionData.Add(MyCompletionData.GetCommandCompletionData(
                        command: command,
                        commandGroup: group,
                        parameterCount: commandGroup.GetParametersInfo(command).Count(),
                        description: commandGroup.GetShortCommandDescription(command),
                        additionalText: commandGroup.GetAdditionalTextToInsert(command)
                        ));
                }
                pool.RegisterGroup(
                    groupName: group,
                    commandHints: commandCompletionData,
                    groupDescription: CommandMaster.GetCommandGroup(group).Description
                    );
            }
            return pool;
        }

        /// <summary>
        /// Wartości ID serii wykresu estymacji
        /// </summary>
        private enum PlotSeriesId : int
        {
            EstimatedTemperature,
            Measurements
        }
    }
}
