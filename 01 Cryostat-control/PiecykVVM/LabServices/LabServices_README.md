# Krótkie wprowadzenie


Celem biblioteki jest zapewnienie stabilnego i łatwego w użyciu API do komunikacji z następującą listą urządzeń:
 - Lumel Re72
 - Zurich Instruments MFIA

W celu użycia biblioteki należy dołączyć projekt biblioteki do programu a następnie dołączyć odwołania do oczekiwanych projektów.


## Zasoby wspólne (namespace DataTemplates & Exceptions)
Są to zasoby wykorzystywane przez kontrolery urządzeń. Zazwyczaj nie ma potrzeby wykorzystywania tych zasobów przy korzystaniu z tej biblioteki. Wyjątkiem jest łapanie wyjątków rzucanych przez funkcje.

Wyjątki:
 - **FailToConnectException** - Błąd nieudanej próby połączenia z urządzeniem
 - **NotConnectedException** - Błąd braku połączenia z urządzeniem
 - **ToMuchDataException** - Błąd wysłania zbyt dużej ilości danych do funkcji


## Lumel Re72 (namespace Lumel)
Zestaw api dla sterownika temperatury Lumel Re72.

### Dane o stanie sterownika
Klasa LumelStore.cs - proszę zajrzeć do komentarzy publicznego kodu.

### Połączenie synchroniczne

Klasa LumelInterface.cs - proszę zajrzeć do komentarzy publicznego kodu.

### Połączenie asynchtoniczne

Nawiązywanie połączenia: \n
&emsp;void LumelController.StartController(LumelControllerInitData)

Zrywanie połączenia(należy poczekać na zmianę flagi sterownika przed zakończeniem programu): \n
&emsp;void LumelController.StopController()

Flaga aktywności sterownika: \n
&emsp;bool LumelController.IsActive()

Wysyłanie komendy do sterownika: \n
&emsp;void LumelController.PushCommand(LumelControllerCommands, List<object>? param = null)

Możliwe do wysłania komendy i ich parametry opisane w komentarzach do LumelControllerCommands w pliku *LumelControllerCommands.cs*.


## Zurich Instruments MFIA (namespace MFIA)
Zestaw api dla miernika impedancji Zurich Instruments MFIA.

W celu użycia sterownika trzeba zainstalować oprogramowanie Zurich Instruments LabOne, zaimportować odpowiedni plik .dll sterownika znajdującego się w folderze API w miejscu instalacji.
Z ciężkich do wyjaśnienia przyczyn 😲 trzeba również skopiować wszystkie pliki .dll sterownika LabOne dla .Net do folderu głównego biblioteki i oznaczyć jako "Zawartość" + "Kopiuj zawsze".

### Dane o pomiarach
Klasa MFIAStore.cs - proszę zajrzeć do komentarzy publicznego kodu.

### Połączenie synchroniczne

Klasa MFIAInterface.cs - proszę zajrzeć do komentarzy publicznego kodu.

### Połączenie asynchtoniczne

Nawiązywanie połączenia: \n
&emsp;void MFIAController.StartController()

Zrywanie połączenia(należy poczekać na zmianę flagi sterownika przed zakończeniem programu): \n
&emsp;void MFIAController.StopController()

Flaga aktywności sterownika: \n
&emsp;bool MFIAController.IsActive()

Wysyłanie komendy do sterownika: \n
&emsp;void MFIAController.PushCommand(MFIAControllerCommands, List<object>? param = null)

Możliwe do wysłania komendy i ich parametry opisane w komentarzach do MFIAControllerCommands w pliku *MFIAControllerCommands.cs*.