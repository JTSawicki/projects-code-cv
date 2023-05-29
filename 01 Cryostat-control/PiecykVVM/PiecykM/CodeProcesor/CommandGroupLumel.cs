using System;
using System.Threading;
using System.Collections.Generic;
using LabServices.Lumel;
using PiecykM.DataConverters;
using Serilog;

namespace PiecykM.CodeProcesor
{
    /// <summary>
    /// Grupa komend programu dla sterownika Lumel
    /// </summary>
    internal sealed class CommandGroupLumel : CommandGroup
    {
        public CommandGroupLumel() : base("lumel", "Grupa kontroli sterownika Lumel Re72")
        {
            RegisterCommand(
                "changeTemperature",
                ChangeTargetTemperature,
                new List<Tuple<ConvertableNumericTypes, object?, object?>>
                {
                    new Tuple<ConvertableNumericTypes, object?, object?>(
                        ConvertableNumericTypes.Double,
                        null,
                        null
                    )
                },
                "Zmienia nastawę temperatury o zadaną wartość"
                );
            RegisterCommand(
                "setPid",
                SetPid,
                new List<Tuple<ConvertableNumericTypes, object?, object?>>
                {
                    new Tuple<ConvertableNumericTypes, object?, object?> (
                        ConvertableNumericTypes.UShort,
                        (ushort)1,
                        (ushort)9999
                    ),
                    new Tuple<ConvertableNumericTypes, object?, object?> (
                        ConvertableNumericTypes.UShort,
                        (ushort)0,
                        (ushort)9999
                    ),
                    new Tuple<ConvertableNumericTypes, object?, object?> (
                        ConvertableNumericTypes.UShort,
                        (ushort)0,
                        (ushort)9999
                    )
                },
                "Ustawia parametry pid"
                );
            RegisterCommand(
                "setTemperature",
                SetTargetTemperature,
                new List<Tuple<ConvertableNumericTypes, object?, object?>>
                {
                    new Tuple<ConvertableNumericTypes, object?, object?>(
                        ConvertableNumericTypes.Double,
                        null,
                        null
                        )
                },
                "Ustawiająca nastawę temperatury"
                );
            RegisterCommand(
                "setTemperatureToPresent",
                SetTargetTemperatureToPresent,
                new List<Tuple<ConvertableNumericTypes, object?, object?>>(),
                "Ustawia nastawę temperatury na obecny odczyt"
                );
            RegisterCommand(
                "waitUntilTemperature",
                WaitUntilTemperature,
                new List<Tuple<ConvertableNumericTypes, object?, object?>>
                {
                    new Tuple<ConvertableNumericTypes, object?, object?>(
                        ConvertableNumericTypes.Double,
                        null,
                        null
                        ),
                    new Tuple<ConvertableNumericTypes, object?, object?>(
                        ConvertableNumericTypes.Double,
                        null,
                        null
                        )
                },
                "Jeżeli jest to możliwe czeka na osiągnięcie wskazanej temperatury"
                );
        }

        /// <summary>
        /// Zmienia nastawę temperatury o zadaną wartość
        /// </summary>
        /// <param name="param"></param>
        private static void ChangeTargetTemperature(List<object> param)
        {
            if(!LumelController.IsActive())
            {
                Log.Error("CommandGroupLumel-Attempting to invoke a command:ChangeTargetTemperature with Lumel controller turned off");
                return;
            }
            if (param.Count != 1)
            {
                Log.Error($"CommandGroupLumel.SetTemperature-Invalid parameter count: {param.Count}");
                return;
            }
            if (param[0] is not double)
            {
                Log.Error($"CommandGroupLumel.SetTemperature-Invalid parameter type: {param[0].GetType()}");
                return;
            }
            LumelController.PushCommand(
                LumelControllerCommands.ChangeTargetTemperature,
                new List<object> { param[0] }
                );
        }

