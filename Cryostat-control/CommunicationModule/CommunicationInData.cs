using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piecyk.CommunicationModule
{
    class CommunicationInData
    {
        /// <summary>
        /// Zmienna określająca jaką czynność powinien wykonać silnik komunikacji po wczytaniu tego bloku danych wejściowych.
        /// Dostępne polecenia:
        ///     - "SetParameters"           - Silnik powinien ustawić parametry wybrane w zmiennych ChangePIDParameters oraz ChangeSetTemperature
        ///     - "RunProgramCommand"      - Ustawianie częstotliwości silnika (jednostka sekundy) (przykład "EngineFrequency 300")
        ///     - "Ignore"                  - Silnik powinien zignorować tą komendę
        ///     - "ShutDown"                - Silnik powinien zakończyć połączenie z sterownikiem się wyłączyć
        /// </summary>
        public string EngineCommand { get; private set; }

        /// <summary>
        /// Zmienna przechowywująca komendę programową jeżeli jest wysłana do silnika
        /// Dostępne komendy:
        ///     - "SetEngineFrequency"      - Ustawianie częstotliwości silnika (jednostka sekundy) (przykład "EngineFrequency 300")
        ///         Parametry: (int)
        ///     - "SetEngineReadMultipler"  - Ustawienie współczyninika jak często wykonywać odczyt temperatury z urządzenia (Multipler x Frequency) (przykład "EngineReadMultipler 10")
        ///         Parametry: (int)
        /// </summary>
        public string ProgramCommand { get; private set; }
        /// <summary>
        /// Lista parametrów komendy.
        /// Parametr powinien być zapisany w formie liczba, gdzie liczba jest rzutowana na typ object.
        /// Przykład: (object) 12
        /// Wspierane typy(w zależności od komendy):
        ///     - "int"
        ///     - "uint"
        ///     - "short"
        ///     - "ushort"
        /// </summary>
        public List<object> CommandParameters { get; private set; }

        /// <summary>
        /// Domyślny konstruktor obiektu do ustawiania parametrów PID i SetT
        /// </summary>
        /// <param name="ChangePIDParameters_">Czy należy zmienić wartość danego nastawu PID. Oczekiwany rozmiar tablicy == 3</param>
        /// <param name="PIDParameters_">Nastawy PID. Oczekiwany rozmiar tablicy == 3</param>
        /// <param name="ChangeSetTemperature_">Czy należy zmienić wartość nastawu temperatury</param>
        /// <param name="SetTemperature_">Nastaw temperatury</param>
        /// <param name="EngineCommand_">Polecenie dla silnika komunikacji.</param>
        public CommunicationInData(bool[] ChangePIDParameters_, ushort[] PIDParameters_, bool ChangeSetTemperature_, ushort SetTemperature_, string EngineCommand_ = "SetParameters")
        {
            // Tworzenie listy parametrów wysyłanych do silnika
            List<object> CommandParameterList = new List<object>();
            for(int i=0; i<3; i++) CommandParameterList.Add(ChangePIDParameters_[i]);
            CommandParameterList.Add(ChangeSetTemperature_);
            for (int i=0; i<3; i++) CommandParameterList.Add(PIDParameters_[i]);
            CommandParameterList.Add(SetTemperature_);

            ObjectSetup(EngineCommand_, "", CommandParameterList);
        }

        /// <summary>
        /// Domyślny konstruktor do wysyłania komendy programowej.
        /// </summary>
        /// <param name="ProgramCommand_"></param>
        /// <param name="CommandParameters_"></param>
        public CommunicationInData(string ProgramCommand_, List<object> CommandParameters_)
        {
            ObjectSetup("RunProgramCommand", ProgramCommand_, CommandParameters_);
        }

        /// <summary>
        /// Konstruktor służący do tymczasowego inicjowania zmiennych oraz wysyłania komend. Nie należy go używać do inicjowania obiektu przesyłającego parametry PID.
        /// </summary>
        public CommunicationInData(string EngineCommand_ = "Ignore")
        {
            ObjectSetup(EngineCommand_, "", null);
        }

        /// <summary>
        /// Funkcja stanowiąca uniwersalny interfejs dla konstruktorów. Do opisu parametrów należy odwołać się do domyślnego konstruktora.
        /// </summary>
        private void ObjectSetup(string EngineCommand_, string ProgramCommand_, List<object> CommandParameters_)
        {
            EngineCommand = EngineCommand_;

            ProgramCommand = ProgramCommand_;
            if (CommandParameters_ != null) CommandParameters = new List<object>(CommandParameters_);
            else CommandParameters = new List<object>();
        }
    }
}
