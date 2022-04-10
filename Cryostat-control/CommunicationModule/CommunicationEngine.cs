using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

using Piecyk.CommunicationModule;
using Piecyk.GlobalFunctions;
using Piecyk.GlobalDataTypes;

namespace Piecyk.CommunicationModule
{
    /// <summary>
    /// Klasa odpowiadająca za obsługę zapytań komunikacji z urządzeniem i przsyłanie odpowiedzi zwrotnych do urządzenia.
    /// Działa jako silnik komunikacyjny uruchamiany w osobnym wątku.
    /// </summary>
    class CommunicationEngine
    {
        /// <summary>
        /// Kolejka komend wejściowych do silnika
        /// </summary>
        public static ConcurrentQueue<CommunicationInData> InQueue = new ConcurrentQueue<CommunicationInData>();
        /// <summary>
        /// Kolejka danych wyjściowych z silnika
        /// </summary>
        public static ConcurrentQueue<CommunicationOutData> OutQueue = new ConcurrentQueue<CommunicationOutData>();
        /// <summary>
        /// Flaga zawierająca informację o obecnej temperaturtrze pieca
        /// </summary>
        public static InterlockedProperty<ushort> CurrentTmperature = new InterlockedProperty<ushort>(0);

        /// <summary>
        /// Parametr informujący czy pętla jest aktywna.
        /// </summary>
        public static InterlockedProperty<bool> IsEngineActive = new InterlockedProperty<bool>(false);

