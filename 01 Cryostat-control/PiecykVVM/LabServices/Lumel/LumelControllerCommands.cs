using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabServices.DataTemplates;
using Serilog;

namespace LabServices.Lumel
{
    public enum LumelControllerCommands : ushort
    {
        /// <summary>
        /// Ustawia okres działania pętli komunikacji
        /// Wartość dolnie ograniczona ze względu na czas operacji odczytu/zapisu LumelRe72 równy ~35 ms
        /// Przy zastosowaniu wartości z poza granicy ustawiana jest graniczna
        /// Domyślnie: 200 ms
        /// Ograniczenie: (100, 60000) - (0.1s, 1min)
        /// Parametry: long(czas w ms)
        /// </summary>
        SetLumelEnginePeriod,
        /// <summary>
        /// Ustawia co ile okresów pętli odczytać temperaturę
        /// Domyślnie: 5
        /// Ograniczenie: >= 1
        /// Parametry: long(ilość okresów)
        /// </summary>
        SetLumelEngineReadMultipler,
        /// <summary>
        /// Nastawia wartość PID sterownika Lumel
        /// Parametry: LumelPidValue
        /// </summary>
        SetPid,
        /// <summary>
        /// Nastawia docelową temperaturę sterownika Lumel
        /// Ograniczenie: Patrz dokumentacja Lumel Re72
        /// Parametry: double(temperatura)
        /// </summary>
        SetTargetTemperature,
        /// <summary>
        /// Nastawia docelową temperaturę sterownika Lumel na wartość obecnego odczytu
        /// Parametry brak
        /// </summary>
        SetTargetTemperatureToPresent,
        /// <summary>
        /// Zmienia wartości nastawy temperatury o zadaną wartość
        /// Parametry: double(zmiana może być ujemna)
        /// </summary>
        ChangeTargetTemperature,
        /// <summary>
        /// Ustawia auto pid
        /// Ograniczenie: Pojedyńcza nastawa w zakresie 0 - 9999
        /// Parametry: LumelAutoPidPool
        /// </summary>
        SetAutoPid
    }
    public sealed partial class LumelController : HardwareController
    {
        protected override void RegisterCommands()
        {
            _commandPool.RegisterCommand((ushort)LumelControllerCommands.SetLumelEnginePeriod, SetLumelEnginePeriod);
            _commandPool.RegisterCommand((ushort)LumelControllerCommands.SetLumelEngineReadMultipler, SetLumelEngineReadMultipler);
            _commandPool.RegisterCommand((ushort)LumelControllerCommands.SetPid, SetPid);
            _commandPool.RegisterCommand((ushort)LumelControllerCommands.SetTargetTemperature, SetTargetTemperature);
            _commandPool.RegisterCommand((ushort)LumelControllerCommands.SetTargetTemperatureToPresent, SetTargetTemperatureToPresent);
            _commandPool.RegisterCommand((ushort)LumelControllerCommands.ChangeTargetTemperature, ChangeTargetTemperature);
            _commandPool.RegisterCommand((ushort)LumelControllerCommands.SetAutoPid, SetAutoPid);
        }

        private void SetLumelEnginePeriod(List<object> param)
        {
            if (param.Count != 1 ||
                param[0] is not long)
            {
                Log.Error($"Bad parameter in LumelController.SetLumelEnginePeriod");
                return;
            }

            long lumelEnginePeriod = (long)param[0];
            if (lumelEnginePeriod < 100)
            {
                Log.Error($"LumelController.SetLumelEnginePeriod-TooSmallValue:{lumelEnginePeriod}");
                lumelEnginePeriod = 100;
            }
            if (lumelEnginePeriod > 60000)
            {
                Log.Error($"LumelController.SetLumelEnginePeriod-TooBigValue:{lumelEnginePeriod}");
                lumelEnginePeriod = 60000;
            }
            _enginePeriod = lumelEnginePeriod;
            LumelStore.SetCurrentEnginePeriod(lumelEnginePeriod);
        }

        private void SetLumelEngineReadMultipler(List<object> param)
        {
            if (param.Count != 1 ||
                param[0] is not long)
            {
                Log.Error($"Bad parameter in LumelController.SetLumelEngineReadMultipler");
                return;
            }
            _engineReadMultipler = (long)param[0];
        }

        private void SetPid(List<object> param)
        {
            if(_autoPidPool != null)
            {
                Log.Information("Skiping pid set because auto pid is active in LumelController.SetPid");
                return;
            }
            if (param.Count != 1 ||
                param[0] is not LumelPidValue)
            {
                Log.Error($"Bad parameter in LumelController.SetPid");
                return;
            }

            LumelPidValue pidValue = (LumelPidValue)param[0];
            _lumelInterface.WriteMultipleRegisters(
                startAddress: (ushort)LumelRe72Registers.Pid1ParameterP,
                values: new ushort[] { pidValue.ParamP, pidValue.ParamI, pidValue.ParamD }
                );
            LumelStore.SetLumelPidValue(pidValue);
        }

        private void SetTargetTemperature(List<object> param)
        {
            if (param.Count != 1 ||
                param[0] is not double)
            {
                Log.Error($"Bad parameter in LumelController.SetTargetTemperature");
                return;
            }

            double doubleTargetTemperature = (double)param[0];
            ushort targetTemperature = LumelDataConverter.ConvertTemperatureDoubleToUshort(
                doubleTargetTemperature,
                _temperatureSensor
                );
            _lumelInterface.WriteSingleRegister(
                (ushort)LumelRe72Registers.TargetTemperature,
                targetTemperature
                );
            LumelStore.SetTargetTemperature(doubleTargetTemperature);
        }

        private void SetTargetTemperatureToPresent(List<object> param)
        {
            double doubleCurrentTemperature = LumelStore.GetCurrentTemperature();
            ushort currentTemperature = LumelDataConverter.ConvertTemperatureDoubleToUshort(
                doubleCurrentTemperature,
                _temperatureSensor
                );
            _lumelInterface.WriteSingleRegister(
                (ushort)LumelRe72Registers.CurrentTemperature,
                currentTemperature
                );
            LumelStore.SetTargetTemperature(doubleCurrentTemperature);
        }

        private void ChangeTargetTemperature(List<object> param)
        {
            if (param.Count != 1 ||
                param[0] is not double)
            {
                Log.Error($"Bad parameter in LumelController.ChangeTargetTemperature");
                return;
            }

            double doubleTargetTemperature = LumelStore.GetTargetTemperature() + (double)param[0];
            ushort targetTemperature = LumelDataConverter.ConvertTemperatureDoubleToUshort(
                 doubleTargetTemperature,
                _temperatureSensor
                );
            _lumelInterface.WriteSingleRegister(
                (ushort)LumelRe72Registers.TargetTemperature,
                targetTemperature
                );
            LumelStore.SetTargetTemperature(doubleTargetTemperature);
        }

        private void SetAutoPid(List<object> param)
        {
            if (param.Count > 1 ||
                (param.Count == 1 && param[0] is not LumelAutoPidPool))
            {
                Log.Error($"Bad parameter in LumelController.SetAutoPid");
                return;
            }
            if (param.Count == 0)
                _autoPidPool = null;
            else
                _autoPidPool = (LumelAutoPidPool)param[0];
        }
    }
}