        /// <summary>
        /// Ustawia nastawę pid sterownika Lumel na zadaną wartość
        /// </summary>
        /// <param name="param"></param>
        private static void SetPid(List<object> param)
        {
            if (!LumelController.IsActive())
            {
                Log.Error("CommandGroupLumel-Attempting to invoke a command:SetPid with Lumel controller turned off");
                return;
            }
            if (param.Count != 3)
            {
                Log.Error($"CommandGroupLumel.SetPid-Invalid parameter count: {param.Count}");
                return;
            }
            if (param[0] is not ushort ||
                param[1] is not ushort ||
                param[2] is not ushort)
            {
                Log.Error($"CommandGroupLumel.SetPid-Invalid parameter type: {param[0].GetType()},{param[1].GetType()},{param[2].GetType()}");
                return;
            }
            LumelPidValue newPidValue = new LumelPidValue(
                (ushort)param[0],
                (ushort)param[1],
                (ushort)param[2]
                );
            LumelController.PushCommand(
                LumelControllerCommands.SetPid,
                new List<object> { newPidValue }
                );
        }

        /// <summary>
        /// Ustawia docelową temperaturę sterownika Lumel
        /// </summary>
        /// <param name="param"></param>
        private static void SetTargetTemperature(List<object> param)
        {
            if (!LumelController.IsActive())
            {
                Log.Error("CommandGroupLumel-Attempting to invoke a command:SetTargetTemperature with Lumel controller turned off");
                return;
            }
            if (param.Count != 1)
            {
                Log.Error($"CommandGroupLumel.SetTemperature-Invalid parameter count: {param.Count}");
                return;
            }
            if (param[0] is not double)
            {
                Log.Error($"CommandGroupLumel.SetTemperature-Invalid parameter type: {param[0].GetType()}");
                return;
            }
            LumelController.PushCommand(
                LumelControllerCommands.SetTargetTemperature,
                new List<object> { param[0] }
                );
        }

        /// <summary>
        /// Ustawia docelową temperaturę sterownika Lumel na obecny odczyt
        /// </summary>
        /// <param name="param"></param>
        private static void SetTargetTemperatureToPresent(List<object> param)
        {
            if (!LumelController.IsActive())
            {
                Log.Error("CommandGroupLumel-Attempting to invoke a command:SetTargetTemperatureToPresent with Lumel controller turned off");
                return;
            }
            LumelController.PushCommand(LumelControllerCommands.SetTargetTemperatureToPresent);
        }

        /// <summary>
        /// Jeżeli jest to możliwe czeka na osiągnięcie zadanej temperatury
        /// </summary>
        /// <param name="param"></param>
        private static void WaitUntilTemperature(List<object> param)
        {
            if (!LumelController.IsActive())
            {
                Log.Error("CommandGroupLumel-Attempting to invoke a command:WaitUntilTemperature with Lumel controller turned off");
                return;
            }
            if (param.Count != 2)
            {
                Log.Error($"CommandGroupLumel.WaitUntilTemperature-Invalid parameter count: {param.Count}");
                return;
            }
            if (param[0] is not double ||
                param[1] is not double )
            {
                Log.Error($"CommandGroupLumel.WaitUntilTemperature-Invalid parameter type: {param[0].GetType()},{param[1].GetType()}");
                return;
            }

            // Odczekanie 1.5 okresu kontrolera Lumel aby zostały zastosowane potencjalne zmiany temperatury.
            int waitTime = (int)(LumelStore.GetCurrentEnginePeriod() * 1.5);
            Thread.Sleep(waitTime);

            double waitTargetTemperature = (double)param[0];
            double waitTargetTemperatureSpreadMargin = (double)param[1];
            while (true)
            {
                double targetTemperature = LumelStore.GetTargetTemperature();
                double currentTemperature = LumelStore.GetCurrentTemperature();
                // Osiągnięcie dokładnego warunku zakończenia
                if (currentTemperature >= waitTargetTemperature - waitTargetTemperatureSpreadMargin &&
                    currentTemperature <= waitTargetTemperature + waitTargetTemperatureSpreadMargin )
                    break;
                // Sprawdzenie osiągalności temperatury
                if ( // Obecny stan leży pomiędzy nastawą oraz oczekiwaną wartością
                     // Wywołanie następuje również jeżli przekroczono temperaturę oczekiwaną i system zmieża do większej zadanej
                     (waitTargetTemperature - currentTemperature) * (currentTemperature - targetTemperature) > 0 ||
                     // Obecny stan leży po przeciwnej stronie nastawy niż oczekiwana wartość
                     Math.Abs(currentTemperature - waitTargetTemperature) > Math.Abs(currentTemperature - targetTemperature)
                   )
                {
                    // Nie można osiągnąć oczekiwanego stanu
                    break;
                }
                Thread.Sleep(10);
            }
        }
    }
}
