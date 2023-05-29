"""
Zbiór obiektów błędów
"""


class TkFactoryBasicException(Exception):
    """
    Podstawowy błąd klasy fabryki elementów widgetów tk - TkFactory
    """

    def __init__(self, info="", exceprionID=0):
        """
        Args:
            info (str): wiadomość do użytkownika/programisty o błędzie
            exceprionID (int): numer id błędu pozwalający na jego identyfikację w programie
        """
        Exception.__init__(self)
        self.info = info
        self.id = exceprionID
        self.name = "TkFactoryBasicException"

    def getInfo(self):
        """
            Fukcja pozwala uzyskać prosty dostęp do informacji o błędzie
            Returns:
                Obiekt zawierający informcję o błedzie podany przy tworzeniu błędu.
                Powinien to być obiekt typu str.
        """
        return self.info

    def getId(self):
        """
            Fukcja pozwala uzyskać prosty dostęp do informacji o błędzie
            Returns:
                Obiekt zawierający informcję o id błędu podany przy tworzeniu błędu.
                Powinien to być obiekt typu int.
        """
        return self.id

    def __str__(self):
        """
            Funkcja generująca informację o błędzie do wypisania w konsoli.
        """
        wynik = "Wywołano błąd: %s\n" % self.name
        wynik += "Info: \'%s\'\n" % self.info
        wynik += "ID błędu: \'%s\'\n" % self.id
        return wynik


class TkFactoryBadId(TkFactoryBasicException):
    """
        Błąd zwracany w przypadku podznia złego id do funkcji
    """

    def __init__(self, info="", exceprionID=0):
        TkFactoryBasicException.__init__(self, info, exceprionID)
        self.name = "TkFactoryBadId"


class TkFactoryAlreadyExist(TkFactoryBasicException):
    """
        Błąd zwracany w przypadku podania już istniejącego klucza w bazie danych przy tworzeniu nowego wpisu
    """

    def __init__(self, info="", exceprionID=0):
        TkFactoryBasicException.__init__(self, info, exceprionID)
        self.name = "TkFactoryAlreadyExist"
