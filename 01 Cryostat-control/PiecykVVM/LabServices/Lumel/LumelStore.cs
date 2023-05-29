using System;
using LabServices.DataTemplates;

namespace LabServices.Lumel
{
    public static class LumelStore
    {
        /// <summary>Obecna nastawa temperatury</summary>
        private static LockedProperty<double> _targetTemperature = new LockedProperty<double>(0);
        /// <summary>
        /// Publiczny event wywoływany przy dodaniu nowego punktu nastawy temperatury
        /// Event może być wywoływany przez inne wątki - zabezpieczenie po stronie odbiorcy
        /// </summary>
        public static event EventHandler? NewTargetTemperaturePointEvent;

        /// <summary>Obecna temperatura wskazywana przez sterownik Lumel</summary>
        private static LockedProperty<double> _currentTemperature = new LockedProperty<double>(0);
        /// <summary>
        /// Publiczny event wywoływany przy dodaniu nowego punktu odczytu temperatury
        /// Event może być wywoływany przez inne wątki - zabezpieczenie po stronie odbiorcy
        /// </summary>
        public static event EventHandler? NewCurrentTemperature;

        /// <summary>Obecna wartość PID wskazywana przez sterownik Lumel</summary>
        private static LockedProperty<LumelPidValue> _currentLumelPidValue = new LockedProperty<LumelPidValue>(
            new LumelPidValue { ParamP = 0, ParamI = 0, ParamD = 0 }
            );
        /// <summary>
        /// Publiczny event wywoływany przy zmianie parametrów PID sterownika Lumel
        /// Event może być wywoływany przez inne wątki - zabezpieczenie po stronie odbiorcy
        /// </summary>
        public static event EventHandler? NewLumelPIDValueEvent;

        private static LockedProperty<long> _currentEnginePeriod = new LockedProperty<long>(200);

        /// <summary>
        /// Funkcja ustawia punkt nastawy temperatury
        /// </summary>
        /// <param name="temperature"></param>
        internal static void SetTargetTemperature(double temperature)
        {
            _targetTemperature.Set(temperature);
            NewTargetTemperaturePointEvent?.Invoke(new object(), EventArgs.Empty);
        }

        /// <summary>
        /// Funkcja ustawia nowy punkt odczytu temperatury
        /// </summary>
        /// <param name="temperature"></param>
        internal static void SetCurrentTemperature(double temperature)
        {
            _currentTemperature.Set(temperature);
            NewCurrentTemperature?.Invoke(new object(), EventArgs.Empty);
        }

        /// <summary>
        /// Funkcja ustawia nowe wartości PID
        /// </summary>
        /// <param name="pidValue"></param>
        internal static void SetLumelPidValue(LumelPidValue pidValue)
        {
            _currentLumelPidValue.Set(pidValue);
            NewLumelPIDValueEvent?.Invoke(new object(), EventArgs.Empty);
        }

        /// <summary>
        /// Ustawia obecny okres kontrolera
        /// </summary>
        /// <returns></returns>
        public static void SetCurrentEnginePeriod(long newEnginePeriod)
        {
            _currentEnginePeriod.Set(newEnginePeriod);
        }

        /// <summary>
        /// Funkcja zwraca obecną wartość nastawy temperatury
        /// </summary>
        /// <returns></returns>
        public static double GetTargetTemperature()
        {
            return _targetTemperature.Get();
        }

        /// <summary>
        /// Funcja zwraca obecną wartość odczytu temperatury
        /// </summary>
        /// <returns></returns>
        public static double GetCurrentTemperature()
        {
            return _currentTemperature.Get();
        }

        /// <summary>
        /// Funckja zwraca obence nastawy PID
        /// </summary>
        /// <returns></returns>
        public static LumelPidValue GetCurrentLumelPidValue()
        {
            return _currentLumelPidValue.Get();
        }

        /// <summary>
        /// Zwraca obecny okres kontrolera
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentEnginePeriod()
        {
            return _currentEnginePeriod.Get();
        }
    }
}
