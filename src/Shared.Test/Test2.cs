using Itaiji;

namespace Kanji.Test;

[TestClass]
public sealed class Test2
{
    [TestMethod]
    public void IvsTypeTest()
    {
        {
            var kanji = new KanjiChar('辻');
            Assert.AreEqual(IvsType.None, kanji.IvsType);
        }

        {
            var kanji = new KanjiChar('辻', 0x00);
            Assert.AreEqual(IvsType.AdobeJapan, kanji.IvsType);
        }

        {
            var kanji = new KanjiChar('辻', 0x02);
            Assert.AreEqual(IvsType.HDandMJ, kanji.IvsType);
        }

        {
            var kanji = new KanjiChar('辻', 0x04);
            Assert.AreEqual(IvsType.HanyoDenshi, kanji.IvsType);
        }

        {
            var kanji = new KanjiChar('辻', 0x07);
            Assert.AreEqual(IvsType.Unknown, kanji.IvsType);
        }
    }
}
