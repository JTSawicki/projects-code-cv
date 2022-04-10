"""
Obiekt wykresu
"""
import tkinter as tk
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import matplotlib.pyplot as plt


class PlotObject:
    def __init__(self, masterFrame, numer):
        f = plt.figure(numer)
        self.plot_cv = FigureCanvasTkAgg(f, master=masterFrame)
        plt.close()
        self.masterFrame = masterFrame

    def pack_plot(self):
        self.plot_cv.get_tk_widget().pack(fill=tk.BOTH, expand=tk.YES)

    def replace_plot(self, f):
        self.plot_cv.get_tk_widget().pack_forget()
        self.plot_cv = FigureCanvasTkAgg(f, master=self.masterFrame)
        self.plot_cv.get_tk_widget().pack()