using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabControlsWPF.Plot2D;
using LabServices.MFIA;

namespace PiecykVVM.ViewModels
{
    internal sealed partial class Measurement2DPlotViewModel : ObservableObject
    {
        public Measurement2DPlotViewModel(Dispatcher viewDispatcher)
        {
            ViewDispatcher = viewDispatcher;

            // Subskrybowanie danych
            MFIAStore.NewMeasuermentEvent += PlotNewMeasuermentEvent;

            // Inicjowanie komend
            ChangePlotParametersCommand = new RelayCommand(ChangePlotParameters);

            // Konstruowanie modelu wykresu
            plotModel = new MultiSeriesPlotModel("Erorr", "", "", new List<SeriesInitData>());
            GenerateNewPlotModel();
        }

        private Dispatcher ViewDispatcher;

        [ObservableProperty]
        private MultiSeriesPlotModel plotModel;

        private const int minShownMeasurementCount = 1;
        private const int maxShownMeasurementCount = 20;
        private const int defaultShownMeasurementCount = 5;
        [ObservableProperty]
        private List<string> shownMeasurementCountList =
            Enumerable.Range(minShownMeasurementCount, maxShownMeasurementCount - minShownMeasurementCount + 1)
            .Select(i => i.ToString())
            .ToList();
        [ObservableProperty]
        private string selectedShownMeasurementCount = defaultShownMeasurementCount.ToString();
        private int _shownMeasurementCount = defaultShownMeasurementCount;

        [ObservableProperty]
        private int currentlyBufforedMeasurementCount = 0;

        [ObservableProperty]
        private List<string> valueToPlotList = new List<string>
        {
            "ABS",
            "Im",
            "Re",
            "Phase",
            "TanDelta"
        };
        [ObservableProperty]
        private string selectedValueToPlot = "ABS";
        private string _valueToPlot = "ABS";
        private Dictionary<string, string> _valueToPlotUnitMap = new Dictionary<string, string>
        {
            ["ABS"] = "Ohm",
            ["Im"] = "Ohm",
            ["Re"] = "Ohm",
            ["Phase"] = "",
            ["TanDelta"] = "",
        };

        [ObservableProperty]
        private List<string> axisTypeList = new List<string>
        {
            "Liniowa",
            "Logarytmiczna"
        };
        private Dictionary<string, AxisType> _axisTypeMap = new Dictionary<string, AxisType>
        {
            ["Liniowa"] = AxisType.Linear,
            ["Logarytmiczna"] = AxisType.Logarytmic
        };
        [ObservableProperty]
        private string selectedXAxisType = "Liniowa";
        [ObservableProperty]
        private string selectedYAxisType = "Liniowa";

        public RelayCommand ChangePlotParametersCommand { get; }


        /// <summary>Funkcja zmienia parametry wykresu</summary>
        private void ChangePlotParameters()
        {
            _shownMeasurementCount = int.Parse(SelectedShownMeasurementCount);
            _valueToPlot = SelectedValueToPlot;
            GenerateNewPlotModel();
            PlotAvalibleData();
        }

        /// <summary>
        /// Funkcja plotuje nowy pomiar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PlotNewMeasuermentEvent(object? sender, EventArgs e)
        {
            CurrentlyBufforedMeasurementCount = MFIAStore.GetMeasurementsCount();
            if (MFIAStore.GetMeasurementsCount() == 0)
            {
                // Nastąpiło wyczyszczenie buforów
                // Wykonywanie akcji czyszczenia wykresu
                for (int i = 0; i < _shownMeasurementCount; i++)
                    PlotModel.PushSeriesData(i, new List<Tuple<double, double>>());
            }
            else
                PlotAvalibleData();
        }

        /// <summary>
        /// Funkcja służąca do generowania nowego modelu wykresu przy zmianie jego parametrów
        /// </summary>
        private void GenerateNewPlotModel()
        {
            List<SeriesInitData> newSeries = new List<SeriesInitData>();
            if(_shownMeasurementCount >= 1)
            {
                newSeries.Add(
                    new SeriesInitData(
                        0,
                        "Ostatni pomiar",
                        OxyPlot.OxyColors.Red,
                        SeriesType.Line)
                    );
            }
            for(int i = 1; i < _shownMeasurementCount; i++)
            {
                newSeries.Add(
                    new SeriesInitData(
                        i,
                        $"Pomiar -{i}",
                        OxyPlot.OxyColor.FromArgb( // Wykorzystano cieniowanie koloru pomarańczowego rgb(255, 131, 0)
                            (byte)(65 + (190 / (_shownMeasurementCount - 1)) * (_shownMeasurementCount - i)),
                            (byte)(255 * (1 - (0.75 / _shownMeasurementCount * i))),
                            (byte)(131 * (1 - (0.75 / _shownMeasurementCount * i))),
                            0
                            ),
                        SeriesType.Line)
                    );
            }
            PlotModel = new MultiSeriesPlotModel(
                title: "Ostatnie pomiary",
                xLabel: "Częstotliwość [Hz]",
                yLabel: $"{_valueToPlot} [{_valueToPlotUnitMap[_valueToPlot]}]",
                series: newSeries,
                xAxis: _axisTypeMap[SelectedXAxisType],
                yAxis: _axisTypeMap[SelectedYAxisType]
                );
        }

        /// <summary>
        /// Funkcja wyświetla dane jeżeli te są dostępne
        /// </summary>
        private void PlotAvalibleData()
        {
            // Pobieranie danych
            List<MFIAMeasurement> data = MFIAStore.TryGetLastMeasurements(_shownMeasurementCount);
            // Plotownaie pomiarów
            for (int i = 0; i < data.Count; i++)
            {
                double[] yValue;
                switch(_valueToPlot)
                {
                    case "ABS":
                        yValue = data[i].ABS;
                        break;
                    case "Im":
                        yValue = data[i].Im;
                        break;
                    case "Re":
                        yValue = data[i].Re;
                        break;
                    case "Phase":
                        yValue = data[i].Phase;
                        break;
                    default: // TanDelta
                        yValue = data[i].TanDelta;
                        break;
                }
                List<Tuple<double, double>> seriesData = new List<Tuple<double, double>>(data[i].Freq.Length);
                for (int j = 0; j < data[i].Freq.Length; j++)
                    seriesData.Add(new Tuple<double, double>(data[i].Freq[j], yValue[j]));
                plotModel.PushSeriesData(i, seriesData);
            }
        }
    }
}
