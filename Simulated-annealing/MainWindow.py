import tkinter as tk
import tkinter.ttk as ttk
from tkinter import X, Y, BOTH, YES, LEFT, RIGHT, TOP, BOTTOM, END
from tkfactory.TkFactory import TkFactory
from tkfactory.MyFactory import MyFactory
from tkinter import messagebox
from tkinter import filedialog
import parameters
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
from PlotObject import PlotObject
import MMWD_DostawcaHulajnog as MMWD
import os

factory = TkFactory.getFactory()
mfactory = MyFactory.getFactory()

class MainWindow(tk.Tk):
    """
        Główne okno programu
    """


    def __init__(self):
        """
            Inicjacja głównego okna
            Zmienne:
                widgets (dic): Słownik zawierający łącza do widgetów
        """
        tk.Tk.__init__(self)
        self.widgets = {}
        self.pltIndex = 1
        self.build()
        self.initActions()
        self.refreshGui()
        # self.widgets["mcTypeCombo"].current(0)


    def build(self):
        """
            Funkcja konstruująca GUI
        """
        # Tworzenie menu
        self.widgets["menu"] = factory.getMenu(self)

        self.widgets["databaseMenu"] = factory.getMenu(self.widgets["menu"])
        self.widgets["menu"].add_cascade(label="Symulacja", menu=self.widgets["databaseMenu"])

        self.widgets["infoMenu"] = factory.getMenu(self.widgets["menu"])
        self.widgets["menu"].add_cascade(label="Info", menu=self.widgets["infoMenu"])
        self.widgets["infoMenu"].add_command(label="O programie", command=self.showInfoAboutProgram)

        self.config(menu=self.widgets["menu"])

        # Tworzenie głównego okna wyboru parametrów
        self.widgets["mChoiceFrame"] = factory.getFrame(self)
        self.widgets["mChoiceFrame"].pack(side=LEFT, fill=BOTH)
        self.widgets["mChoiceFrame"].config(borderwidth=2)

        self.widgets["commentLabel1"] = factory.getLabel(self.widgets["mChoiceFrame"], "Parametry symulacji")
        self.widgets["commentLabel1"].grid(row=0, column=1, columnspan=2)

        self.widgets["mapRaduisLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "Promień rozmiaru mapy")
        self.widgets["mapRaduisLabel"].grid(row=1, column=1)
        self.widgets["mapRaduisEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        self.widgets["mapRaduisEntry"].grid(row=1, column=2)

        self.widgets["scooterCountLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "Ilość hulajnóg")
        self.widgets["scooterCountLabel"].grid(row=2, column=1)
        self.widgets["scooterCountEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        self.widgets["scooterCountEntry"].grid(row=2, column=2)

        self.widgets["scooterMinPriceLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "Minimalna cena")
        self.widgets["scooterMinPriceLabel"].grid(row=3, column=1)
        self.widgets["scooterMinPriceEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        self.widgets["scooterMinPriceEntry"].grid(row=3, column=2)

        self.widgets["scooterMaxPriceLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "Maksymalna cena")
        self.widgets["scooterMaxPriceLabel"].grid(row=4, column=1)
        self.widgets["scooterMaxPriceEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        self.widgets["scooterMaxPriceEntry"].grid(row=4, column=2)

        self.widgets["carCapasityPriceLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "Pojemność bagażnika")
        self.widgets["carCapasityPriceLabel"].grid(row=5, column=1)
        self.widgets["carCapasityPriceEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        self.widgets["carCapasityPriceEntry"].grid(row=5, column=2)

        self.widgets["diselPricePriceLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "Cena paliwa / km")
        self.widgets["diselPricePriceLabel"].grid(row=6, column=1)
        self.widgets["diselPricePriceEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        self.widgets["diselPricePriceEntry"].grid(row=6, column=2)

        self.widgets["commentLabel2"] = factory.getLabel(self.widgets["mChoiceFrame"], "Parametry algorytmu")
        self.widgets["commentLabel2"].grid(row=7, column=1, columnspan=2)

        # self.widgets["TempLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "Temp")
        # self.widgets["TempLabel"].grid(row=8, column=1)
        # self.widgets["TempEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        # self.widgets["TempEntry"].grid(row=8, column=2)

        self.widgets["TminLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "Tmin")
        self.widgets["TminLabel"].grid(row=9, column=1)
        self.widgets["TminEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        self.widgets["TminEntry"].grid(row=9, column=2)

        self.widgets["AlfaLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "Alfa")
        self.widgets["AlfaLabel"].grid(row=10, column=1)
        self.widgets["AlfaEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        self.widgets["AlfaEntry"].grid(row=10, column=2)

        self.widgets["K_NumberLabel"] = factory.getLabel(self.widgets["mChoiceFrame"], "K_Number")
        self.widgets["K_NumberLabel"].grid(row=11, column=1)
        self.widgets["K_NumberEntry"] = factory.getEntry(self.widgets["mChoiceFrame"])
        self.widgets["K_NumberEntry"].grid(row=11, column=2)

        self.widgets["loadFileFrame"] = factory.getLabelFrame(self.widgets["mChoiceFrame"], "Plik do wczytania")
        self.widgets["loadFileFrame"].grid(row=12, column=1, rowspan=3, columnspan=2, sticky='wesn')
        self.widgets["loadFileEntry"] = factory.getEntry(self.widgets["loadFileFrame"])
        self.widgets["loadFileEntry"].pack(side=LEFT, fill=X, expand=YES)

        self.widgets["updateButton"] = factory.getButton(self.widgets["mChoiceFrame"], "Załaduj nowe wartości", self.updateParameters)
        self.widgets["updateButton"].grid(row=15, column=1, columnspan=2, sticky='we')

        self.widgets["runButton"] = factory.getButton(self.widgets["mChoiceFrame"], "Uruchom symulację", self.runSimulation)
        self.widgets["runButton"].grid(row=16, column=1, columnspan=2, sticky='we')

        self.widgets["commentLabel3"] = factory.getLabel(self.widgets["mChoiceFrame"], "Obecne parametry algorytmu i symulacji")
        self.widgets["commentLabel3"].grid(row=17, column=1, columnspan=2)
        self.widgets["currentParametersDisplayText"] = factory.getLabel(self.widgets["mChoiceFrame"], "asdf\nasdf\nasdf")
        self.widgets["currentParametersDisplayText"].grid(row=18, column=1, columnspan=2, rowspan=5, sticky='w')

        # Dodawanie okienka wykresów
        self.widgets["chartNotebook"] = factory.getNotebook(self)
        self.widgets["chartNotebook"].pack(side=RIGHT, fill=BOTH, expand=YES)
        # self.widgets["chartNotebook"].config(borderwidth=2)

        self.widgets["chart1Frame"] = factory.getFrame(self.widgets["chartNotebook"])
        self.widgets["chart1Frame"].pack(fill=BOTH, expand=YES)
        self.widgets["chartNotebook"].add(self.widgets["chart1Frame"], text="Wykres 1")

        self.widgets["chart2Frame"] = factory.getFrame(self.widgets["chartNotebook"])
        self.widgets["chart2Frame"].pack(fill=BOTH, expand=YES)
        self.widgets["chartNotebook"].add(self.widgets["chart2Frame"], text="Wykres 2")

        self.widgets["chart1"] = PlotObject(self.widgets["chart1Frame"], 1)
        newF = plt.figure(num=self.pltIndex, dpi=100)
        self.pltIndex += 1
        plt.plot([], [])
        plt.title('Mapa miasta')
        self.widgets["chart1"].replace_plot(newF)
        plt.close()

        self.widgets["chart2"] = PlotObject(self.widgets["chart2Frame"], 2)
        newF = plt.figure(num=self.pltIndex, dpi=100)
        self.pltIndex += 1
        plt.plot([], [])
        plt.title('Przebieg algorytmu')
        self.widgets["chart2"].replace_plot(newF)
        plt.close()


    def initActions(self):
        """
            Funkcja inicjuje domyślne zachowania gui takie jak reakcja na zmianę pola
        """
        pass


    def refreshGui(self):
        """
            Funkcja odświeża elementy gui
        """
        # Odświeżanie pola textbox wyświetlającego obecne parametry
        OutText = ""
        OutText = OutText + "MapXSize = " + str(parameters.MapXSize) + '\n'
        OutText = OutText + "MapYSize = " + str(parameters.MapYSize) + '\n'
        OutText = OutText + "ScooterCout = " + str(parameters.ScooterCout) + '\n'
        OutText = OutText + "ScooterPriceRange = " + str(parameters.ScooterPriceRange) + '\n'
        OutText = OutText + "CarCapasity = " + str(parameters.CarCapasity) + '\n'
        OutText = OutText + "Diesel_Price = " + str(parameters.Diesel_Price) + '\n'
        # OutText = OutText + "Temp = " + str(parameters.Temp) + '\n'
        OutText = OutText + "Tmin = " + str(parameters.Tmin) + '\n'
        OutText = OutText + "Alfa = " + str(parameters.Alfa) + '\n'
        OutText = OutText + "K_Number = " + str(parameters.K_Number) + '\n'
        OutText = OutText + "DataLoad = " + str(parameters.loadData) + '\n'
        OutText = OutText + "FilePath = " + str(parameters.path)
        self.widgets["currentParametersDisplayText"]["text"] = OutText
        self.widgets["currentParametersDisplayText"]["justify"] = tk.LEFT


    def showInfoAboutProgram(self):
            """
                Funkcja wyświetla informacje o programie
            """
            info = """
    Autorzy: Jan Sawicki, Jakub Pudło, Hubert Sujka
    Wersja: 2.0
    Użyto:
        - python 3
        - colorama
        - tkinter
        - numpy
        - matplotlib
    """
            popup = factory.getToplevel(factory.getWidget("root"))
            popup.title("O programie")
            txt = factory.getText(popup)
            txt.insert(END, info)
            txt.pack(fill=BOTH, expand=YES)


    def updateParameters(self):
        """
        Funkcja updejtująca parametry programu na podstawie nowych podanych przez użytkownika
        """
        mapRaduis_var = self.widgets["mapRaduisEntry"].get()
        scooterCount_var = self.widgets["scooterCountEntry"].get()
        scooterMinPrice_var = self.widgets["scooterMinPriceEntry"].get()
        scooterMaxPrice_var = self.widgets["scooterMaxPriceEntry"].get()
        carCapasityPrice_var = self.widgets["carCapasityPriceEntry"].get()
        diselPricePrice_var = self.widgets["diselPricePriceEntry"].get()
        # TempEntry_var = self.widgets["TempEntry"].get()
        TminEntry_var = self.widgets["TminEntry"].get()
        AlfaEntry_var = self.widgets["AlfaEntry"].get()
        K_NumberEntry_var = self.widgets["K_NumberEntry"].get()

        try:
            if mapRaduis_var != '':
                mapRaduis_var = int(mapRaduis_var)
                parameters.MapXSize=(-mapRaduis_var, mapRaduis_var)
                parameters.MapYSize = (-mapRaduis_var, mapRaduis_var)
            if scooterCount_var != '':
                scooterCount_var = int(scooterCount_var)
                parameters.ScooterCout = scooterCount_var
            if scooterMinPrice_var != '':
                scooterMinPrice_var = int(scooterMinPrice_var)
                parameters.ScooterPriceRange = (scooterMinPrice_var, parameters.ScooterPriceRange[1])
            if scooterMaxPrice_var != '':
                scooterMaxPrice_var = int(scooterMaxPrice_var)
                parameters.ScooterPriceRange = (parameters.ScooterPriceRange[0], scooterMaxPrice_var)
            if carCapasityPrice_var != '':
                carCapasityPrice_var = int(carCapasityPrice_var)
                parameters.CarCapasity = carCapasityPrice_var
            if diselPricePrice_var != '':
                diselPricePrice_var = int(diselPricePrice_var)
                parameters.Diesel_Price = diselPricePrice_var
            # if TempEntry_var != '':
                # TempEntry_var = int(TempEntry_var)
                # parameters.Temp = TempEntry_var
            if TminEntry_var != '':
                TminEntry_var = float(TminEntry_var)
                parameters.Tmin = TminEntry_var
            if AlfaEntry_var != '':
                AlfaEntry_var = float(AlfaEntry_var)
                parameters.Alfa = AlfaEntry_var
            if K_NumberEntry_var != '':
                K_NumberEntry_var = int(K_NumberEntry_var)
                parameters.K_Number = K_NumberEntry_var
        except:
            messagebox.showerror("Data error", "Podałeś dane w niepoprawnym formacie")
        self.refreshGui()

        filePath = self.widgets["loadFileEntry"].get()
        if filePath == '':
            parameters.loadData = False
        else:
            parameters.loadData = True
            parameters.path = filePath


    def runSimulation(self):
        if not os.path.isfile(parameters.path) and parameters.loadData:
            messagebox.showerror("Data error", "Podałeś NIEPOPRAWNĄ ŚCIEŻKĘ DO PLIKU!!! Zgiń śmieciu")
            return 0

        MMWD.generateScooters()
        self.refreshGui()
        MMWD.resetData()
        data = MMWD.simulation()

        newF = plt.figure(self.pltIndex, dpi=100)
        self.pltIndex += 1
        plt.plot(data[0], data[1], 'go')
        plt.plot(data[2], data[3], 'ro')
        plt.plot(data[4], data[5], 'b--')
        plt.title('Mapa miasta')
        plt.legend(['Zbieracze', 'Hulajnogi','Trasa zbieracza'])
        self.widgets["chart1"].replace_plot(newF)
        # plt.close()

        newF = plt.figure(num=self.pltIndex, dpi=100)
        self.pltIndex += 1
        plt.plot(data[6])
        plt.title('Przebieg algorytmu')
        self.widgets["chart2"].replace_plot(newF)
        # plt.close()