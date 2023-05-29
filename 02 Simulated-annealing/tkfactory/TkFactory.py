"""
    Author: Jan Sawicki
    Version: 1.1.2
    Since: 1.0.1
"""

import tkinter as tk
import tkinter.ttk as ttk
from .factoryexceptions import TkFactoryBadId, TkFactoryAlreadyExist


class TkFactory():
    """
        Klasa służąca do automatycznego generowania obiektów słżących do tworzenia gui w tkinter.
        Klasa pozwala na łatwe masowe zmienianie wyglądu tworzonych obirktów za pomocą klas styli.
    """

    factory = None

    def __init__(self):
        """
            Konstruktor obiektu fabryki - Nie należy go jednak używać bezpośrednio do tworzenia obiektów tej kalsy.
            Jeżeli jeszcze nie ma żadnego obiektu klasy fabryki tworzy nowy.
            Jeżeli już istnieje jakiś obiekt fabryki zwraca istniejący obiekt.
            Tworzone pola:
                self.widgets (dic): słownik zawierający wszystkie utworzone i nie skasowane widgety
                self.styles (dic): zawiera wszystkie podawane przy tworzeniu style
                self.count (int): licznik ilości stworzonych już przez fabrykę widgetów pomaga przy tworzeniu automatycznych nazw zmiennych
        """
        # Sprawdzanie czy nie istnieje już jakiś obiekt fabryki
        if TkFactory.factory is not None:
            self = TkFactory.factory
        # Tworzenie zmiennych
        self.widgets = {}
        self.styles = {}
        self.count = 0

# global class functions
# ----------------------------------------------------------------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------------------------------------------------------------

    def getFactory():
        """
            Funkcja służąca do otrzymywania obiektów tej klasy.
        """
        # Sprawdzanie czy nie istnieje już jakiś obiekt fabryki
        if TkFactory.factory is not None:
            return TkFactory.factory
        TkFactory.factory = TkFactory()
        # Tworzenie obiektu
        return TkFactory.factory

    def __str__(self):
        """
            Funkcja zapisująca podstawowe informacje o obiekcie do łańcucha znaków i zwracająca ten łańcuch pozwalając go wypisać na ekranie.
            Przykładowy wynik działania funkcji:
                "TkFactory: syles=5, widgets=17"
            Returns:
                Opis obiektu
        """
        return "TkFactory: styles=%i, widgets=%i, created=%i" % (len(self.styles), len(self.widgets), self.count)

    def getWidget(self, name):
        """
            Funkcja służy do uzyskiwania wcześniej utworzonych obiektów widgetów.  \n
            Args:
                name (str): - Nazwa utworznego wcześniej obiektu który chcemy uzyskać

            Raises:
                factoryexceptions.TkFactoryBadId: Błąd zwracany w przypadku gdy podano niewłaściwe id pobieranego obiektu.
        """
        if name not in self.widgets:
            raise TkFactoryBadId("Podano id nieistniejącego obiektu. id: %s" % str(name))
        return self.widgets[name]

    def addExternalWidget(self, widget, name):
        """
            Dodaje zewnętrzny widget do bazy danych programu.
            Args:
                widget (tkinter.BaseWidget): Dodawany widget
                name (str): - Nazwa dodawanego widgetu

            Raises:
                factoryexceptions.TkFactoryAlreadyExist: Błąd zwracany w przypadku podania nazwy dla obiektu która już wcześniej była użyta
        """
        if name in self.widgets:
                raise TkFactoryAlreadyExist("Widget o podanej nazwie: \'%s\' jest już wczśniej utworzony", 0)
        else:
            self.widgets[name] = widget

    def clear(self):
        """
            Usuwa wszystkie pola słownika widgets które mają wartość None - obiekty zostały już z nich usunięte
        """
        for i in self.widgets:
            if self.widgets[i] is None:
                del self.widgets[i]

    def deleteWidget(self, name):
        """
            Usuwa widget o podanej nazwie. UWAGA!!! Funkcja nie odpakowywuje obiektu z włącznoego gui.
            Args:
                name (str): Nazwa obiektu który chcemy usunąć

            Raises:
                factoryexceptions.TkFactoryBadId: Błąd zwracany w przypadku gdy podano niewłaściwe id usuwanego obiektu.
        """
        # Sprawdzanie czy obiekt istnieje w bazie danych
        if name not in self.widgets:
            raise TkFactoryBadId("Podano id nieistniejącego obiektu. id: %s" % str(name))
        # Usuwanie widgetu
        del self.widgets[name]

