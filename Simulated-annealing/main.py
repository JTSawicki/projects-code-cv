from MainWindow import MainWindow
from tkfactory.TkFactory import TkFactory
from tkinter import messagebox

# Inicjowanie fabryki abstrakcyjnej
factory = TkFactory.getFactory()

# Tworzenie głównego okna
root = MainWindow()
factory.addExternalWidget(root, "root")

# Ustawianie ikony i nazwy programu
# root.iconbitmap('databasefolder/programicon.ico')
root.title("Zagadnienie zbieracza hulajnóg - symulowane wyżarzanie")

# Uruchamianie programu
root.mainloop()