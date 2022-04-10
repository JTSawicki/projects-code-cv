'''
Skrypt służący do generowania plików z danymi do wczytania
'''
import numpy as np
import random

def listToStr(list):
    outStr = '['
    for i in range(len(list)):
        outStr += str(list[i])
        if i < len(list) - 1:
            outStr += ', '
    outStr += ']'
    return outStr



print("Generowanie pliku z danymi. Podaj ścieżkę do pliku wyjściowego.")
path = input("Ścieżka> ")

MapRadius = 60
MapXSize = (-MapRadius, MapRadius)
MapYSize = (-MapRadius, MapRadius)
ScooterCout = 50
ScooterPriceRange = (20, 50)
CarCapasity = 20
Disel_Price = 1

ScooterX = np.random.randint(MapXSize[0], MapXSize[1], ScooterCout)
ScooterY = np.random.randint(MapYSize[0], MapYSize[1], ScooterCout)
ScooterPrice = np.random.randint(ScooterPriceRange[0], ScooterPriceRange[1], ScooterCout)

ScooterWeight = []
for i in ScooterPrice:
    ScooterWeight.append(random.uniform(0.8, 1.2))

with open(path, 'wt') as f:
    f.write(listToStr(ScooterX) + '\n')
    f.write(listToStr(ScooterY) + '\n')
    f.write(listToStr(ScooterPrice) + '\n')
    f.write(listToStr(ScooterWeight) + '\n')
    f.write(str(list(MapXSize)) + '\n')
    f.write(str(list(MapYSize)) + '\n')
    f.write(str(ScooterCout) + '\n')
    f.write(str(list(ScooterPriceRange)) + '\n')
    f.write(str(CarCapasity) + '\n')
    f.write(str(Disel_Price))
