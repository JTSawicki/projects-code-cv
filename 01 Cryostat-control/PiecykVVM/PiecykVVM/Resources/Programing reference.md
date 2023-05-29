# Opis użytkowania skryptu *.PCode*

## Wstęp

Skrypty PCode są prostymi programami służącymi do kontrolowania urządzań za pomocą programu Piecyk.

Składnia języka jest prosta. Każda komenda składa się z słowa kluczowego określającego grupę poleceń do których użytkownik się odwołuje oraz polecenia wraz z parametrami otoczonymi nawiasami i rozdzielonymi przecinkami.

W jednej linii może się znajdować jedynie jedno polecenie.

Skrypt ignoruje puste znaki, a wszystko co znajduje się w linii po znaku '#' traktowane jest jako komentarz i nie podlega interpretacji.

Przykład prostego programu:


> lumel.setTemperature(12)\
func.repeat(2)\
&emsp;mfia.sweep\
&emsp;lumel.changeTemperature(2)\
&emsp;func.wait(2000)\
func.end

Skrypt automatycznie zapisuje wykonane pomiary przez użytkownika w wybranym w menu *Parametry pomiaru* pliku.
Format zapisu zależy od wybranego rozszerzenia pliku zapisu. Może to być plik *.cvs* lub *.tsv*. W przypadku wybrania pliku o innym rozszerzeniu zostanie użyty format kodowania *.tsv*.

## Grupa funkcyjna - func

Jest to specjalna grupa poleceń. Nie kontroluje ona żadnego fizycznego sprzętu, a polecenia są wykonywane bezpośrednio w kodzie pętli głównej interpretera.

> **end()**\
Parametry: brak\
>Działanie:\
&emsp;Zamyka blok pętli

> **longWait(godziny, minuty, sekundy)**\
Parametry:
> - int godziny< 0, - > - Ilość godzin
> - int minuty< 0, 60 > - Ilość minut
> - int sekundy< 0, 60 > - Ilość sekund
>
>Działanie:\
&emsp;Wstrzymuje działanie programu na określony czas

> **repeat(times)**\
Parametry:
> - int times< 0, - > - Ilość powtórzeń
>
> Działanie:\
&emsp;Pętla wykonująca kod zawarty w bloku zamkniętym za pomocą polecenia func.end określoną ilość razy.

> **wait(milisekundy)**\
Parametry:
> - int milisekundy< 0, - > - 
>
>Działanie:\
&emsp;Wstrzymuje działanie programu na określony czas

## Opis sprzętowych grup poleceń

### Grupa lumel
Grupa poleceń odpowiadających za sterowanie nastawami sterownika temperatury LumelRe72.

Poza komendami użytkownik może wybrać parametry *Okres działania kontrolera* oraz *Częstość pomiaru temperatury* w menu *Parametry pomiaru*.

Parametr *Okres działania kontrolera* określa z jakim okresem kontroler będzie starał się wykonywać nową integrację. Wpływa to na średni czas reakcji na komendę. Należy jednak pamiętać że każda operacja połączenia z sterownikiem LumelRe72 zabiera około 30ms. Nie należy dlatego ustawiać tej wartości na zbyt małą bo może to powodować niestabilność rzeczywistego okresu wykonania i powodować gubienie kroków pętli kontrolera. Zalecane jest utrzymanie minimum 200ms jednak program pozwala na obniżenie tej wartości do 100ms.

Parametr *Częstość pomiaru temperatury* określa co ile kroków pętli program ma odczytać wartość temperatury układu. Średnią częstość odczytu można określić jako iloraz *Okres działania kontrolera* * *Częstość pomiaru temperatury*

Można również skorzystać z funkcjonalności *Auto PID*, która powoduje automatyczne ustawianie parametrów PID w zleżności od nastawy temperatury.

> **changeTemperature(zmianaTemperatury)**\
Parametry:
> - double zmianaTemperatury< -, - > - Wartość wprowadzanej zmiany.
>
>Działanie:\
&emsp;Zmienia nastawę temperatury o zadaną wartość. Jeżeli zmiana wykracza poza maksimum lub minimum sterownika Lumel kontroler ograniczy ją do wartości granicznej.

> **setPid(ParametrP, ParametrI, ParametrD)**\
Parametry:
> - ushort ParametrP< 1, 9999 > - Wartość parametru P
> - ushort ParametrI< 0, 9999 > - Wartość parametru I
> - ushort ParametrD< 0, 9999 > - Wartość parametru D
>
>Działanie:\
&emsp;Nastawia nowe parametry pid. Nie robi nic przy włączonym systemie auto pid.

> **setTemperature(temperatura)**\
Parametry:
> - double temperatura< -, - > - Nowa wartość temperatury
>
>Działanie:\
&emsp;Ustawia nową wartość nastawy temperatury. Jeżeli zmiana wykracza poza maksimum lub minimum sterownika Lumel kontroler ograniczy ją do wartości granicznej.

> **setTemperatureToPresent()**\
Parametry: brak\
>
>Działanie:\
&emsp;Zmienia nastawę na obecny odczyt.

> **waitUntilTemperature(temperatura, odchyłka)**\
Parametry:
> - double temperatura< -, - > - Docelowa temperatura
> - double odchyłka< -, - > - Dopuszczalna odchyłka
>
>Działanie:\
&emsp;Oczekuje na osiągnięcie zadanego progu temperatury. Jeżeli jego osiągnięcie(np. temperatura docelowa poniżej obecnej, a nastawa powyżej) jest niemożliwe funkcja przerywa działanie. Dodatkowo funkcja automatycznie odczekuje na początku okres równy 1.5 * *Okres działania kontrolera*.

### Grupa mfia
Grupa poleceń pozwalająca na wykonywanie pomiarów za pomocą mostka pomiarowego impedancji Zurich Instruments MFIA.

Ustawienia parametrów pomiaru użytkownik wybiera w menu *Parametry pomiaru*.

> **sweep()**\
Parametry: brak\
>Działanie:\
&emsp;Zleca wykonanie pomiaru i czeka na jego zakończenie

