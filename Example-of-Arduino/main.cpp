#include <Arduino.h>

//Definicje pinów
const uint8_t PinEncoderA = 3;
const uint8_t PinEncoderB = 4;
const uint8_t PinEncoderC = 2; //Encoder Button
const uint8_t PinLED1 = 9;
const uint8_t PinLED2 = 10;
const uint8_t PinBuzzer = A3;
const uint8_t PinBatteryVoltageControl = A0; //18;
const uint8_t PinPowerLED = 0;

//Zmienne enkodera
bool encoderABeginState = 0;                  //Stan pinu A enkodera przy spoczynku - inicjowany w setup()
bool encoderACurrentState = 0;                //Stan pinu A enkodera w chwili obecnej(ostatniego odczytania)
unsigned long encoderALastTime = 0;                 //Moment ostatniego odczytu czasu przez funkcję analizy obrotu enkodera
unsigned long encoderACurrentTime = 0;              //Obecny moment odczytu czasu przez funkcję analizy obrotu enkodera
const unsigned long encoderAMinDelayTime = 2;        //Minimalny moment odstępu czasu między załączeniami działania funkcji analizy obrotu enkodera
unsigned long encoderButtonLastTime = 0;                 //Moment ostatniego odczytu czasu przez funkcję analizy naciśnięcia przycisku enkodera
unsigned long encoderButtonCurrentTime = 0;              //Obecny moment odczytu czasu przez funkcję analizy naciśnięcia przycisku enkodera
const unsigned long encoderButtonMinDelayTime = 10;        //Minimalny moment odstępu czasu między załączeniami działania funkcji analizy naciśnięcia przycisku enkodera

//Zmienne strowania LED
const unsigned int levelMultipiler = 10;  //O ile należy zmienić LEDLevel na jeden impuls enkodera
volatile bool LED2State = true;                    //Włączony LED = true, wyłączony LED = false
volatile int LEDLevel = 255;                         //min = 0, max = 255

//Zmienne alarmu
int batteryLevel = 0;                     //Zbadany poziom napięcia baterii
int batteryAlarmLevel = 670;                //Poziom włączenia alarmu
bool alarm = false;                       //Zmienna określająca czy alarm jest włączony

//Zmienne pomocnicze
int tmp = 0;                              //Pomocnicza zmienna obliczeniowa

//Definicje funkcji
void EncoderButtonAction();
void EncoderTurnAction();
void RefreshOutput();
void ChangeLEDLevel(int levelChange);

void setup() {
  //Ustawianie roli pinów, pin PinBatteryVoltageControl nie jest ustalany ponieważ pełni on rolę wejścia analogowego
  pinMode(PinEncoderA, INPUT);
  pinMode(PinEncoderB, INPUT);
  pinMode(PinEncoderC, INPUT);
  pinMode(PinLED1, OUTPUT);
  pinMode(PinLED2, OUTPUT);
  pinMode(PinBuzzer, OUTPUT);
  pinMode(PinPowerLED, OUTPUT);

  //Przerwanie dla obrotu enkodera
  attachInterrupt(digitalPinToInterrupt(PinEncoderA), EncoderTurnAction, CHANGE);
  //Przerwanie dla naciśnięcia enkodera
  attachInterrupt(digitalPinToInterrupt(PinEncoderC), EncoderButtonAction, RISING);

  //Inicjowanie zmiennej wartością odczytaną
  encoderABeginState = digitalRead(PinEncoderA);

  //Inicjowanie stanów wyjść arduino
  RefreshOutput(); //Inicjowanie świecenia lampki
  digitalWrite(PinBuzzer, LOW); //Inicjowanie buzzera(brak wydawanego dźwięku)
  digitalWrite(PinPowerLED, HIGH); //Włączanie LED sygnalizującego włączenie
  
  //Lampka jest gotowa do działania
}

void loop() {
  for(int j=0; j<20; j++) {
  delay(100);
  RefreshOutput();
  }
  if(batteryAlarmLevel >= analogRead(PinBatteryVoltageControl)) {
    digitalWrite(PinBuzzer, HIGH);
  }
}

/**
 * @brief Funkcja zmieniająca wartość poziomu jasności świecenia
 * Jasność jest sterowana w zakresie 0(min) - 255(max). Zmiana następuje o levelChange * levelMultipiler.
 * @param levelChange Poziom wprowadzanej zmiany
 */
void ChangeLEDLevel(int levelChange){
  LEDLevel = LEDLevel + levelChange * levelMultipiler;
  if(LEDLevel > 255) LEDLevel = 255;
  else if(LEDLevel < 0) LEDLevel = 0;
  return;
}

/**
 * @brief Funkcja oświeżająca poziom jasności lampki
 * Funkcja ustawia jasność świecenia lampki przez wysłanie odpowiedniego sterowania PWM na sterownik MOSFET oraz czy drugi panel ma być zaświecony.
 * Wartości jasności są odwrucone o 255 ponieważ w układzie zastosowany został sterownik z inwerterem.
 */
void RefreshOutput(){
  //Zmienna tmp jest użyta jako typczasowe miejsce zapisu poziomu sygnału PWM
  tmp = 255 - LEDLevel;

  if(LED2State){  //Oba LEDy ulegają zaświeceniu
    if(LEDLevel == 0){
      digitalWrite(PinLED1, HIGH);
      digitalWrite(PinLED2, HIGH);
    } else if(LEDLevel == 255) {
      digitalWrite(PinLED1, LOW);
      digitalWrite(PinLED2, LOW);
    } else {
      analogWrite(PinLED1, tmp);
      analogWrite(PinLED2, tmp);
    }
  } else {  //Tylko jeden LED podlega zaświecaniu
    digitalWrite(PinLED2, HIGH);
    if(LEDLevel == 0){
      digitalWrite(PinLED1, HIGH);
    } else if(LEDLevel == 255) {
      digitalWrite(PinLED1, LOW);
    } else {
      analogWrite(PinLED1, tmp);
    }
  }
}

/**
 * @brief Funkcja odpowiadająca za obsługę działania obrotu enkodera
 * 
 */
void EncoderTurnAction() {
  encoderACurrentState = digitalRead(PinEncoderA); //Odczyt obencego stanu pinu
  encoderACurrentTime = millis(); //Odczyt obecnego czasu
  //Jeżeli został spełniony minimalny warunke działania zmiany w czasie oraz nastąpiła zmiana zostaje uruchomione działanie funkcji
  if(encoderACurrentTime - encoderAMinDelayTime >= encoderALastTime && encoderABeginState != encoderACurrentState) {
    // Jeżeli stan pinu B jest różny od stany pinu A enkodera to znaczy że jest on obracany w prawo
    if(digitalRead(PinEncoderB) != encoderACurrentState) {
      //Obrót w prawo
      ChangeLEDLevel(-1);
    } else {
      //Obrót w lewo
      ChangeLEDLevel(1);
    }
    //Zmiana wartości czasu ostatniego odczytu
    encoderALastTime = encoderACurrentTime;
  }
}

/**
 * @brief Funkcja odpowiadająca za obsługę wciśnięcia przycisku
 * 
 */
void EncoderButtonAction() {
  encoderButtonCurrentTime = millis(); // Reads the "current" time
  if(encoderButtonCurrentTime - encoderButtonMinDelayTime >= encoderButtonLastTime){
    LED2State = !LED2State;
    encoderButtonLastTime = encoderButtonCurrentTime;
  }
}