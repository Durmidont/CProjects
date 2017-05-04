using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FormatsConvert.Tests
{
    [TestFixture]
    public class SpecialFuncsTests
    {
        [Test]
        public void ToAbsolute()
        {
            ushort u = 0xF000;
            double d = u.ToAbsolute(10000);
            Assert.AreEqual(d, -10000);
        }
    }
}
