import matplotlib.pyplot as plt
import numpy as np
import colorama
from colorama import Fore, Style
import math
import random
import LoadData
import parameters
import parameters

# Miejsce na stałe programu i deklaracje zmiennych globalnych
# -----------------------------------------------------------------------------------------------------------------------------------

# Miejsce na rozpoczęcie programu
# -----------------------------------------------------------------------------------------------------------------------------------

# Inicjowanie kolorowego wyjścia na ekran
'''colorama.init(convert=True)

print(Fore.GREEN)
print("Symulator optymalizacji zbierania hulajnóg")
print("Wersja: 1.0")
print(Fore.CYAN)
print("Wykonali:")
print("\t- Jan Sawicki")
print("\t- Jakub Pudło")
print("\t- Hubert Sujka")
print(Style.RESET_ALL)'''

# Miejsce na generowanie danych wejściowych
# -----------------------------------------------------------------------------------------------------------------------------------
ScooterX = np.array([])
ScooterY = np.array([])
ScooterPrice = np.array([])
ScooterWeight = np.array([])
i = 0
licznik = 0
Temp = 21370
# Tmin = 0.1
# Alfa = 0.998
# K_Number = 100
total_result = []
result = []
S_x = [0]
S_y = [0]
A = [[]]
list = []
weight_colected = 0
Max_Price = 0
# Miejsce na funkcje
# -----------------------------------------------------------------------------------------------------------------------------------
def generateScooters():
    global ScooterX, ScooterY, ScooterPrice, ScooterWeight
    # Generowanie wpspółrzędnych hulajnóg
    if parameters.loadData is False:
        ScooterX = np.random.randint(parameters.MapXSize[0], parameters.MapXSize[1], parameters.ScooterCout)
        ScooterY = np.random.randint(parameters.MapYSize[0], parameters.MapYSize[1], parameters.ScooterCout)
        ScooterPrice = np.random.randint(parameters.ScooterPriceRange[0], parameters.ScooterPriceRange[1], parameters.ScooterCout)

        ScooterWeight=[]
        for i in ScooterPrice:
            ScooterWeight.append(random.uniform(0.8, 1.2))
    else:
        ScooterData = LoadData.my_function(parameters.path)
        ScooterX = ScooterData[0]
        ScooterY = ScooterData[1]
        ScooterPrice = ScooterData[2]
        ScooterWeight = ScooterData[3]
        parameters.MapXSize = tuple(ScooterData[4])
        parameters.MapYSize = tuple(ScooterData[5])
        parameters.ScooterCout = ScooterData[6]
        parameters.ScooterPriceRange = tuple(ScooterData[7])
        parameters.CarCapasity = ScooterData[8]
        parameters.Disel_Price = ScooterData[9]


def resetData():
    global i, licznik, Temp, total_result, result, S_x, S_y, A, list, weight_colected, Max_Price
    i = 0
    licznik=0
    Temp = 21370
    total_result=[]
    result=[]
    S_x=[0]
    S_y=[0]
    A=[[]]
    list=[]
    weight_colected=0
    Max_Price = max(ScooterPrice)


def income_function(result):
    income = 0
    for i in result:
        income = income + ScooterPrice[i]
    return income


def cost_function(result):
    cost = parameters.Diesel_Price * math.sqrt((parameters.CollectorsX - ScooterX[result[0]]) ** 2 + (parameters.CollectorsX - ScooterY[result[0]]) ** 2)
    for i in range(1, len(result)):
        cost = cost + parameters.Diesel_Price * math.sqrt(
            (ScooterX[result[i]] - ScooterX[result[i - 1]]) ** 2 + (ScooterY[result[i]] - ScooterY[result[i - 1]]) ** 2)
    cost = cost + parameters.Diesel_Price * math.sqrt(
        (parameters.CollectorsX - ScooterX[result[-1]]) ** 2 + (parameters.CollectorsX - ScooterY[result[-1]]) ** 2)
    return cost


def weight_function(result):
    w=0
    for i in result:
        w = w + ScooterWeight[i]
    return w


def penalty_function(result):
    penalty=0
    if weight_function(result)>parameters.CarCapasity:
        return Max_Price+(weight_function(result)-parameters.CarCapasity)*Max_Price
    return 0


def goal_function(result):
    goal = cost_function(result) - income_function(result) + penalty_function(result)
    return goal


def P(befor,after,Temp):
    return math.exp(-(after-befor)/Temp)


def simulation():
    global i, licznik, Temp, total_result, result, S_x, S_y, A, list, weight_colected, Max_Price, ScooterX, ScooterY, ScooterPrice, ScooterWeight
    # Miejsce main
    # -----------------------------------------------------------------------------------------------------------------------------------

    while(parameters.CarCapasity-1>=weight_colected):
        x = random.sample(range(len(ScooterX)), 1)[0]
        if x not in result:
            result.append(x)
            weight_colected = weight_colected+ScooterWeight[x]


    while (Temp > parameters.Tmin):
        for k in range(parameters.K_Number):
            modyfi = random.randint(1, 3)
            new_result= result[:]
            if (modyfi==1): # Zmieniamy dwie pozycje z poprzedniego rozwiazania
                change = random.sample(range(len(new_result)), 2)
                new_result1 = result[:]

                x = random.sample(range(len(ScooterX)), 1)[0]
                if x not in result:
                    new_result1[change[0]] = x
                new_result = new_result1[:]

                x = random.sample(range(len(ScooterX)), 1)[0]
                if x not in new_result:
                    new_result[change[1]] = x
            elif modyfi==2 and len(new_result)>3 :
                new_result.pop()
            elif (modyfi==3):
                x = random.sample(range(len(ScooterX)), 1)[0]
                if x not in result:
                    new_result.append(x)
                    weight_colected = weight_colected + ScooterWeight[x]


            if (goal_function(result) > goal_function(new_result)):
                result = new_result
            elif (random.random() < P(goal_function(result), goal_function(new_result), Temp)):
                result = new_result

            total_result.append(-goal_function(result))

        Temp=parameters.Alfa*Temp


    print("X, Y, Zysk, Waga")
    for a in result:
        print(ScooterX[a], ScooterX[a], ScooterPrice[a], ScooterWeight[a])
        S_x.append(ScooterX[a])
        S_y.append(ScooterY[a])
    S_x.append(0)
    S_y.append(0)

    print(result)
    print(weight_function(result))
    print(total_result[-1])
    '''plt.figure(1)
    plt.plot(CollectorsX, CollectorsY, 'go')
    plt.plot(ScooterX, ScooterY, 'ro')
    plt.plot(S_x, S_y, 'b--')
    plt.title('Mapa miasta')
    plt.legend(['Zbieracze', 'Hulajnogi','Trasa zbieracza'])
    # plt.show()

    plt.figure(2)
    plt.plot(total_result)
    plt.show()'''
    return [parameters.CollectorsX, parameters.CollectorsY, ScooterX, ScooterY, S_x, S_y, total_result]
