using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabControlsWPF.Plot3D;
using LabControlsWPF;
using LabServices.MFIA;

using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PiecykVVM.ViewModels
{
    internal sealed partial class Measurement3DPlotViewModel : ObservableObject
    {
        public Measurement3DPlotViewModel(Dispatcher dispatcher)
        {
            ViewDispatcher = dispatcher;

            // Subskrybowanie danych
            MFIAStore.NewMeasuermentEvent += PlotNewMeasuermentEvent;

            // Inicjowanie komend
            GeneratePlotCommand = new RelayCommand(GeneratePlot);
        }

        private Dispatcher ViewDispatcher;

        [ObservableProperty]
        private List<string> axisTypeList = new List<string> { "Liniowa", "Logarytmiczna" };
        private Dictionary<string, AxisType> _axisTypeMap = new Dictionary<string, AxisType>
        {
            ["Liniowa"] = AxisType.Linear,
            ["Logarytmiczna"] = AxisType.Logarytmic
        };
        [ObservableProperty]
        private string selectedXAxisType = "Liniowa";
        [ObservableProperty]
        private string selectedYAxisType = "Liniowa";
        [ObservableProperty]
        private string selectedZAxisType = "Liniowa";

        [ObservableProperty]
        private List<string> plotTypeList = new List<string> { "Czas-Częstotliwość", "Temperatura-Częstotliwość" };
        [ObservableProperty]
        private string selectedPlotType = "Czas-Częstotliwość";
        [ObservableProperty]
        private List<string> plotedParameterList = new List<string> { "ABS", "Im", "Re", "Phase", "TanDelta" };
        [ObservableProperty]
        private string selectedPlotedParameter = "ABS";

        [ObservableProperty]
        private string xAxisTitle = "Oś X";
        [ObservableProperty]
        private string yAxisTitle = "Oś Y";
        [ObservableProperty]
        private string zAxisTitle = "Oś Z";

        [ObservableProperty]
        private AxisType xAxisType = AxisType.Linear;
        [ObservableProperty]
        private AxisType yAxisType = AxisType.Linear;
        [ObservableProperty]
        private AxisType zAxisType = AxisType.Linear;

        [ObservableProperty]
        private bool plotXInfoLines = true;
        [ObservableProperty]
        private bool plotYInfoLines = true;
        [ObservableProperty]
        private bool plotVerdicalInfoLines = false;

        [ObservableProperty]
        private bool livePlot = false;

        /// <summary>Oświetlenie wykresu</summary>
        public Model3DGroup Lights
        {
            get
            {
                var group = new Model3DGroup();
                // group.Children.Add(new AmbientLight(Colors.White));
                group.Children.Add(new AmbientLight(Colors.Gray));
                group.Children.Add(new PointLight(Colors.Red, new Point3D(0, -1000, 0)));
                group.Children.Add(new PointLight(Colors.Blue, new Point3D(0, 0, 1000)));
                group.Children.Add(new PointLight(Colors.Green, new Point3D(1000, 1000, 0)));
                return group;
            }
        }

        /// <summary>Lista plotowanych punktów</summary>
        [ObservableProperty]
        private Point3D[,] data;
        [ObservableProperty]
        private object plotInvalidateFlag;

        public RelayCommand GeneratePlotCommand { get; }


        /// <summary>
        /// Funkcja generuje wykres
        /// </summary>
        private void GeneratePlot()
        {
            // Podpisy osi X i Y
            if (SelectedPlotType.Equals("Czas-Częstotliwość"))
            {
                XAxisTitle = "Czas [s]";
                YAxisTitle = "Częstotliwość [Hz]";
            }
            else // "Temperatura-Częstotliwość"
            {
                XAxisTitle = "Temperatura [°C]";
                YAxisTitle = "Częstotliwość [Hz]";
            }

            // Podpis osi Z
            if (SelectedPlotedParameter.Equals("ABS"))
                ZAxisTitle = "ABS [Ohm]";
            else if (SelectedPlotedParameter.Equals("Im"))
                ZAxisTitle = "Im [Ohm]";
            else if (SelectedPlotedParameter.Equals("Re"))
                ZAxisTitle = "Re [Ohm]";
            else if (SelectedPlotedParameter.Equals("Phase"))
                ZAxisTitle = "Phase";
            else if (SelectedPlotedParameter.Equals("TanDelta"))
                ZAxisTitle = "TanDelta";

            // Typy osi
            XAxisType = _axisTypeMap[SelectedXAxisType];
            YAxisType = _axisTypeMap[SelectedYAxisType];
            ZAxisType = _axisTypeMap[SelectedZAxisType];

            // Plotowane dane
            Data = GetPointsToPlot();

            PlotInvalidateFlag = new object();
            // MaterialMessageBox.NewFastMessage(MaterialMessageFastType.NotImplementedWarning, "Generowanie wykresu");
        }

        /// <summary>
        /// Funkcja uzyskuje punkty pomiarów do splotowania
        /// </summary>
        /// <returns></returns>
        public Point3D[,] GetPointsToPlot()
        {
            List<MFIAMeasurement> measurements = MFIAStore.GetAllMeasurements();
            if (measurements.Count == 0)
            {
                // Brak danych do plotowania
                return new Point3D[0, 0];
            }

            // Sortowanie punktów temperatury
            if (SelectedPlotType.Equals("Temperatura-Częstotliwość"))
            {
                measurements.Sort(CompareMFIAMeasurementByTemperature);
                List<MFIAMeasurement> measurementsToRemove = new List<MFIAMeasurement>();
                if (measurements.Count > 1)
                    for (int i = 0; i < measurements.Count - 1; i++)
                        if (measurements[i].Temperature == measurements[i + 1].Temperature)
                            measurementsToRemove.Add(measurements[i]);
                foreach (MFIAMeasurement elem in measurementsToRemove)
                    measurements.Remove(elem);
            }

            // Point3D[,] result = new Point3D[measurements.Count, measurements[0].Freq.Length];
            Point3D[,] result = new Point3D[measurements[0].Freq.Length, measurements.Count];
            for (int i = 0; i < measurements.Count; i++)
                for (int j = 0; j < measurements[0].Freq.Length; j++)
                {
                    double x;
                    double y = measurements[0].Freq[j];
                    double z;
                    // Wybieranie danej osi x
                    if (SelectedPlotType.Equals("Czas-Częstotliwość"))
                    {
                        // x = (measurements[0].TimeStamp - measurements[i].TimeStamp).TotalSeconds;
                        x = (measurements[measurements.Count - 1].TimeStamp - measurements[measurements.Count - 1 - i].TimeStamp).TotalSeconds;
                    }
                    else // "Temperatura-Częstotliwość"
                    {
                        x = measurements[measurements.Count - 1 - i].Temperature;
                    }
                    // Wybieranie danej osi z
                    if (SelectedPlotedParameter.Equals("ABS"))
                    {
                        z = measurements[measurements.Count - 1 - i].ABS[j];
                    }
                    else if (SelectedPlotedParameter.Equals("Im"))
                    {
                        z = measurements[measurements.Count - 1 - i].Im[j];
                    }
                    else if (SelectedPlotedParameter.Equals("Re"))
                    {
                        z = measurements[measurements.Count - 1 - i].Re[j];
                    }
                    else if (SelectedPlotedParameter.Equals("Phase"))
                    {
                        z = measurements[measurements.Count - 1 - i].Phase[j];
                    }
                    else // "TanDelta"
                    {
                        z = measurements[measurements.Count - 1 - i].TanDelta[j];
                    }
                    result[j, i] = new Point3D(x, y, z);
                }
            return result;
        }

        /// <summary>
        /// Funkcja eventu wywoływana przy nowym pomiarze
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PlotNewMeasuermentEvent(object? sender, EventArgs e)
        {
            if (LivePlot && MFIAStore.GetMeasurementsCount() >= 3)
            {
                // Wykonywanie nowego wykresu
                GeneratePlot();
            }
        }

        /// <summary>
        /// Funkcja porównuje elementy MFIAMeasurement po wartości temperatury
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static int CompareMFIAMeasurementByTemperature(MFIAMeasurement first, MFIAMeasurement second)
        {
            if (first.Temperature < second.Temperature)
                return -1;
            if (first.Temperature > second.Temperature)
                return 1;
            return 0;
        }

        private static Point3D[,] CreateTestArray()
        {
            int Rows = 50;
            int Columns = 100;
            double MinX = 0;
            double MaxX = 5;
            double MinY = 0;
            double MaxY = 5;
            Point3D[,] data = new Point3D[Rows, Columns];
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                {
                    double x = MinX + (double)j / (Columns - 1) * (MaxX - MinX);
                    double y = MinY + (double)i / (Rows - 1) * (MaxY - MinY);
                    data[i, j] = new Point3D(
                        x * 5,
                        y,
                        Math.Sin(x * y) * 10
                        // x * y
                        );
                }

            return data;
        }

        private static void SaveMesh(Point3D[,] mesh)
        {
            StringBuilder builder = new StringBuilder();
            int rows = mesh.GetUpperBound(0) + 1;
            int columns = mesh.GetUpperBound(1) + 1;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    builder.Append($"({mesh[i, j].X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{mesh[i, j].Y.ToString(System.Globalization.CultureInfo.InvariantCulture)},{mesh[i, j].Z.ToString(System.Globalization.CultureInfo.InvariantCulture)})");
                }
                builder.Append("\n");
            }
            string saveFile = PiecykM.SaveProvider.SaveManager.GetExampleSaveFileName("Mesh save", "txt");
            PiecykM.SaveProvider.SaveManager.WritetoFile(
                PiecykM.SaveProvider.SaveManager.AppFolder_UserData + PiecykM.SaveProvider.SaveManager.DirectorySeparator + saveFile,
                builder.ToString()
                );
        }
    }
}
