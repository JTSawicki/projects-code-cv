"""
    Author: Jan Sawicki
    Version: 1.0.0
    Since: 1.1.0
"""
from ..TkFactory import TkFactory
from ..factoryexceptions import TkFactoryBadId
import tkinter as tk
from tkinter import END, LEFT, CENTER, RIGHT, NORMAL, DISABLED, N, S, W, E


factory = TkFactory.getFactory()


class EntryTable(tk.Frame):
    """
        Widget stanowiący reprezentację tabeli w postaci tabali widgetów tk.Entry
        Tabla indexuje pola od 0 do podanej wielkości
        Sposób numerowania pól tabeli jest pokazany poniżej:
        +--------------+--------------+--------------+--------------+
        | cornerLabel  | rowLabel[0]  | rowLabel[1]  | rowLabel[2]  |
        +--------------+--------------+--------------+--------------+
        | rowLabel[0]  | entry[0][0]  | entry[0][1]  | entry[0][2]  |
        +--------------+--------------+--------------+--------------+
        | rowLabel[1]  | entry[1][0]  | entry[1][1]  | entry[1][2]  |
        +--------------+--------------+--------------+--------------+
        | rowLabel[2]  | entry[2][0]  | entry[2][1]  | entry[2][2]  |
        +--------------+--------------+--------------+--------------+
    """

    def __init__(self, parent, col, row, justify="left", width="auto", labels=True, labelsJustify="left", labelsWidth="auto", style="auto"):
        """
            Args:
                parent (tkinter.BaseWidget): Obiekt do którego będzie pakowany podany widget(obiekt wywołujący)
                col (int): Liczba kolumn które ma zawierać tabela
                row (int): Liczba wierszy które ma zawierać tabela
                justify (str): Justowanie takstu - left/right/center
                width (int): Szerokość komurek tabeli
                labels (True/False): Flaga czy dodawać etykietki z opisami rzędów i kolumn
                style (str): Styl za pomocą którego ma zostać stworzony obiekt
            Tworzone zmienne:
                self.entries - Lista zawierająca pola tabeli
                self.labels - Flaga informująca czy etykiety pól zostały utworzone
                self.colLabels - Lista zawierająca etykiety kolumn
                self.rowLabels - Lista zawierająca etykiety rzędów
                self.cornerLabel - Narożna etykieta
                self.col - Ilość kolumn w tabeli
                self.row - Ilość rzędów w tabeli
                self.style - Styl gui (patrz Tkfactory)
        """
        tk.Frame.__init__(self, parent)
        self.entries = []
        self.labels = labels
        self.colLabels = []
        self.rowLabels = []
        self.cornerLabel = None
        self.col = col
        self.row = row
        self.style = style

        # Tworzenie etykietek z opisami
        if labels:
            self.cornerLabel = factory.getLabel(self, "", style=self.style)
            self.cornerLabel.grid(row=0, column=0)
            for i in range(row):
                self.rowLabels.append(factory.getLabel(self, "Row %i" % i, style=self.style))
                self.rowLabels[i].grid(row=i + 1, column=0)
                # ustawianie szerokości kolumn
                if width != "auto":
                    self.rowLabels[i].config(width=width)
                # ustawianie justowania tekstu
                if justify == "right":
                    self.rowLabels[i].config(justify=RIGHT)
                if justify == "center":
                    self.rowLabels[i].config(justify=CENTER)
                else:
                    self.rowLabels[i].config(justify=LEFT)

            for i in range(col):
                self.colLabels.append(factory.getLabel(self, "Col %i" % i, style=self.style))
                # print(i + 1, "\'%s\'" % ("Some text %i" % i), self.rowLabels[i])
                self.colLabels[i].grid(row=0, column=i + 1)
                # ustawianie szerokości kolumn
                if width != "auto":
                    self.colLabels[i].config(width=width)
                # ustawianie justowania tekstu
                if justify == "right":
                    self.colLabels[i].config(justify=RIGHT)
                if justify == "center":
                    self.colLabels[i].config(justify=CENTER)
                else:
                    self.colLabels[i].config(justify=LEFT)

        # generowanie tablicy
        for i in range(row):
            self.entries.append([])
            for j in range(col):
                self.entries[i].append(factory.getEntry(self, style=self.style))
                self.entries[i][j].grid(row=i + 1, column=j + 1, sticky=N + S + E + W)
                # ustawianie szerokości kolumn
                if width != "auto":
                    self.entries[i][j].config(width=width)
                # ustawianie justowania tekstu
                if justify == "right":
                    self.entries[i][j].config(justify=RIGHT)
                if justify == "center":
                    self.entries[i][j].config(justify=CENTER)
                else:
                    self.entries[i][j].config(justify=LEFT)

    def getValue(self, row, col):
        """
            Funkcja służąca do uzyskiwania wartości pola
            Args:
                row (int): Numer rzędu
                col (int): Numer kolumny

            Returns:
                (str): Wartość pola

            Raises:
                TkFactoryBadId: W przypadku podania niepoprawnych wartości row lub col
        """
        # Sprawdzenie poprawności podawanych danych
        if col < 0 or col >= self.col or row < 0 or row >= self.row:
            raise TkFactoryBadId("W funkcji EntryTable.getValue(), podałeś niepoprawne współrzędne pola: row=%i, col=%i" % (row, col))

        return self.entries[row][col].get()

    def setValue(self, row, col, value):
        """
            Funkcja służąca do ustawiania wartości pola
            Args:
                row (int): Numer rzędu
                col (int): Numer kolumny
                value (str): Ustawiana wartość pola

            Returns:
                (str): Wartość pola

            Raises:
                TkFactoryBadId: W przypadku podania niepoprawnych wartości row lub col
        """
        # Sprawdzanie poprawności podanych danych
        if col < 0 or col >= self.col or row < 0 or row >= self.row:
            raise TkFactoryBadId("W funkcji EntryTable.setValue, podałeś niepoprawne współrzędne pola: row=%i, col=%i" % (row, col))

        # Usówania starych danych i wstawianie nowych
        self.entries[row][col].delete(0, END)
        self.entries[row][col].insert(0, value)

    def insertValue(self, row, col, value):
        """
            Funkcja służąca do dodawania wartości do pola wartości pola
            Args:
                row (int): Numer rzędu
                col (int): Numer kolumny
                value (str): Ustawiana wartość pola

            Returns:
                (str): Wartość pola

            Raises:
                TkFactoryBadId: W przypadku podania niepoprawnych wartości row lub col
        """
        # Sprawdzanie poprawności podanych danych
        if col < 0 or col >= self.col or row < 0 or row >= self.row:
            raise TkFactoryBadId("W funkcji EntryTable.insertValue, podałeś niepoprawne współrzędne pola: row=%i, col=%i" % (row, col))

        # Usówania starych danych i wstawianie nowych
        self.entries[row][col].insert(END, value)

    def lockEntry(self, row, col):
        """
            Funkcja służąca do blokowania możliwości edycji pola - nie działają wtedy funkcje edycji i użytkownik też nie może nic do niego pisać
            Args:
                row (int): Numer rzędu
                col (int): Numer kolumny
                value (str): Ustawiana wartość pola

            Returns:
                (str): Wartość pola

            Raises:
                TkFactoryBadId: W przypadku podania niepoprawnych wartości row lub col
        """
        # Sprawdzanie poprawności podanych danych
        if col < 0 or col >= self.col or row < 0 or row >= self.row:
            raise TkFactoryBadId("W funkcji EntryTable.lockEntry, podałeś niepoprawne współrzędne pola: row=%i, col=%i" % (row, col))

        self.entries[row][col].config(state=DISABLED)

    def unlockEntry(self, row, col):
        """
            Funkcja służąca do odblokowania możliwości edycji pola
            Args:
                row (int): Numer rzędu
                col (int): Numer kolumny
                value (str): Ustawiana wartość pola

            Returns:
                (str): Wartość pola

            Raises:
                TkFactoryBadId: W przypadku podania niepoprawnych wartości row lub col
        """
        # Sprawdzanie poprawności podanych danych
        if col < 0 or col >= self.col or row < 0 or row >= self.row:
            raise TkFactoryBadId("W funkcji EntryTable.unlockEntry, podałeś niepoprawne współrzędne pola: row=%i, col=%i" % (row, col))

        self.entries[row][col].config(state=NORMAL)

    def setRowLabel(self, labelIndex, text):
        """
            Funkcja pozwala ustawiać wartość etykiet rzędów. Jeżeli nie utworzono etykiet wywołanie funkcji zostanie zignorowane.
            Args:
                labelIndex (int): Numer indexu etykiety do której chcemy pisać
                text (str): Tekst który chcemy zapisać do etykiety

            Raises:
                TkFactoryBadId: W przypadku podania niepoprawnych wartości labelIndex
        """
        # Sprawdzenie czy etykiety zostały utworzone, a jeśli nie to zignorowanie wywołania funkcji
        if not self.labels:
            return None
        # Sprawdzanie poprawności podanych danych
        if labelIndex < 0 or labelIndex >= self.row:
            raise TkFactoryBadId("W funkcji EntryTable.setRowLabel, podałeś niepoprawne współrzędne pola: labelIndex=%i" % labelIndex)
        self.rowLabels[labelIndex].config(text=text)

    def getRowLabelValue(self, labelIndex):
        """
            Funkcja pozwala uzyskiwać wartości etykiet rzędów. Jeżeli nie utworzono etykiet wywołanie funkcji zostanie zignorowane.
            Args:
                labelIndex (int): Numer indexu etykiety której wartość chcemy uzyskać

            Raises:
                TkFactoryBadId: W przypadku podania niepoprawnych wartości labelIndex
        """
        # Sprawdzenie czy etykiety zostały utworzone, a jeśli nie to zignorowanie wywołania funkcji
        if not self.labels:
            return None
        # Sprawdzanie poprawności podanych danych
        if labelIndex < 0 or labelIndex >= self.row:
            raise TkFactoryBadId("W funkcji EntryTable.getRowLabel, podałeś niepoprawne współrzędne pola: labelIndex=%i" % labelIndex)

        return self.rowLabels[labelIndex]["text"]

    def setColLabel(self, labelIndex, text):
        """
            Funkcja pozwala ustawiać wartość etykiet kolumn. Jeżeli nie utworzono etykiet wywołanie funkcji zostanie zignorowane.
            Args:
                labelIndex (int): Numer indexu etykiety do której chcemy pisać
                text (str): Tekst który chcemy zapisać do etykiety

            Raises:
                TkFactoryBadId: W przypadku podania niepoprawnych wartości labelIndex
        """
        # Sprawdzenie czy etykiety zostały utworzone, a jeśli nie to zignorowanie wywołania funkcji
        if not self.labels:
            return None
        # Sprawdzanie poprawności podanych danych
        if labelIndex < 0 or labelIndex >= self.col:
            raise TkFactoryBadId("W funkcji EntryTable.setColLabel, podałeś niepoprawne współrzędne pola: labelIndex=%i" % labelIndex)
        self.colLabels[labelIndex].config(text=text)

    def setCornerLabel(self, text):
        """
            Funkcja pozwala ustawiać wartość narożnej etykiety. Jeżeli nie utworzono etykiet wywołanie funkcji zostanie zignorowane.
            Args:
                text (str): Tekst który chcemy zapisać do etykiety
        """
        # Sprawdzenie czy etykiety zostały utworzone, a jeśli nie to zignorowanie wywołania funkcji
        if not self.labels:
            return None
        self.cornerLabel.config(text=text)

    def changeColWidth(self, col, width):
        """
            Funkcja pozwala na zmianę szerokości wybraneh kolumny w tabeli
            Args:
                col (int): Numer kolumny
                width (int): ustawiana szerokość

            Raises:
                TkFactoryBadId: W przypadku podania niepoprawnych wartości col
        """
        # Sprawdzanie poprawności podanych danych
        if col < 0 or col >= self.col:
            raise TkFactoryBadId("W funkcji EntryTable.changeColWidth, podałeś niepoprawne współrzędne pola: col=%i" % col)

        # Zmienianie szarokości
        for i in self.entries:
            i[col].config(width=width)
        # Sprawdzanie czy trzeba zmienić szerokość etykiety i ewentualna zmiana
        if self.labels:
            self.colLabels[col].config(width=width)

    def changeLabelColWidth(self, width):
        """
            Funkcja pozwala na zmianę szerokości wybraneh kolumny etykiet (kolumna z etykietą cornerLabel).  Jeżeli nie utworzono etykiet wywołanie funkcji zostanie zignorowane.
            Args:
                width (int): ustawiana szerokość
        """
        # Sprawdzenie czy etykiety zostały utworzone, a jeśli nie to zignorowanie wywołania funkcji
        if not self.labels:
            return None
        # Zmienianie szerokości
        self.cornerLabel.config(width=width)
        for i in self.rowLabels:
            i.config(width=width)
