using System;
using System.Collections.Generic;
using System.Threading;
using PiecykM.DataConverters;
using LabServices.MFIA;
using Serilog;

namespace PiecykM.CodeProcesor
{
    /// <summary>
    /// Grupa komend programu dla mostka pomiarowego MFIA
    /// </summary>
    internal sealed class CommandGroupMfia : CommandGroup
    {
        public CommandGroupMfia() : base("mfia", "Grupa kontroli miernika MFIA")
        {
            RegisterCommand(
                "sweep",
                Sweep,
                new List<Tuple<ConvertableNumericTypes, object?, object?>>(),
                "Wykonanie pomiaru impedancji(czeka na koniec)"
                );
        }

        /// <summary>
        /// Funkcja wywołuje pomiar i czeka na jego zakończenie
        /// </summary>
        /// <param name="param"></param>
        private static void Sweep(List<object> param)
        {
            if (!MFIAController.IsActive())
            {
                Log.Error("CommandGroupMfia-Attempting to invoke a command:sweep with MFIA controller turned off");
                return;
            }
            int measurementCount = MFIAStore.GetMeasurementsCount();
            MFIAController.PushCommand(MFIAControllerCommands.MakeMeasurement);
            while(measurementCount == MFIAStore.GetMeasurementsCount())
                Thread.Sleep(50);
        }
    }
}