        /// <summary>
        /// Pętla wątku do komunikacji z urządzeniem zewnętrznym.
        /// </summary>
        /// <param name="parameter">Obiekt inicjalizujący typu CommunicationEngineInitData</param>
        public static void communicationLoop(object parameter)
        {
            // Zmienne pomocnicze
            ushort[] PIDParameters = new ushort[3];
            ushort SetTemperature = 0;
            ushort Temperature = 0;
            CommunicationInData InData = new CommunicationInData();
            bool flag = false;
            int readMultiplerFrequencyFlag = 0;

            // Tworzenie obiektu RS485Protocol. Obiekt utworzony poza try aby można było go potem zamknąć niezależnie od pętli.
            RS485Protocol protocol = RS485Protocol.GetRS485Protocol();

            // Czyszczenie danych wejściowych(jeżeli zostały po poprzedniej istancji komunikacji).
            while (!InQueue.IsEmpty) InQueue.TryDequeue(out InData);
            InData = new CommunicationInData();

            try
            {
                // Oznaczenie aktywacji wątku
                IsEngineActive.Set(true);

                // Wczytywanie parametrów inicjalizujących
                CommunicationEngineInitData InitData = parameter as CommunicationEngineInitData;
                long EngineFrequency = InitData.EngineFrequency;
                int EngineReadMultipler = InitData.EngineReadMultipler;
                // Otwieranie połączenia obiektu do komunikacji
                protocol.connectToPort(InitData.COMPort, InitData.Baudrate, InitData.DeviceAddress);

                // Czytanie parametrów PID aby je poznać na start
                PIDParameters[0] = protocol.ReadRegister(4043);
                PIDParameters[1] = protocol.ReadRegister(4044);
                PIDParameters[2] = protocol.ReadRegister(4045);
                SetTemperature = protocol.ReadRegister(4084);
                // Wykonywanie początkowego odczytu temperatury
                Temperature = protocol.ReadRegister(4006);
                // Ustawianie flagi wyjściowej
                CurrentTmperature.Set(Temperature);

                // Główna pętla wątku
                while (true)
                {
                    // Pomiar czasu rozpoczęcia pętli
                    Stopwatch watch = Stopwatch.StartNew();

                    // Zwiększenie licznika przerwy odczytu
                    readMultiplerFrequencyFlag++;

                    // Wykonywanie odczytu parametrów obiektu analizowanego
                    if (readMultiplerFrequencyFlag >= EngineReadMultipler)
                    {
                        // Zerowanie flagi
                        readMultiplerFrequencyFlag = 0;
                        // Wykonywanie odczytu
                        Temperature = protocol.ReadRegister(4006);
                        // Ustawianie flagi wyjściowej
                        CurrentTmperature.Set(Temperature);
                        // Zwracenie wyniku pomiaru
                        OutQueue.Enqueue(new CommunicationOutData(PIDParameters, SetTemperature, Temperature, DateTime.Now));

                        // Wypisywanie do konsoli
                        LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", $"Temperatura pieca = {Temperature}"));
                    }

                    // Sprawdzenie czy nie ma komendy do wykonania
                    while (!InQueue.IsEmpty)
                    {
                        // Wczytywanie komendy
                        flag = false;
                        while (!flag) flag = InQueue.TryDequeue(out InData);

                        if(InData.EngineCommand.Contains("Ignore"))
                        {
                            // Brak działań XD
                            ;
                        }
                        else if (InData.EngineCommand.Contains("ShutDown"))
                        {
                            protocol.ClosePort();
                            IsEngineActive.Set(false);
                            return;
                        }
                        else if (InData.EngineCommand.Contains("SetParameters"))
                        {
                            if ((bool)InData.CommandParameters[0])
                            {
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4043, (ushort)InData.CommandParameters[4]);
                                PIDParameters[0] = (ushort)InData.CommandParameters[4];
                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Ustawiono parametr P na wartość " + ((ushort)InData.CommandParameters[4]).ToString() ));
                            }
                            if ((bool)InData.CommandParameters[1])
                            {
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4044, (ushort)InData.CommandParameters[5]);
                                PIDParameters[1] = (ushort)InData.CommandParameters[5];
                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Ustawiono parametr I na wartość " + ((ushort)InData.CommandParameters[5]).ToString()));
                            }
                            if ((bool)InData.CommandParameters[2])
                            {
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4045, (ushort)InData.CommandParameters[6]);
                                PIDParameters[2] = (ushort)InData.CommandParameters[6];
                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Ustawiono parametr D na wartość " + ((ushort)InData.CommandParameters[6]).ToString()));
                            }
                            if ((bool)InData.CommandParameters[3])
                            {
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4084, (ushort)InData.CommandParameters[7]);
                                SetTemperature = (ushort)InData.CommandParameters[7];
                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Ustawiono parametr SetT na wartość " + ((ushort)InData.CommandParameters[7]).ToString()));
                            }
                        }
                        else if (InData.EngineCommand.Contains("RunProgramCommand"))
                        {
                            // Zmienianie okresu pracy silnika
                            if (InData.ProgramCommand.Contains("settings.LumelEnginePeriod"))
                            {
                                EngineFrequency = (int) InData.CommandParameters[0];
                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Ustawiono EngineFrequency na wartość " + ((int)InData.CommandParameters[0]).ToString()));
                            }
                            // Zmienianie częstości wykonywania pomiaru
                            else if (InData.ProgramCommand.Contains("settings.LumelEngineReadMultipler"))
                            {
                                EngineReadMultipler = (int) InData.CommandParameters[0];
                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Ustawiono SetEngineReadMultipler na wartość " + ((int)InData.CommandParameters[0]).ToString()));
                            }
                            // Zmienianie histerezy
                            else if (InData.ProgramCommand.Contains("settings.Hysteresis"))
                            {
                                // Konwersja danych
                                ushort NewHysteresis = ThreadSafeGlobalFunctions.ConvertTemperatureParameterDoubleToUshort((float)InData.CommandParameters[0]);
                                // Ustawianie parametru
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4036, NewHysteresis);
                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Ustawiono SetEngineReadMultipler na wartość " + ((float)InData.CommandParameters[0]).ToString()));
                            }
                            // Zmienianie wartości strefy rozsunięcia
                            else if (InData.ProgramCommand.Contains("settings.SpreadZone"))
                            {
                                // Konwersja danych
                                ushort NewSpreadZone = ThreadSafeGlobalFunctions.ConvertTemperatureParameterDoubleToUshort((float)InData.CommandParameters[0]);
                                // Ustawianie parametru
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4060, NewSpreadZone);
                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Ustawiono SetEngineReadMultipler na wartość " + ((float)InData.CommandParameters[0]).ToString()));
                            }
                            // Zmiana nastawów PID o zadane wartości
                            else if (InData.ProgramCommand.Contains("pid.change"))
                            {
                                // Wczytywanie zmiennych
                                int[] newPID = new int[3];
                                newPID[0] = PIDParameters[0] + (short)InData.CommandParameters[0];
                                newPID[1] = PIDParameters[1] + (short)InData.CommandParameters[1];
                                newPID[2] = PIDParameters[2] + (short)InData.CommandParameters[2];

                                // Kumulowanie danych dla loga
                                string OldPidInfo = "(" + ((short)InData.CommandParameters[0]).ToString() + ", " + ((short)InData.CommandParameters[1]).ToString() + ", " + ((short)InData.CommandParameters[2]).ToString() + ")";

                                // Kontrola ograniczeniczeń
                                for (int i=0; i<2; i++)
                                {
                                    const int maxPIDSetting = 9999; // Maksymalna wartość nastawów PID wspierana przez LUMEL
                                    if (newPID[i] > maxPIDSetting) newPID[i] = maxPIDSetting;
                                    else if (newPID[i] < 0) newPID[i] = 0;
                                }

                                // Zapisywanie danych do rejestrów
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4043, (ushort)newPID[0]);
                                PIDParameters[0] = (ushort)newPID[0];
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4044, (ushort)newPID[1]);
                                PIDParameters[1] = (ushort)newPID[1];
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4045, (ushort)newPID[2]);
                                PIDParameters[2] = (ushort)newPID[2];

                                // Wysyłanie loga
                                string NewPidInfo = "(" + ((short)InData.CommandParameters[0]).ToString() + ", " + ((short)InData.CommandParameters[1]).ToString() + ", " + ((short)InData.CommandParameters[2]).ToString() + ")";
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Zmieniono nastaw PID z " + OldPidInfo + " na " + NewPidInfo));
                            }
                            // Ustawianie temperatury na obecną
                            else if (InData.ProgramCommand.Contains("temperature.toPresent"))
                            {
                                // Wykonywanie pomiaru
                                ushort tmpCurrentTemperature = protocol.ReadRegister(4006);
                                // Przepisanie pomiaru do rejestru nastawu
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4084, tmpCurrentTemperature);

                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Zmieniono SetT na odczyt temperatury obecnej w piecu z " + SetTemperature.ToString() + " na " + tmpCurrentTemperature.ToString()));

                                SetTemperature = tmpCurrentTemperature;
                            }
                            // Zmiana temperatury o zadaną wartość
                            else if (InData.ProgramCommand.Contains("temperature.change"))
                            {
                                // Obliczanie zadanego nastawu
                                double newTemperatureDouble = ThreadSafeGlobalFunctions.ConvertTemperatureUshortToDouble(SetTemperature) + (float) InData.CommandParameters[0];
                                ushort newTemperatureUshort = ThreadSafeGlobalFunctions.ConvertTemperatureDoubleToUshort(newTemperatureDouble);
                                // Zapis temperatury
                                flag = false;
                                while (!flag) flag = protocol.WriteToRegister(4084  , newTemperatureUshort);

                                // Wysyłanie loga
                                LogsOutputLoop.LogsQueue.Enqueue(new LogTemplate("CommunicationEngine", "Zmieniono SetT z " + SetTemperature.ToString() + " na " + newTemperatureUshort.ToString()));

                                SetTemperature = newTemperatureUshort;
                            }
                        }
                    }

                    // Przerwa wywołana częstotliwością silnika
                    long LoopExecutionTime = watch.ElapsedMilliseconds;
                    if(LoopExecutionTime < EngineFrequency) // Sprawdzenie czy pętla nie trwała dłużej niż zadana wartość
                    {
                        Thread.Sleep((int)(EngineFrequency - LoopExecutionTime));
                    }
                    // W innym przypadku brak wykonania przerwy w pętli. Pętla trwała za długo.
                }
            }
            catch (ThreadAbortException exc)
            {
                Console.WriteLine("Zamknięcie wątku #1");
                Console.WriteLine("Wyjątek w wątku komunikacji: ( " + exc.Message + " )"); // Obsługa wyjątku przerwania działania wątku przez Thread.Abort()
                protocol.ClosePort();
                IsEngineActive.Set(false);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Zamknięcie wątku #2");
                Console.WriteLine("Wyjątek w wątku komunikacji: ( " + exc.Message + " )"); // Pozostałe wyjątki
                new MyMaterialMessageBox("W wyniku krytycznego błędu nastąpiło zerwanie połączenia z urządzeniem", MyMaterialMessageBox.MessageBoxType.Error, MyMaterialMessageBox.MessageBoxButtons.Ok).ShowDialog();
                protocol.ClosePort();
                IsEngineActive.Set(false);
            }
        }
    }
}
