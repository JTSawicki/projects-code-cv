using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Piecyk.GlobalDataTypes;

namespace Piecyk.Tests
{
    public class GlobalDataTypes_InterlockedProperty_Tests
    {
        [Fact]
        public void InterlockedProperty_GetSetTest()
        {
            // Arrage
            bool expected_1 = true;
            bool expected_2 = true;
            bool expected_3 = false;
            bool expected_4 = true;
            bool expected_5 = true;
            bool expected_6 = false;
            bool expected_7 = true;
            bool expected_8 = true;
            bool expected_9 = false;

            // Act
            InterlockedProperty<string> SProperty = new InterlockedProperty<string>("XD");
            InterlockedProperty<int> IProperty = new InterlockedProperty<int>(12);
            InterlockedProperty<float> FProperty = new InterlockedProperty<float>(12);

            bool actual_1 = SProperty.Get().Equals("XD");
            SProperty.Set(":(");
            bool actual_2 = SProperty.Get().Equals(":(");
            bool actual_3 = SProperty.Get().Equals("XD");

            bool actual_4 = IProperty.Get().Equals(12);
            IProperty.Set(13);
            bool actual_5 = IProperty.Get().Equals(13);
            bool actual_6 = IProperty.Get().Equals(12);

            bool actual_7 = FProperty.Get().Equals(12);
            FProperty.Set(13);
            bool actual_8 = FProperty.Get().Equals(13);
            bool actual_9 = FProperty.Get().Equals(12);

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
        public void InterlockedProperty_CompareTest()
        {
            // Arrage
            bool expected_1 = true;
            bool expected_2 = false;
            bool expected_3 = true;
            bool expected_4 = false;
            bool expected_5 = true;
            bool expected_6 = false;
            bool expected_7 = false;

            // Act
            InterlockedProperty<string> SProperty = new InterlockedProperty<string>("XD");
            InterlockedProperty<int> IProperty = new InterlockedProperty<int>(12);
            InterlockedProperty<float> FProperty = new InterlockedProperty<float>(12);

            bool actual_1 = SProperty.Equals("XD");
            bool actual_2 = SProperty.Equals("Coś XD");
            bool actual_3 = IProperty.Equals(12);
            bool actual_4 = IProperty.Equals(0);
            bool actual_5 = FProperty.Equals(12);
            bool actual_6 = FProperty.Equals(0);
            bool actual_7 = FProperty.Equals("XD");

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
        public void InterlockedProperty_WorkingHashTest()
        {
            InterlockedProperty<string> property = new InterlockedProperty<string>("");
            int hash = property.GetHashCode(); //< Testowanie czy nie nastąpi błąd
        }
    }
}
