using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Piecyk.CodeProcessorModule;
using System.IO;
using Piecyk.GlobalFunctions;

namespace Piecyk.Tests
{
    public class CodeProcessorModule_CodePreprocessor_Tests
    {
        [Fact]
        public void ProcessCode_AllScenarioTest()
        {
            // Arrage
            CodeCommandContainer expected_1 = new CodeCommandContainer(7, "pid.change",
                new List<object>(new object[] { (short)5, (short)0, (short)0 }),
                new List<string>(new string[] { "short", "short", "short" })
                );

            CodeCommandContainer expected_2 = new CodeCommandContainer(2, "settings.LumelEnginePeriod",
                new List<object>(new object[] { (uint)50 }),
                new List<string>(new string[] { "uint" })
                );

            List<CodeCommandContainer> expected_3 = new List<CodeCommandContainer>();

            // Act
            List<CodeCommandContainer> FirstParsed = CodePreprocessor.ProcessCode(File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram3.txt"));
            List<CodeCommandContainer> SecondParsed = CodePreprocessor.ProcessCode(File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram4.txt"));

            Assert.Equal(5, FirstParsed.Count);
            CodeCommandContainer actual_1 = FirstParsed[3];
            CodeCommandContainer actual_2 = FirstParsed[0];
            List<CodeCommandContainer> actual_3 = SecondParsed;

            // Assert
            Assert.Equal(expected_1.LineNumber, actual_1.LineNumber);
            Assert.Equal(expected_1.ParametersTypes, actual_1.ParametersTypes);
            Assert.Equal(expected_1.ParametersTypes, actual_1.ParametersTypes);
            Assert.Equal((short)expected_1.Parameters[0], (short)actual_1.Parameters[0]);
            Assert.Equal((short)expected_1.Parameters[1], (short)actual_1.Parameters[1]);
            Assert.Equal((short)expected_1.Parameters[2], (short)actual_1.Parameters[2]);

            Assert.Equal(expected_2.LineNumber, actual_2.LineNumber);
            Assert.Equal(expected_2.ParametersTypes, actual_2.ParametersTypes);
            Assert.Equal(expected_2.ParametersTypes, actual_2.ParametersTypes);
            Assert.Equal((uint)expected_2.Parameters[0], (uint)actual_2.Parameters[0]);

            Assert.Equal(expected_3, actual_3);
        }

        [Fact]
        public void SplitCode_AllScenarioTest()
        {
            // Arrage
            List<string> expected_1 = new List<string>(
                File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram1.txt")
                .Split(Environment.NewLine)
                );

            // Act
            List<string> actual_1 = CodePreprocessor.SplitCode(
                File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram1.txt")
                );

            // Assert
            Assert.Equal(expected_1, actual_1);
        }

        [Fact]
        public void RemoveComments_AllScenarioTest()
        {
            // Arrage
            List<string> expected_1 = new List<string>(
                File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram1-AfterCommentRemove.txt")
                .Split(Environment.NewLine)
                );

            // Act
            List<string> actual_1 = CodePreprocessor.RemoveComments( new List<string>(
                File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram1.txt")
                .Split(Environment.NewLine)
                ));

            // Assert
            Assert.Equal(expected_1, actual_1);
        }

        [Fact]
        public void ClearBlank_AllScenarioTest()
        {
            // Arrage
            List<string> expected_1 = new List<string>(
                File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram1-AfterClearBlank.txt")
                .Split(Environment.NewLine)
                );

            List<string> expected_2 = new List<string>(
                File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram1-AfterSplitCommentRemoveClearBlank.txt")
                .Split(Environment.NewLine)
                );

            List<uint> expected_3 = new List<uint>(new uint[] { 2, 4, 6, 7, 8, 10, 11, 12});

            // Act
            List<string> actual_1 = CodePreprocessor.ClearBlank( // Testowanie minimalne - podział + sama funkcja
                CodePreprocessor.SplitCode(
                File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram1.txt")
                )).Item2;

            List<string> actual_2 = CodePreprocessor.ClearBlank( // Testowanie pełnego ciągu podziału i konwersji wstępnej
                CodePreprocessor.RemoveComments(
                CodePreprocessor.SplitCode(
                File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram1.txt")
                ))).Item2;

            List<uint> actual_3 = CodePreprocessor.ClearBlank( // Testowanie wartości numeracji wierszy(zastosowano pełny ciąg podziału i konwersji wstępnej)
                CodePreprocessor.RemoveComments(
                CodePreprocessor.SplitCode(
                File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram1.txt")
                ))).Item1;

            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
        }

        [Fact]
        public void SplitToCommandAndParameters_AllScenarioTest()
        {
            // Arrage
            List<string[]> expected_1 = new List<string[]>();
            expected_1.Add(new string[] { "settings.LumelEnginePeriod", "50" });
            expected_1.Add(new string[] { "temperature.toPresent" });
            expected_1.Add(new string[] { "function.repeat", "10" });
            expected_1.Add(new string[] { "pid.change", "5", "0", "0" });
            expected_1.Add(new string[] { "end" });

            // Act
            List<string[]> actual_1 = CodePreprocessor.SplitToCommandAndParameters(
                new List<string>(
                    File.ReadAllText("TestCodes" + FileManager.DirectorySeparator + "ExampleProgram2-NoCommentNoBlank.txt")
                    .Split(Environment.NewLine)
                ));

            // Assert
            for(int i=0; i<expected_1.Count; i++)
                for(int j=0; j<expected_1[i].Length; j++)
                {
                    Assert.Equal(expected_1[i][j], actual_1[i][j]);
                }
        }

        [Fact]
        public void CheckParametersAndCommandCorrectness_AllScenarioTest()
        {
            // Arrage
            bool expected_1 = true;
            bool expected_2 = true;
            bool expected_3 = true;
            bool expected_4 = false;
            bool expected_5 = false;

            // Act
            List<string[]> testInput = new List<string[]>(); // All good value functions test
            testInput.Add(new string[] { "function.repeat", "10"});
            testInput.Add(new string[] { "temperature.set", "12.4"});
            testInput.Add(new string[] { "temperature.set", "12,4"});
            testInput.Add(new string[] { "pid.change", "12", "14", "4" });
            testInput.Add(new string[] { "pid.change", "-12", "0", "48" });
            bool actual_1 = CodePreprocessor.CheckParametersAndCommandCorrectness(testInput);

            testInput = new List<string[]>(); // Parameter set function test
            testInput.Add(new string[] { "settings.LumelEnginePeriod", "526" });
            bool actual_2 = CodePreprocessor.CheckParametersAndCommandCorrectness(testInput);

            testInput = new List<string[]>(); // No parameter function test
            testInput.Add(new string[] { "temperature.toPresent" });
            bool actual_3 = CodePreprocessor.CheckParametersAndCommandCorrectness(testInput);

            testInput = new List<string[]>(); // Out of range bad numeric value test
            testInput.Add(new string[] { "pid.set", "100000", "100000", "100000" });
            bool actual_4 = CodePreprocessor.CheckParametersAndCommandCorrectness(testInput);

            testInput = new List<string[]>(); // Bad type value test
            testInput.Add(new string[] { "function.repeat", "-12.5" });
            bool actual_5 = CodePreprocessor.CheckParametersAndCommandCorrectness(testInput);

            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
        }

        [Fact]
        public void ConvertToCommandConatainer_GoodValueScenarioTest()
        {
            // Arrage
            CodeCommandContainer expected_1 = new CodeCommandContainer(2, "pid.change",
                new List<object>(new object[] { (short) 12, (short) 14, (short) 4 }),
                new List<string>(new string[] { "short", "short", "short" })
                );

            CodeCommandContainer expected_2 = new CodeCommandContainer(5, "settings.LumelEnginePeriod",
                new List<object>(new object[] { (uint) 526 }),
                new List<string>(new string[] { "uint" })
                );

            // Act
            List<string[]> CommandTestInput = new List<string[]>();
            CommandTestInput.Add(new string[] { "pid.change", "12", "14", "4" });
            CommandTestInput.Add(new string[] { "settings.LumelEnginePeriod", "526" });
            List<uint> LineNumbersTestInput = new List<uint>(new uint[] { 2, 5});

            List<CodeCommandContainer> actual_command_list = CodePreprocessor.ConvertToCommandConatainer(CommandTestInput, LineNumbersTestInput);
            CodeCommandContainer actual_1 = actual_command_list[0];
            CodeCommandContainer actual_2 = actual_command_list[1];

            // Assert
            Assert.Equal(expected_1.LineNumber, actual_1.LineNumber);
            Assert.Equal(expected_1.ParametersTypes, actual_1.ParametersTypes);
            Assert.Equal(expected_1.ParametersTypes, actual_1.ParametersTypes);
            Assert.Equal((short) expected_1.Parameters[0], (short)actual_1.Parameters[0]);
            Assert.Equal((short) expected_1.Parameters[1], (short)actual_1.Parameters[1]);
            Assert.Equal((short) expected_1.Parameters[2], (short)actual_1.Parameters[2]);

            Assert.Equal(expected_2.LineNumber, actual_2.LineNumber);
            Assert.Equal(expected_2.ParametersTypes, actual_2.ParametersTypes);
            Assert.Equal(expected_2.ParametersTypes, actual_2.ParametersTypes);
            Assert.Equal((uint)expected_2.Parameters[0], (uint)actual_2.Parameters[0]);
        }
    }
}
