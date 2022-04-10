using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using System.IO;
using System.Threading;
using Piecyk.GlobalFunctions;

namespace Piecyk.Tests
{
    [Collection("Sequential")]
    public class LogsOutputLoop_Tests
    {
        // W przypadku nie działania testu warto sprawdzić czy Resources.SettingsPool.DisplayedLogsSources zawiera "Tests"
        [Fact]
        public void Namespace_Function_ScenarioTest()
        {
            string FirstLogContent = "Pierwszy log testowy";
            string SecondLogContent = "Drugi log testowy";

            // Ustawianie wartości wyjścia logów na plik
            Resources.SettingsPool.LoggingOutput = "file";
            // Uruchamianie wątku logowania danych
            Thread loggingEngine = new Thread(LogsOutputLoop.LoggingLoop);
            loggingEngine.Priority = ThreadPriority.Lowest;
            loggingEngine.Start();
            // Wysyłanie danych do logowania
            LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("Tests", FirstLogContent));
            LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("Tests", SecondLogContent));
            // Przerwa na upewnienie się że dane zostały odebrane
            Thread.Sleep(Resources.SettingsPool.LoggingLoopPeriod * 2);
            // Pobieranie nazwy pliku do którego są wysyłane dane
            string TestOutputFile = LogsOutputLoop.OutputFileName.Get();
            // Wyłączanie wątku z przerwą na upewnienie się że wątek został wyłączony ze względu na używanie pliku
            LogsOutputLoop.IsEngineActive.Set(false);
            Thread.Sleep(Resources.SettingsPool.LoggingLoopPeriod * 2);

            // Sprawdzenie czy pobrana ścieżka do pliku została ustawiona przez wątek logowania
            Assert.NotEqual("", TestOutputFile);

            // Sprawdzanie czy logi zostały zapisane
            string ActualTextFromFile = File.ReadAllText(FileManager.AppFolder_Logs + FileManager.DirectorySeparator + TestOutputFile);

            Assert.Contains(FirstLogContent, ActualTextFromFile);
            Assert.Contains(SecondLogContent, ActualTextFromFile);
        }
    }
}
