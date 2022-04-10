using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Piecyk.GlobalFunctions;

namespace Piecyk.Tests
{
    public class CheckFunctions_Tests
    {
        [Fact]
        public void ValidateCustomDeviceAdress_AllScenarioTest()
        {
            // Arrage
            bool expected_1 = false;
            bool expected_2 = false;
            bool expected_3 = false;
            bool expected_4 = false;
            bool expected_5 = true;
            bool expected_6 = true;
            bool expected_7 = true;
            // Act
            bool actual_1 = CheckFunctions.ValidateCustomDeviceAdress("-1");     // Bad numeric value
            bool actual_2 = CheckFunctions.ValidateCustomDeviceAdress("250");    // Too big
            bool actual_3 = CheckFunctions.ValidateCustomDeviceAdress("asdf");   // Not numeric
            bool actual_4 = CheckFunctions.ValidateCustomDeviceAdress("0");      // Bad value == 0
            bool actual_5 = CheckFunctions.ValidateCustomDeviceAdress("15");     // Good - 15 != 0
            bool actual_6 = CheckFunctions.ValidateCustomDeviceAdress("1");      // Good - Min value == 1
            bool actual_7 = CheckFunctions.ValidateCustomDeviceAdress("247");    // Good - Max Value == 247
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
        public void ValidatePIDParameter_AllScenarioTest()
        {
            // Arrage
            bool expected_1 = false;
            bool expected_2 = false;
            bool expected_3 = false;
            bool expected_4 = true;
            bool expected_5 = true;
            bool expected_6 = true;
            // Act
            bool actual_1 = CheckFunctions.ValidatePIDParameter("qwerty");// Non numeric value
            bool actual_2 = CheckFunctions.ValidatePIDParameter("-1");// Negative value
            bool actual_3 = CheckFunctions.ValidatePIDParameter("10000");// Too big value
            bool actual_4 = CheckFunctions.ValidatePIDParameter("123");// Value in range
            bool actual_5 = CheckFunctions.ValidatePIDParameter("0");// Min value == 0
            bool actual_6 = CheckFunctions.ValidatePIDParameter("9999");// Max value == 9999
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
            Assert.Equal(expected_6, actual_6);
        }

        [Fact]
        public void ValidateTParameter_AllScenarioTest()
        {
            // Arrage
            bool expected_1 = false;
            bool expected_2 = false;
            bool expected_3 = true;
            bool expected_4 = true;
            bool expected_5 = true;
            // Act
            bool actual_1 = CheckFunctions.ValidateTParameter("qwerty");    // Non numeric value
            bool actual_2 = CheckFunctions.ValidateTParameter( (Resources.SettingsPool.TemperatureLimits[Resources.SettingsPool.TemperatureProbeType].Item2 + 10).ToString() );  // Out of range value
            bool actual_3 = CheckFunctions.ValidateTParameter("100");       // Value in range
            bool actual_4 = CheckFunctions.ValidateTParameter(Resources.SettingsPool.TemperatureLimits[Resources.SettingsPool.TemperatureProbeType].Item1.ToString() );         // Min value
            bool actual_5 = CheckFunctions.ValidateTParameter(Resources.SettingsPool.TemperatureLimits[Resources.SettingsPool.TemperatureProbeType].Item2.ToString() );         // Max value
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
        }

        [Fact]
        public void ValidateParameterType_CorrectValuesScenariosTest()
        {
            // Arrage
            bool expected_1 = true;
            bool expected_2 = true;
            bool expected_3 = true;
            bool expected_4 = true;
            bool expected_5 = true;
            bool expected_6 = true;
            bool expected_7 = true;
            bool expected_8 = true;
            bool expected_9 = true;
            bool expected_10 = true;
            bool expected_11 = true;
            bool expected_12 = true;
            bool expected_13 = true;
            bool expected_14 = true;
            bool expected_15 = true;
            bool expected_16 = true;
            bool expected_17 = true;
            bool expected_18 = true;
            bool expected_19 = true;
            bool expected_20 = true;
            bool expected_21 = false;

            // Act
            bool actual_1 = CheckFunctions.ValidateParameterType("100", "short");                        // Mid range value
            bool actual_2 = CheckFunctions.ValidateParameterType(short.MinValue.ToString(), "short");    // Min value
            bool actual_3 = CheckFunctions.ValidateParameterType(short.MaxValue.ToString(), "short");    // Max value

            bool actual_4 = CheckFunctions.ValidateParameterType("100", "ushort");                       // Mid range value
            bool actual_5 = CheckFunctions.ValidateParameterType(ushort.MinValue.ToString(), "ushort");  // Min value
            bool actual_6 = CheckFunctions.ValidateParameterType(ushort.MaxValue.ToString(), "ushort");  // Max value

            bool actual_7 = CheckFunctions.ValidateParameterType("100", "int");                          // Mid range value
            bool actual_8 = CheckFunctions.ValidateParameterType(int.MinValue.ToString(), "int");        // Min value
            bool actual_9 = CheckFunctions.ValidateParameterType(int.MaxValue.ToString(), "int");        // Max value

            bool actual_10 = CheckFunctions.ValidateParameterType("100", "uint");                         // Mid range value
            bool actual_11 = CheckFunctions.ValidateParameterType(uint.MinValue.ToString(), "uint");      // Min value
            bool actual_12 = CheckFunctions.ValidateParameterType(uint.MaxValue.ToString(), "uint");      // Max value

            bool actual_13 = CheckFunctions.ValidateParameterType("100", "float");                        // Mid range value
            bool actual_14 = CheckFunctions.ValidateParameterType(float.MinValue.ToString(), "float");    // Min value
            bool actual_15 = CheckFunctions.ValidateParameterType(float.MaxValue.ToString(), "float");    // Max value

            bool actual_16 = CheckFunctions.ValidateParameterType("100", "ufloat");                       // Mid range value
            bool actual_17 = CheckFunctions.ValidateParameterType("0", "ufloat");                         // Min value
            bool actual_18 = CheckFunctions.ValidateParameterType(float.MaxValue.ToString(), "ufloat");   // Max value

            bool actual_19 = CheckFunctions.ValidateParameterType("qwerty", "string");                    // Any string XD
            bool actual_20 = CheckFunctions.ValidateParameterType("", "string");                          // Empty string XD

            bool actual_21 = CheckFunctions.ValidateParameterType("qwerty", "anything XD");               // Test if false XD

            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
            Assert.Equal(expected_6, actual_6);
            Assert.Equal(expected_7, actual_7);
            Assert.Equal(expected_8, actual_8);
            Assert.Equal(expected_9, actual_9);
            Assert.Equal(expected_10, actual_10);
            Assert.Equal(expected_11, actual_11);
            Assert.Equal(expected_12, actual_12);
            Assert.Equal(expected_13, actual_13);
            Assert.Equal(expected_14, actual_14);
            Assert.Equal(expected_15, actual_15);
            Assert.Equal(expected_16, actual_16);
            Assert.Equal(expected_17, actual_17);
            Assert.Equal(expected_18, actual_18);
            Assert.Equal(expected_19, actual_19);
            Assert.Equal(expected_20, actual_20);
            Assert.Equal(expected_21, actual_21);
        }

        [Fact]
        public void ValidateParameterType_OutOfRangeValuesScenariosTest()
        {
            // Arrage
            bool expected_1 = false;
            bool expected_2 = false;
            bool expected_3 = false;
            bool expected_4 = false;
            bool expected_5 = false;
            bool expected_6 = false;
            bool expected_7 = false;
            bool expected_8 = false;
            bool expected_9 = false;
            bool expected_10 = false;
            bool expected_11 = false;
            bool expected_12 = false;

            // Act
            bool actual_1 = CheckFunctions.ValidateParameterType((short.MinValue - 10L).ToString(), "short");    // Min value - 10
            bool actual_2 = CheckFunctions.ValidateParameterType((short.MaxValue + 10L).ToString(), "short");    // Max value + 10

            bool actual_3 = CheckFunctions.ValidateParameterType((ushort.MinValue - 10L).ToString(), "ushort");  // Min value - 10
            bool actual_4 = CheckFunctions.ValidateParameterType((ushort.MaxValue + 10L).ToString(), "ushort");  // Max value + 10

            bool actual_5 = CheckFunctions.ValidateParameterType((int.MinValue - 10L).ToString(), "int");        // Min value - 10
            bool actual_6 = CheckFunctions.ValidateParameterType((int.MaxValue + 10L).ToString(), "int");        // Max value + 10

            bool actual_7 = CheckFunctions.ValidateParameterType((uint.MinValue - 10L).ToString(), "uint");      // Min value - 10
            bool actual_8 = CheckFunctions.ValidateParameterType((uint.MaxValue + 10L).ToString(), "uint");      // Max value + 10

            bool actual_9 = CheckFunctions.ValidateParameterType(((double)float.MinValue * 10).ToString(), "float");    // Min value - 10
            bool actual_10 = CheckFunctions.ValidateParameterType(((double)float.MaxValue * 10).ToString(), "float");    // Max value + 10

            bool actual_11 = CheckFunctions.ValidateParameterType("-10", "ufloat");                               // Min value - 10
            bool actual_12 = CheckFunctions.ValidateParameterType(((double)float.MinValue * 10).ToString(), "ufloat");   // Max value + 10

            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
            Assert.Equal(expected_6, actual_6);
            Assert.Equal(expected_7, actual_7);
            Assert.Equal(expected_8, actual_8);
            Assert.Equal(expected_9, actual_9);
            Assert.Equal(expected_10, actual_10);
            Assert.Equal(expected_11, actual_11);
            Assert.Equal(expected_12, actual_12);
        }

        [Fact]
        public void ValidateParameterType_BadValuesValuesScenariosTest()
        {
            // Arrage
            bool expected_1 = false;
            bool expected_2 = false;
            bool expected_3 = false;
            bool expected_4 = false;
            bool expected_5 = false;
            bool expected_6 = false;
            bool expected_7 = false;
            bool expected_8 = false;
            bool expected_9 = false;

            // Act
            bool actual_1 = CheckFunctions.ValidateParameterType("qwerty", "short");     // Non numeric value

            bool actual_2 = CheckFunctions.ValidateParameterType("qwerty", "ushort");    // Non numeric value
            bool actual_3 = CheckFunctions.ValidateParameterType("-10", "ushort");       // Negative value

            bool actual_4 = CheckFunctions.ValidateParameterType("qwerty", "int");       // Non numeric value

            bool actual_5 = CheckFunctions.ValidateParameterType("qwerty", "uint");      // Non numeric value
            bool actual_6 = CheckFunctions.ValidateParameterType("-10", "uint");         // Negative value

            bool actual_7 = CheckFunctions.ValidateParameterType("qwerty", "float");     // Non numeric value

            bool actual_8 = CheckFunctions.ValidateParameterType("qwerty", "ufloat");    // Non numeric value
            bool actual_9 = CheckFunctions.ValidateParameterType("-10", "ufloat");       // Negative value

            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
            Assert.Equal(expected_6, actual_6);
            Assert.Equal(expected_7, actual_7);
            Assert.Equal(expected_8, actual_8);
            Assert.Equal(expected_9, actual_9);
        }

        [Fact]
        public void ValidateParameterNumericLimits_CorrectValuesScenariosTest()
        {
            // Arrage
            bool expected_1 = true;
            bool expected_2 = true;
            bool expected_3 = true;
            bool expected_4 = true;
            bool expected_5 = true;
            bool expected_6 = true;
            bool expected_7 = true;
            bool expected_8 = true;
            bool expected_9 = true;
            bool expected_10 = true;
            bool expected_11 = true;
            bool expected_12 = true;
            bool expected_13 = true;
            bool expected_14 = true;
            bool expected_15 = true;
            bool expected_16 = true;
            bool expected_17 = true;
            bool expected_18 = true;
            bool expected_19 = true;
            bool expected_20 = true;
            bool expected_21 = true;
            bool expected_22 = true;

            // Act
            bool actual_1 = CheckFunctions.ValidateParameterNumericLimits("15", "short", "10", "20");
            bool actual_2 = CheckFunctions.ValidateParameterNumericLimits("10", "short", "10", "20");
            bool actual_3 = CheckFunctions.ValidateParameterNumericLimits("20", "short", "10", "20");

            bool actual_4 = CheckFunctions.ValidateParameterNumericLimits("15", "ushort", "10", "20");
            bool actual_5 = CheckFunctions.ValidateParameterNumericLimits("10", "ushort", "10", "20");
            bool actual_6 = CheckFunctions.ValidateParameterNumericLimits("20", "ushort", "10", "20");

            bool actual_7 = CheckFunctions.ValidateParameterNumericLimits("15", "int", "10", "20");
            bool actual_8 = CheckFunctions.ValidateParameterNumericLimits("10", "int", "10", "20");
            bool actual_9 = CheckFunctions.ValidateParameterNumericLimits("20", "int", "10", "20");

            bool actual_10 = CheckFunctions.ValidateParameterNumericLimits("15", "uint", "10", "20");
            bool actual_11 = CheckFunctions.ValidateParameterNumericLimits("10", "uint", "10", "20");
            bool actual_12 = CheckFunctions.ValidateParameterNumericLimits("20", "uint", "10", "20");

            bool actual_13 = CheckFunctions.ValidateParameterNumericLimits("15", "float", "10", "20");
            bool actual_14 = CheckFunctions.ValidateParameterNumericLimits("15.45", "float", "10", "20");
            bool actual_15 = CheckFunctions.ValidateParameterNumericLimits("15,45", "float", "10", "20");
            bool actual_16 = CheckFunctions.ValidateParameterNumericLimits("10", "float", "10", "20");
            bool actual_17 = CheckFunctions.ValidateParameterNumericLimits("20", "float", "10", "20");

            bool actual_18 = CheckFunctions.ValidateParameterNumericLimits("15", "ufloat", "10", "20");
            bool actual_19 = CheckFunctions.ValidateParameterNumericLimits("15,45", "ufloat", "10", "20");
            bool actual_20 = CheckFunctions.ValidateParameterNumericLimits("15.45", "ufloat", "10", "20");
            bool actual_21 = CheckFunctions.ValidateParameterNumericLimits("10", "ufloat", "10", "20");
            bool actual_22 = CheckFunctions.ValidateParameterNumericLimits("20", "ufloat", "10", "20");
            
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
            Assert.Equal(expected_6, actual_6);
            Assert.Equal(expected_7, actual_7);
            Assert.Equal(expected_8, actual_8);
            Assert.Equal(expected_9, actual_9);
            Assert.Equal(expected_10, actual_10);
            Assert.Equal(expected_11, actual_11);
            Assert.Equal(expected_12, actual_12);
            Assert.Equal(expected_13, actual_13);
            Assert.Equal(expected_14, actual_14);
            Assert.Equal(expected_15, actual_15);
            Assert.Equal(expected_16, actual_16);
            Assert.Equal(expected_17, actual_17);
            Assert.Equal(expected_18, actual_18);
            Assert.Equal(expected_19, actual_19);
            Assert.Equal(expected_20, actual_20);
            Assert.Equal(expected_21, actual_21);
            Assert.Equal(expected_22, actual_22);
        }

        [Fact]
        public void ValidateParameterNumericLimits_BadValuesValuesScenariosTest()
        {
            // Arrage
            bool expected_1 = false;
            bool expected_2 = false;
            bool expected_3 = false;
            bool expected_4 = false;
            bool expected_5 = false;
            bool expected_6 = false;
            bool expected_7 = false;
            bool expected_8 = false;
            bool expected_9 = false;
            bool expected_10 = false;
            bool expected_11 = false;
            bool expected_12 = false;

            // Act
            bool actual_1 = CheckFunctions.ValidateParameterNumericLimits("5", "short", "10", "20");
            bool actual_2 = CheckFunctions.ValidateParameterNumericLimits("55", "short", "10", "20");

            bool actual_3 = CheckFunctions.ValidateParameterNumericLimits("5", "ushort", "10", "20");
            bool actual_4 = CheckFunctions.ValidateParameterNumericLimits("55", "ushort", "10", "20");

            bool actual_5 = CheckFunctions.ValidateParameterNumericLimits("5", "int", "10", "20");
            bool actual_6 = CheckFunctions.ValidateParameterNumericLimits("55", "int", "10", "20");

            bool actual_7 = CheckFunctions.ValidateParameterNumericLimits("5", "uint", "10", "20");
            bool actual_8 = CheckFunctions.ValidateParameterNumericLimits("55", "uint", "10", "20");

            bool actual_9 = CheckFunctions.ValidateParameterNumericLimits("5", "float", "10", "20");
            bool actual_10 = CheckFunctions.ValidateParameterNumericLimits("55", "float", "10", "20");

            bool actual_11 = CheckFunctions.ValidateParameterNumericLimits("5", "ufloat", "10", "20");
            bool actual_12 = CheckFunctions.ValidateParameterNumericLimits("55", "ufloat", "10", "20");
            
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
            Assert.Equal(expected_5, actual_5);
            Assert.Equal(expected_6, actual_6);
            Assert.Equal(expected_7, actual_7);
            Assert.Equal(expected_8, actual_8);
            Assert.Equal(expected_9, actual_9);
            Assert.Equal(expected_10, actual_10);
            Assert.Equal(expected_11, actual_11);
            Assert.Equal(expected_12, actual_12);
        }
    }
}
