using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Collections.Generic;

namespace DemoUT.Tests
{
    [TestClass]
    public class UTMathTests
    {
        [TestMethod]

        [TestCategory("Method1")]
        public void TestMultiply()
        {
            Assert.AreEqual(8, UTMath.Multiply(2, 4));
        }

        [TestMethod]
        [DataRow(3, 5, DisplayName = "TestMultiply2 - a")]
        [DataRow(35, 53, DisplayName = "TestMultiply2 - b")]
        [DataRow(4, 5, DisplayName = "TestMultiply2 - c")]
        [TestCategory("Method2")]
        public void TestMultiply2(int a, int b)
        {
            //if (a == 4)
            //    Assert.AreEqual(2 * a * b, UTMath.Multiply(a, b));
            //else
                Assert.AreEqual(a * b, UTMath.Multiply(a, b));
        }

        List<MultiplyInput> s_multiplyInputs = new List<MultiplyInput>
        {
            new MultiplyInput { A = 3, B = 4},
            new MultiplyInput { A = 2, B = 44},
            new MultiplyInput { A = 4, B = 2223},
            new MultiplyInput { A = 422, B = 343}
        };

        [TestMethod]
        [DataRow(0, DisplayName ="0")]
        [DataRow(1, DisplayName = "1")]
        [DataRow(2, DisplayName = "2")]
        [DataRow(3, DisplayName = "3")]
        public void TestMultiply3(int inputIndex)
        {
            var input = s_multiplyInputs[inputIndex];
            if (input.A == 4)
                Assert.AreEqual(2 * input.A * input.B, UTMath.Multiply(input.A, input.B));
            else
                Assert.AreEqual(input.A * input.B, UTMath.Multiply(input.A, input.B));
        }

        private class TestMultiply3DataSourceAttribute : Attribute, ITestDataSource
        {
            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                //Microsoft.VisualStudio.TestTools.UnitTesting.ITestMethod
                return new[]
                    {   new object[] {new MultiplyInput { A = 3, B = 4} },
                        new object[] {new MultiplyInput { A = 2, B = 44} },
                        new object[] {new MultiplyInput { A = 4, B = 2223} }
                    };

            }

            public string GetDisplayName(MethodInfo methodInfo, object[] data)
            {
                return string.Format("{0} ({1})", methodInfo.Name, ((MultiplyInput)data[0]).A);
            }
        }
    }

    public interface ITestDataSource
    {
        /// <summary>
        /// Gets the test data from custom data source.
        /// </summary>
        IEnumerable<object[]> GetData(MethodInfo methodInfo);

        /// <summary>
        /// Display name to be displayed for test corresponding to data row.
        /// </summary>
        string GetDisplayName(MethodInfo methodInfo, object[] data);
    }

    public class MultiplyInput
    {
        public int A { get; set; }

        public int B { get; set; }
    }

}
