using System;
using System.Collections.Generic;
using LabServices.DataTemplates;
using Serilog;

namespace LabServices.MFIA
{
    /// <summary>
    /// Enumeracja wyliczająca kody komend kontrolera MFIA
    /// </summary>
    public enum MFIAControllerCommands : ushort
    {
        /// <summary>
        /// Wynonanie pomiaru.
        /// Parametry brak
        /// </summary>
        MakeMeasurement,
        /// <summary>
        /// Nastawia parametry sweepera.
        /// Parametry: MFIASweeperInitData
        /// </summary>
        SetSweeperParameters
    }

    public sealed partial class MFIAController : HardwareController
    {
        protected override void RegisterCommands()
        {
            _commandPool.RegisterCommand((ushort)MFIAControllerCommands.MakeMeasurement, MakeMeasurement);
            _commandPool.RegisterCommand((ushort)MFIAControllerCommands.SetSweeperParameters, SetSweeperParameters);
        }

        private void SetSweeperParameters(List<object> param)
        {
            if (param.Count != 1 ||
                param[0] is not MFIASweeperInitData)
            {
                Log.Error($"Bad parameter in MFIAController.SetSweeperParameters");
                return;
            }
            _mfiaInterface.SetSweeperParameters((MFIASweeperInitData)param[0]);
        }

        private void MakeMeasurement(List<object> param)
        {
            _mfiaInterface.MakeMeasurement();
        }
    }
}
