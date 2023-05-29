using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PiecykM.DataConverters;
using LabServices.Lumel;
using Serilog;
using OxyPlot;
using LabControlsWPF.Plot2D;

namespace PiecykVVM.ViewModels
{
    /// <summary>
    /// ViewModel dla widoku SystemStateView.
    /// Jest to jedyny model, nie następuje przełączanie widoków.
    /// </summary>
    internal sealed partial class SystemStateViewModel : ObservableObject
    {
        public SystemStateViewModel(Dispatcher viewDispatcher)
        {
            ViewDispatcher = viewDispatcher;

            // Inicjalizacja modelu wykresu
            plotModel = new RealTimePlotModel(
                title: "Historia stanu pieca",
                yLabel: "Temperatura [°C]",
                new List<SeriesInitData>
                {
                    new SeriesInitData((int)PlotSeriesID.TargetTemperature, "Nastawa temperatury", OxyColors.Green, SeriesType.Line),
                    new SeriesInitData((int)PlotSeriesID.CurrentTemperature, "Obecna temperata", OxyColors.Red, SeriesType.Line)
                },
                maxPoinCount: 15
                );

            // Subskrybowanie danych
            LumelStore.NewCurrentTemperature += UpdateCurrentTemperatureInfo;
            LumelStore.NewCurrentTemperature += PlotNewPoint;
            LumelStore.NewTargetTemperaturePointEvent += UpdateTargetTemperatureInfo;
            LumelStore.NewTargetTemperaturePointEvent += PlotNewPoint;
            LumelStore.NewLumelPIDValueEvent += UpdatePidInfo;

            // Inicjowanie komend
            ForceParametersCommand = new RelayCommand(ForceParameters, CanForceParameters);
            this.PropertyChanged += ForceParametersCommandLockDedection;

            // Wywołanie pierwszego zaciągnięcia danych
            FirstInfoDataPull();
        }

        private Dispatcher ViewDispatcher;

        [ObservableProperty]
        private RealTimePlotModel plotModel;

        [ObservableProperty]
        private string currentTemperature = "Nieznana";
        [ObservableProperty]
        private string targetTemperature = "Nieznana";
        [ObservableProperty]
        private string paramterPValue = "Nieznana";
        [ObservableProperty]
        private string paramterIValue = "Nieznana";
        [ObservableProperty]
        private string paramterDValue = "Nieznana";

        [ObservableProperty]
        private string forcedTargetTemperatureValue = "";
        [ObservableProperty]
        private bool forceTargetTemperature = false;
        [ObservableProperty]
        private string forcedParamterPValue = "";
        [ObservableProperty]
        private bool forcePid = false;
        [ObservableProperty]
        private string forcedParamterIValue = "";
        [ObservableProperty]
        private string forcedParamterDValue = "";

        public ICommand ForceParametersCommand { get; }

        /// <summary>
        /// Funkcja zaciągająca pierwszy raz informacje o nastawach do GUI
        /// </summary>
        private void FirstInfoDataPull()
        {
            if (!LumelController.IsActive())
                return;
            double tmp1 = LumelStore.GetCurrentTemperature();
            CurrentTemperature = tmp1.ToString(".0");
            double tmp2 = LumelStore.GetTargetTemperature();
            TargetTemperature = tmp2.ToString(".0");
            LumelPidValue tmp3 = LumelStore.GetCurrentLumelPidValue();
            ParamterPValue = tmp3.ParamP.ToString();
            ParamterIValue = tmp3.ParamI.ToString();
            ParamterDValue = tmp3.ParamD.ToString();
        }

        /// <summary>
        /// Funkcja eventu zaciągająca informację o obecnej temperaturze układu.
        /// Może być bezpiecznie wywoływana przez eventy z innych wątków.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCurrentTemperatureInfo(object? sender, System.EventArgs e)
        {
            ViewDispatcher.Invoke(() =>
            {
                double tmp = LumelStore.GetCurrentTemperature();
                CurrentTemperature = tmp.ToString(".0");
            });
        }

        /// <summary>
        /// Funkcja eventu zaciągająca informację o obecnej nastawie temperatury.
        /// Może być bezpiecznie wywoływana przez eventy z innych wątków.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTargetTemperatureInfo(object? sender, System.EventArgs e)
        {
            ViewDispatcher.Invoke(() =>
            {
                double tmp = LumelStore.GetTargetTemperature();
                TargetTemperature = tmp.ToString(".0");
            });
        }

