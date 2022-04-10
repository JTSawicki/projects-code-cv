using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace Piecyk.Tests
{
    // [Collection("Sequential")]
    public class _TemplateTestClass
    {
        // Typical test scenario:
        // AllScenarioTest
        // CorrectValueTest
        // OutOfRangeValueTest
        // BadValueTest

        [Fact]
        public void Namespace_Function_ScenarioTest()
        {
            // Arrage
            bool expected_ = true;

            // Act
            bool actual_ = true;

            // Assert
            Assert.Equal(expected_, actual_);
        }
    }
}
