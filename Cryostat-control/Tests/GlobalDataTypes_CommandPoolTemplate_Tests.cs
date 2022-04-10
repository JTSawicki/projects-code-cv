using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Piecyk.GlobalDataTypes;

namespace Piecyk.Tests
{
    public class GlobalDataTypes_CommandPoolTemplate_Tests
    {
        [Fact]
        public void FindCommand_AllScenarioTest()
        {
            // Arrage
            bool expected_1 = true;
            bool expected_2 = true;
            bool expected_3 = true;
            bool expected_4 = true;
            bool expected_5 = true;

            // Act
            // Sprawdzam tylko kilka parametrów ponieważ jest ich dużo, a nie ma implementacji poprawnej funkcji Equals.
            bool actual_1 = CommandPoolTemplate.FindCommand("pid", "set", false).Content.Equals("set");
            bool actual_2 = CommandPoolTemplate.FindCommand("temperature", "toPresent", false).Content.Equals("toPresent");
            CommandTemplate tmp3 = CommandPoolTemplate.FindCommand("XD", "TegoNaPewnoNieMaXD", false);
            bool actual_3 = tmp3.Content.Equals("") && tmp3.Description.Equals("") && tmp3.CommandParameterList.Count == 0;

            CommandTemplate tmp4 = CommandPoolTemplate.FindCommand("", "end", true);
            bool actual_4 = tmp4.Content.Equals("end") && tmp3.CommandParameterList.Count == 0;

            CommandTemplate tmp5 = CommandPoolTemplate.FindCommand("To jest nieistotne", "TegoNaPewnoNieMaXD", true);
            bool actual_5 = tmp5.Content.Equals("") && tmp3.Description.Equals("") && tmp3.CommandParameterList.Count == 0;

            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
        }
    }
}
