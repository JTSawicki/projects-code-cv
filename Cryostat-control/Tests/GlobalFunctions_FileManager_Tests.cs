using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using System.IO;
using Piecyk.GlobalFunctions;

namespace Piecyk.Tests
{
    [Collection("Sequential")]
    public class GlobalFunctions_FileManager_Tests
    {
        [Fact]
        public void InitFileStructure_IsWorkingTest()
        {
            // Arrage
            bool expected_1 = true;
            bool expected_2 = true;
            bool expected_3 = true;
            // Act
            FileManager.InitFileStructure(); // Wywołanie inicjalizujące które będzie testowane
            bool actual_1 = Directory.Exists(FileManager.AppFolder);
            bool actual_2 = Directory.Exists(FileManager.AppFolder_Logs);
            bool actual_3 = Directory.Exists(FileManager.AppFolder_Tests);
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
        }

        [Fact]
        public void IsObjectPathInsideAppFolderAndValid_AllScenarioTest()
        {
            // Arrage
            bool expected_1 = true;
            bool expected_2 = true;
            bool expected_3 = true;
            bool expected_4 = true;
            bool expected_5 = false;
            bool expected_6 = false;
            bool expected_7 = false;

            // Act
            string test_path = FileManager.AppFolder; // Folder aplikacji
            bool actual_1 = FileManager.IsObjectPathInsideAppFolderAndValid(test_path);

            test_path = FileManager.AppFolder + FileManager.DirectorySeparator + "plik.txt"; // Plik w folderze aplikacji
            bool actual_2 = FileManager.IsObjectPathInsideAppFolderAndValid(test_path);

            test_path = FileManager.AppFolder + FileManager.DirectorySeparator + "nieistniejacydziwnyjakistamfolder"; // Folder w folderze aplikacji
            bool actual_3 = FileManager.IsObjectPathInsideAppFolderAndValid(test_path);

            test_path = FileManager.AppFolder + FileManager.DirectorySeparator + "nieistniejacydziwnyjakistamfolder" + FileManager.DirectorySeparator + "plik.txt"; // Plik w nieistniejącym podfolderze aplikacji
            bool actual_4 = FileManager.IsObjectPathInsideAppFolderAndValid(test_path);

            test_path = FileManager.AppFolder + FileManager.DirectorySeparator + "<plik/>.txt"; // Plik w folderze aplikacji z niedozwolonymi znakami
            bool actual_5 = FileManager.IsObjectPathInsideAppFolderAndValid(test_path);

            test_path = FileManager.AppFolder + FileManager.DirectorySeparator + ":|folder"; // Folder w folderze aplikacji z niedozwolonymi znakami
            bool actual_6 = FileManager.IsObjectPathInsideAppFolderAndValid(test_path);

            test_path = FileManager.AppDataFolder + FileManager.DirectorySeparator + "plik.txt"; // Plik spoza katalogu aplikacji
            bool actual_7 = FileManager.IsObjectPathInsideAppFolderAndValid(test_path);

            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
            Assert.Equal(expected_6, actual_6);
            Assert.Equal(expected_7, actual_7);
        }

        [Fact]
        public void AddTextToFile_AllScenarioTest()
        {
            // Arrage
            bool expected_1 = true;

            // Act
            FileManager.InitFileStructure(); // Wywołanie inicjalizacji systemu plików
            string test_file_path = FileManager.AppFolder_Tests + FileManager.DirectorySeparator + "PlikTestowy_AddTextToFile_AllScenarioTest.txt"; // Tworzenie ścieżki do pliku
            if (File.Exists(test_file_path)) // Usuwanie starego pliku jeżeli istnieje
            {
                File.Delete(test_file_path);
            }
            // Dodawanie tekstu do pliku
            string FirstLine = "Pierwsza linia tekstu";
            string SecondLine = "Druga linia tekstu";
            FileManager.AddTextToFile(test_file_path, FirstLine, EndLine: true);
            FileManager.AddTextToFile(test_file_path, SecondLine);

            string ActualTextFromFile = File.ReadAllText(test_file_path);
            string ExpectedText = FirstLine + Environment.NewLine + SecondLine;

            bool actual_1 = ActualTextFromFile.Equals(ExpectedText);

            // Assert
            Assert.Equal(expected_1, actual_1);
        }
    }
}
