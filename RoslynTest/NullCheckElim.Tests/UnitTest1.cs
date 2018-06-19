using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssert;

namespace NullCheckElim.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ShouldEvaluateSimpleBB()
        {
            (true).ShouldBeTrue();
        }
    }
}
