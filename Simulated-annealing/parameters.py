"""
Plik przechowywujący parametry progrmu
"""

# Rozmiar mapy
MapXSize = (-1000, 1000)
MapYSize = (-1000, 1000)
# Dane hulajnóg
ScooterCout = 1000
ScooterPriceRange = (120, 200)  # Zakres zysków za zebranie chulajnóg

# Dane zbieraczy hulajnóg
CarCapasity = 50
CollectorsX = 0
CollectorsY = 0
Diesel_Price = 1

# Parametry algorytmu
# Temp = 21370
Tmin = 0.1
Alfa = 0.8
K_Number = 100

# Wczytywanie danych
loadData = False
path = 'data1.txt'
'''
Kolejność danych w wczytywanych plikach:
 - ScooterX
 - ScooterY
 - ScooterPrice
 - ScooterWeight
'''