"""
    Author: Jan Sawicki
    Version: 1.0.0
    Since: 1.1.0
"""

from .factoryexceptions import TkFactoryAlreadyExist
from .mywidgets.EntryTable import EntryTable


class MyFactory():
    """
        Klasa służąca do automatycznego generowania obiektów słżących do tworzenia gui w tkinter.
        Klasa pozwala na zapisywanie obiektów w instancji klasy TkFactory
        Klasa pozwala na łatwe masowe zmienianie wyglądu tworzonych obirktów za pomocą klas styli podawanych do klasy fabryki abstrakcyjnej TkFactory.
    """

    factory = None

    def __init__(self):
        """
            Konstruktor obiektu fabryki - Nie należy go jednak używać bezpośrednio do tworzenia obiektów tej kalsy.
            Jeżeli jeszcze nie ma żadnego obiektu klasy fabryki tworzy nowy.
            Jeżeli już istnieje jakiś obiekt fabryki zwraca istniejący obiekt.
            Tworzone pola:
                self.count (int): licznik ilości stworzonych już przez fabrykę widgetów pomaga przy tworzeniu automatycznych nazw zmiennych
        """
        # Sprawdzanie czy nie istnieje już jakiś obiekt fabryki
        if MyFactory.factory is not None:
            self = MyFactory.factory
        # Tworzenie zmiennych
        self.count = 0

# global class functions
# ----------------------------------------------------------------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------------------------------------------------------------

    def getFactory():
        """
            Funkcja służąca do otrzymywania obiektów tej klasy.
        """
        # Sprawdzanie czy nie istnieje już jakiś obiekt fabryki
        if MyFactory.factory is not None:
            return MyFactory.factory
        MyFactory.factory = MyFactory()
        # Tworzenie obiektu
        return MyFactory.factory

    def __str__(self):
        """
            Funkcja zapisująca podstawowe informacje o obiekcie do łańcucha znaków i zwracająca ten łańcuch pozwalając go wypisać na ekranie.
            Przykładowy wynik działania funkcji:
                "TkFactory: syles=5, widgets=17"
            Returns:
                Opis obiektu
        """
        return "MyFactory: created=%i" % self.count

# my widegts generating functions - container widgets
# ----------------------------------------------------------------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------------------------------------------------------------

# my widegts generating functions - element widgets
# ----------------------------------------------------------------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------------------------------------------------------------

    def getEntryTable(self, parent, col, row, justify="left", width="auto", labels=True, labelsJustify="left", labelsWidth="auto", style="auto", name="none", prefix=""):
        """
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                col (int): Liczba kolumn które ma zawierać tabela
                row (int): Liczba wierszy które ma zawierać tabela
                justify (str): Justowanie takstu - left/right/center
                width (int): Szerokość komurek tabeli
                labels (True/False): Flaga czy dodawać etykietki z opisami rzędów i kolumn
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpis tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Frame, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Frame): Jeżeli nie podano nazwy widgetu

            Raises:
                factoryexceptions.TkFactoryAlreadyExist: Błąd zwracany w przypadku podania nazwy dla obiektu która już wcześniej była użyta

        """
        # Zwiększanie licznika tworzonych obiektów i generowanie domyślnej nazwy widgetu jeżeli potrzebna
        self.count += 1
        if name == "auto":
            new_name = "%swidget%i" % (prefix, self.count)
        if name == "none":
            pass
        else:
            if name in self.widgets:
                raise TkFactoryAlreadyExist("Widget o podanej nazwie: \'%s\' jest już wczśniej utworzony", 0)
            new_name = name

        # Tworzenie obiektu
        new_widget = EntryTable(parent, col, row, justify="left", width="auto", labels=True, labelsJustify="left", labelsWidth="auto", style="auto")

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget
