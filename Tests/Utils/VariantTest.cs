using NUnit.Framework;

namespace Plucky.Common
{
    public class VariantTest
    {
        [Test]
        public void CopyShortTest()
        {
            Variant uut = new Variant();

            uut.shortValue = 12345;
            Assert.AreEqual(12345, uut.shortValue);
            Assert.AreEqual(16, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Variant)));

            Variant copy = uut;
            Assert.AreEqual(12345, copy.shortValue);
        }

        [Test]
        public void CopyStringTest()
        {
            Variant uut = new Variant();

            uut.stringValue = "My String";
            Assert.AreEqual("My String", uut.stringValue);
            Assert.AreEqual(16, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Variant)));

            Variant copy = uut;
            Assert.AreEqual("My String", copy.stringValue);

            uut.stringValue = null;
            Assert.AreEqual("My String", copy.stringValue);
        }
    }
}