# tk widegts generating functions - container widgets
# ----------------------------------------------------------------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------------------------------------------------------------

    def getTk(self, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Tk, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Menu): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Tk()

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getFrame(self, parent, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Frame, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Menu): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Frame(parent)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getLabelFrame(self, parent, text, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                text (str): Napis jaki ma być wyświetlany przez przycisk
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
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
        new_widget = tk.LabelFrame(parent, text=text)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getPanedWindow(self, parent, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.PanedWindow, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.PanedWindow): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.PanedWindow(parent)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getToplevel(self, parent, transient=True, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                transient (bool): Określa czy uruchomione okno ma mieć tą samą ikonę na padku programów co okno wywołujące
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Toplevel, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Toplevel): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Toplevel(parent)

        # Ustawianie ustawień ikony
        if transient:
            new_widget.transient(parent)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

# tk widegts generating functions - element widgets
# ----------------------------------------------------------------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------------------------------------------------------------

    def getButton(self, parent, text, command, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt przycisku.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                text (str): Napis jaki ma być wyświetlany przez przycisk
                command (func): Funkcja która ma być wywołana w momencie naciśnięcia przycisku
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą przy tworzeniu nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Button, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Button): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Button(parent, text=text, command=command)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getCanvas(self, parent, width, height, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                width (int): Szerokość tworzonego okna
                height (int): Wysokość tworzonego okna
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Canvas, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Canvas): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Canvas(parent, width=width, height=height)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getLabel(self, parent, text, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                text (str): Napis jaki ma być wyświetlony
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Label, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Label): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Label(parent, text=text)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getListbox(self, parent, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Listbox, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Listbox): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Listbox(parent)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getMenu(self, parent, tearoff=0, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                tearoff (int - 1/0): Wyświetlanie niepotrzebnego separatora funkcyjnego przy wartości 1. Pozwala on otworzyć menu w nowym oknie
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Menu, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Menu): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Menu(parent, tearoff=tearoff)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getMessage(self, parent, text, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                text (str): Napis jaki ma być wyświetlony
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Message, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Message): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Message(parent, text=text)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getRadiobutton(self, parent, text, var, val, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                text (str): Napis jaki ma być wyświetlony
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Radiobutton, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Radiobutton): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Radiobutton(parent, text=text, variable=var, value=val)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getScale(self, parent, from_, to_, orient=tk.HORIZONTAL, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                orient (tk.HORIZONTAL/tk.VERTICAL): Orientacja suwaka
                from_ (int): Początek skali suwaka
                to_ (int): Koniec skali suwaka
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Scale, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Scale): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Scale(parent, from_=from_, to=to_, orient=orient)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getScrollbar(self, parent, orient=tk.VERTICAL, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                orient (tk.HORIZONTAL/tk.VERTICAL): Orientacja suwaka
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Scrollbar, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Scrollbar): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Scrollbar(parent, orient=orient)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getSpinbox(self, parent, from_, to_, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                from_ (int): Początek skali suwaka
                to_ (int): Koniec skali suwaka
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Spinbox, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Spinbox): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Spinbox(parent, from_=from_, to=to_)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getText(self, parent, background="white", style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                background (str): Kolor tła pola textowego
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Text, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Text): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Text(parent, background=background)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getEntry(self, parent, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.Entry, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.Entry): Jeżeli nie podano nazwy widgetu

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
        new_widget = tk.Entry(parent)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

# ttk widegts generating functions - container widgets
# ----------------------------------------------------------------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------------------------------------------------------------

    def getNotebook(self, parent, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.ttk.Notebook, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.ttk.Notebook): Jeżeli nie podano nazwy widgetu

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
        new_widget = ttk.Notebook(parent)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

# tkk widegts generating functions
# ----------------------------------------------------------------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------------------------------------------------------------

    def getCombobox(self, parent, values, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                values (list of strings): Lista wartości które mają być wyświetlane i możliwe do wybrania w tworzonym obiekcie combobox
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.ttk.Combobox, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.ttk.Combobox): Jeżeli nie podano nazwy widgetu

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
        new_widget = ttk.Combobox(parent, values=values)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getProgressbar(self, parent, orient='horizontal', mode='determinate', style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                orient ("horizontal"/"vertical"): Orientacja suwaka
                mode ('indeterminate'/'determinate'): Tryb
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.ttk.Progressbar, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.ttk.Progressbar): Jeżeli nie podano nazwy widgetu

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
        new_widget = ttk.Progressbar(parent, orient=orient, mode=mode)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getSeparator(self, parent, orient='horizontal', style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                orient ("horizontal"/"vertical"): Orientacja suwaka
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.ttk.Separator, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.ttk.Separator): Jeżeli nie podano nazwy widgetu

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
        new_widget = ttk.Separator(parent, orient=orient)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget

    def getTreeview(self, parent, style="auto", name="none", prefix=""):
        """
            Funkcja generuje obiekt głównego okna.
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
                name (str): Nazwa tworzonego obiektu. Można również podać argument "auto" w celu wygenerowania nazwy automatycznej. W przypadku braku podania nie nastąpi wpid tworzonego widgetu do bazy danych.
                prefix (str): Prefix dodawany przed nazwą w wypadku tworzenia nazwy automatycznej. Np. dla prefixsu "PopUp1" nazwa automatyczna mogła by się wygenerować taka: "PopUp1widget124"

            Returns:
                touple(widget, name) (tkinter.ttk.Treeview, str): Jeżeli podano nazwę widgetu lub wybrano nazwę automatyczną
                widget (tkinter.ttk.Treeview): Jeżeli nie podano nazwy widgetu

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
        new_widget = ttk.Treeview(parent)

        if not name == "none":
            self.widgets[new_name] = new_widget
            return (self.widgets[new_name], new_name)
        else:
            return new_widget
