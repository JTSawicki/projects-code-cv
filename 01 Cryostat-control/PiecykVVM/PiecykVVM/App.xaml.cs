using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows;
using Serilog;
using Serilog.Exceptions;
using PiecykVVM.Windows;
using PiecykM.SaveProvider;
using LabServices.Lumel;
using LabServices.MFIA;

namespace PiecykVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly int _aplicationVersion = 2;
        private readonly int _aplicationSubVersion = 1;
        protected override void OnStartup(StartupEventArgs e)
        {
            // Inicjalizacja Loggera(musi być pierwsza)
            SetupSerilogLogger();
            Log.Information("Begining of aplication initialization");
            Log.Information($"Aplication version: {_aplicationVersion}.{_aplicationSubVersion}");
            try
            {
                // Tworzenie pierwszego okna
                ConnectWindow connectWindow = new ConnectWindow();
                connectWindow.Show();
                // Inicjalizacja bazowa(wymagana)
                base.OnStartup(e);
            }
            catch
            {
                Log.Fatal("Aplication fail to start");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Aplication ending");
            // Wyłączanie kontrolerów
            if (LumelController.IsActive())
                LumelController.StopController();
            if (MFIAController.IsActive())
                MFIAController.StopController();
            // Oczekiwanie na zamknięcie wątków kontrolerów
            while (LumelController.IsActive() ||
                MFIAController.IsActive())
            {
                Thread.Sleep(10);
            }
            // Wyłączanie Loggera
            Log.CloseAndFlush();
            // Zakończenie bazowe(wymagane)
            base.OnExit(e);
        }

        /// <summary>
        /// Funkcja inicjalizuje logger Serilog dla całego programu
        /// </summary>
        private void SetupSerilogLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .MinimumLevel.Verbose()
                .WriteTo.Debug(outputTemplate: "[Serilog] {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception} {Properties:j}{NewLine}")
                .WriteTo.File(formatter: new Serilog.Formatting.Json.JsonFormatter(), path: SaveManager.GetLogFilePath("json"), restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
        }
    }
}
