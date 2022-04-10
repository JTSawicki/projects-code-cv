using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Piecyk.GlobalFunctions;

namespace Piecyk.Tests
{
    public class ThreadSafeGlobalFunctions_Tests
    {
        [Fact]
        public void ConvertTemperatureDoubleToUshort_CorrectValuesTest()
        {
            // Arrage
            ushort expected_1 = 100;
            ushort expected_2 = 105;
            ushort expected_3 = 65536 - 105;
            ushort expected_4 = 0;
            // Act
            ushort actual_1 = ThreadSafeGlobalFunctions.ConvertTemperatureDoubleToUshort(10);
            ushort actual_2 = ThreadSafeGlobalFunctions.ConvertTemperatureDoubleToUshort(10.5);
            ushort actual_3 = ThreadSafeGlobalFunctions.ConvertTemperatureDoubleToUshort(-10.5);
            ushort actual_4 = ThreadSafeGlobalFunctions.ConvertTemperatureDoubleToUshort(0);
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
        }

        [Fact]
        public void ConvertTemperatureDoubleToUshort_TooBigValuesTest()
        {
            // Arrage
            ushort expected_1 = 0;
            ushort expected_2 = 105;
            // Act
            ushort actual_1 = ThreadSafeGlobalFunctions.ConvertTemperatureDoubleToUshort(0.01234);
            ushort actual_2 = ThreadSafeGlobalFunctions.ConvertTemperatureDoubleToUshort(10.51234);
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
        }

        [Fact]
        public void ConvertTemperatureDoubleToUshort_TooManyNumbersAfterDotValuesTest()
        {
            // Arrage
            ushort expected_1 = (ushort)(65536 + Resources.SettingsPool.TemperatureLimits[Resources.SettingsPool.TemperatureProbeType].Item1 * 10);
            ushort expected_2 = (ushort)(Resources.SettingsPool.TemperatureLimits[Resources.SettingsPool.TemperatureProbeType].Item2 * 10);
            // Act
            ushort actual_1 = ThreadSafeGlobalFunctions.ConvertTemperatureDoubleToUshort(ushort.MaxValue * (-1));
            ushort actual_2 = ThreadSafeGlobalFunctions.ConvertTemperatureDoubleToUshort(ushort.MaxValue);
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
        }

        [Fact]
        public void ConvertTemperatureStringToUshort_CorrectValuesTest()
        {
            // Arrage
            ushort expected_1 = 100;
            ushort expected_2 = 105;
            ushort expected_3 = 105;
            ushort expected_4 = 65536 - 105;
            // Act
            ushort actual_1 = ThreadSafeGlobalFunctions.ConvertTemperatureStringToUshort("10");
            ushort actual_2 = ThreadSafeGlobalFunctions.ConvertTemperatureStringToUshort("10.5");
            ushort actual_3 = ThreadSafeGlobalFunctions.ConvertTemperatureStringToUshort("10,5");
            ushort actual_4 = ThreadSafeGlobalFunctions.ConvertTemperatureStringToUshort("-10.5");
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
        }

        [Fact]
        public void ConvertTemperatureUshortToDouble_CorrectValuesTest()
        {
            // Arrage
            double expected_1 = 10;
            double expected_2 = 10.5;
            double expected_3 = -10.5;
            double expected_4 = 0;
            // Act
            double actual_1 = ThreadSafeGlobalFunctions.ConvertTemperatureUshortToDouble(100);
            double actual_2 = ThreadSafeGlobalFunctions.ConvertTemperatureUshortToDouble(105);
            double actual_3 = ThreadSafeGlobalFunctions.ConvertTemperatureUshortToDouble(65536 - 105);
            double actual_4 = ThreadSafeGlobalFunctions.ConvertTemperatureUshortToDouble(0);
            // Assert
            Assert.Equal(expected_1, actual_1);
            Assert.Equal(expected_2, actual_2);
            Assert.Equal(expected_3, actual_3);
            Assert.Equal(expected_4, actual_4);
        }
    }
}
