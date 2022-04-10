using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Piecyk.GlobalFunctions;

namespace Piecyk.Tests
{

    public class GlobalFunctions_ConverterFunctions_Tests
    {
        [Fact]
        public void Namespace_Function_ScenarioTest()
        {
            // Arrage
            short expected_1 = 12;
            ushort expected_2 = 12;
            int expected_3 = 12;
            uint expected_4 = 12;

            float expected_5 = 12.5F;
            float expected_6 = 12.5F;
            float expected_7 = 12.5F;
            float expected_8 = 12.5F;

            // Act
            short actual_1 = (short) ConverterFunctions.ConvertNumericStringToNumber("12", "short");
            ushort actual_2 = (ushort) ConverterFunctions.ConvertNumericStringToNumber("12", "ushort");
            int actual_3 = (int) ConverterFunctions.ConvertNumericStringToNumber("12", "int");
            uint actual_4 = (uint) ConverterFunctions.ConvertNumericStringToNumber("12", "uint");

            float actual_5 = (float) ConverterFunctions.ConvertNumericStringToNumber("12.5", "float");
            float actual_6 = (float) ConverterFunctions.ConvertNumericStringToNumber("12,5", "float");
            float actual_7 = (float) ConverterFunctions.ConvertNumericStringToNumber("12.5", "ufloat");
            float actual_8 = (float) ConverterFunctions.ConvertNumericStringToNumber("12,5", "ufloat");

            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
            Assert.Equal(expected_6, actual_6);
            Assert.Equal(expected_7, actual_7);
            Assert.Equal(expected_8, actual_8);
        }
    }
}