        /// <summary>
        /// Funkcja eventu zaciągająca informację o obecnych nastawach PID.
        /// Może być bezpiecznie wywoływana przez eventy z innych wątków.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePidInfo(object? sender, System.EventArgs e)
        {
            ViewDispatcher.Invoke(() =>
            {
                LumelPidValue tmp = LumelStore.GetCurrentLumelPidValue();
                ParamterPValue = tmp.ParamP.ToString();
                ParamterIValue = tmp.ParamI.ToString();
                ParamterDValue = tmp.ParamD.ToString();
            });
        }

        /// <summary>
        /// Funkcja eventu zaciągająca informacje o temperaturach do wykresu.
        /// Może być bezpiecznie wywoływana przez eventy z innych wątków.
        /// </summary>
        private void PlotNewPoint(object? sender, System.EventArgs e)
        {
            ViewDispatcher.Invoke(() =>
            {
                double tmpCurrent = LumelStore.GetCurrentTemperature();
                double tmpTarget = LumelStore.GetTargetTemperature();
                DateTime time = DateTime.Now;
                PlotModel.PushNewPoint((int)PlotSeriesID.TargetTemperature, time, tmpTarget);
                PlotModel.PushNewPoint((int)PlotSeriesID.CurrentTemperature, time, tmpCurrent);
            });
        }

        /// <summary>
        /// Funkcja wymuszająca nastawy sprzętu
        /// </summary>
        private void ForceParameters()
        {
            // Kontrola wartości następuje automatycznie w CanForceParameters
            if(ForceTargetTemperature)
            {
                LumelController.PushCommand(
                    LumelControllerCommands.SetTargetTemperature,
                    new List<object>
                    {
                        NumericConverters.StringToNumber(ForcedTargetTemperatureValue, ConvertableNumericTypes.Double)!
                    });
            }
            if(ForcePid)
            {
                LumelController.PushCommand(LumelControllerCommands.SetAutoPid); // Wyłączanie autopid jeżeli jest włączony
                LumelController.PushCommand(
                    LumelControllerCommands.SetPid,
                    new List<object>
                    {
                        new LumelPidValue(
                            (ushort)NumericConverters.StringToNumber(ForcedParamterPValue, ConvertableNumericTypes.UShort)!,
                            (ushort)NumericConverters.StringToNumber(ForcedParamterIValue, ConvertableNumericTypes.UShort)!,
                            (ushort)NumericConverters.StringToNumber(ForcedParamterDValue, ConvertableNumericTypes.UShort)!
                            )
                    });
            }
        }

        /// <summary>
        /// Funkcja sprawdzająca czy spełniono wymagania wywołania wymuszenia parametrów
        /// </summary>
        /// <returns></returns>
        private bool CanForceParameters()
        {
            // Sprawdzenie czy są parametry do wysłania
            if( !ForceTargetTemperature &&
                !ForcePid
                )
                return false;

            // Sprawdzanie konwertowalności typu
            if(ForceTargetTemperature)
                if (NumericConverters.StringToNumber(ForcedTargetTemperatureValue, ConvertableNumericTypes.Double) == null)
                    return false;
            if(ForcePid)
            {
                if (NumericConverters.StringToNumber(ForcedParamterPValue, ConvertableNumericTypes.UShort) == null)
                    return false;
                if (NumericConverters.StringToNumber(ForcedParamterIValue, ConvertableNumericTypes.UShort) == null)
                    return false;
                if (NumericConverters.StringToNumber(ForcedParamterDValue, ConvertableNumericTypes.UShort) == null)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Funkcja nadzorująca blokadę przycisku wymuszenia parametrów
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForceParametersCommandLockDedection(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( e.PropertyName == nameof(ForceTargetTemperature) ||
                e.PropertyName == nameof(ForcedTargetTemperatureValue) ||
                e.PropertyName == nameof(ForcePid) ||
                e.PropertyName == nameof(ForcedParamterPValue) ||
                e.PropertyName == nameof(ForcedParamterIValue) ||
                e.PropertyName == nameof(ForcedParamterDValue)
                )
            {
                (ForceParametersCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Zmienna identyfiukująca serie danych na wykresie
        /// </summary>
        private enum PlotSeriesID : int
        {
            TargetTemperature,
            CurrentTemperature
        }
    }
}
