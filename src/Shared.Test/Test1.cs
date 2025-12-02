using Itaiji;

namespace Kanji.Test;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    [Timeout(100,CooperativeCancellation =true)]
    public void EnumerateKanjiTest1()
    {
        var str = "辻太郎"; // ふつうの辻
        var en = str.EnumerateKanji().ToList();
        CollectionAssert.AreEqual(new KanjiChar[3] {
            new KanjiChar('辻'),
            new KanjiChar('太'),
            new KanjiChar('郎')
        }, en);
    }

    [TestMethod]
    public void EnumerateKanjiTest2()
    {
        var str = "辻󠄀太郎"; // adobe-japanのツジ
        var en = str.EnumerateKanji().ToList();
        CollectionAssert.AreEqual(new KanjiChar[3] {
            new KanjiChar('辻', 0x00),
            new KanjiChar('太'),
            new KanjiChar('郎')
        }, en);
    }

    [TestMethod]
    [DynamicData(nameof(IvsSamples))]
    public void IsVariationTest(TestData dt)
    {
        var kanji = dt.KanjiFunc();
        Assert.AreEqual(dt.IsVariation, kanji.IsVariation);
    }

    [TestMethod]
    [DynamicData(nameof(IvsSamples))]
    public void ISAttributeTest(TestData dt)
    {
        var kanji = dt.KanjiFunc();
        Assert.AreEqual(dt.IsAdobeJapan, kanji.IsAdobeJapan);
        Assert.AreEqual(dt.IsHanyoDenshi, kanji.IsHanyoDenshi);
        Assert.AreEqual(dt.IsMojiJoho, kanji.IsMojiJoho);
    }

    [TestMethod]
    [DynamicData(nameof(IvsSamples))]
    public void ValidTest(TestData dt)
    {
        var str = dt.KanjiFunc().ToString();
        Assert.AreEqual(dt.IsValidAsAdobeJapan, str.IsValidAsAdobeJapan());
        Assert.AreEqual(dt.IsValidAsHanyoDenshi, str.IsValidAsHanyoDenshi());
        Assert.AreEqual(dt.IsValidAsMojiJoho, str.IsValidAsMojiJoho());
    }

    public class TestData
    {
        public Func<KanjiChar> KanjiFunc { get; set; }
        public IvsType ExpectedIvsType { get; set; }
        public string ExpectedString { get; set; }
        public bool IsVariation { get; set; }
        public bool IsAdobeJapan { get; set; }
        public bool IsHanyoDenshi { get; set; }
        public bool IsMojiJoho { get; set; }
        public bool IsValidAsAdobeJapan { get; set; }
        public bool IsValidAsHanyoDenshi { get; set; }
        public bool IsValidAsMojiJoho { get; set; }
        public override string ToString() => KanjiFunc().ToString();

    }

    public static IEnumerable<object[]> IvsSamples()
    {
        yield return new object[]
        {
            new TestData {
                KanjiFunc = () => new KanjiChar('博'),
                ExpectedIvsType = IvsType.None,
                ExpectedString = char.ConvertFromUtf32(0x535A),
                IsVariation = false,
                IsAdobeJapan = false,
                IsHanyoDenshi = false,
                IsMojiJoho = false,
                IsValidAsAdobeJapan = true,
                IsValidAsHanyoDenshi = true,
                IsValidAsMojiJoho = true
            }
        };
        yield return new object[]
        {
            new TestData {
                KanjiFunc = () => new KanjiChar('博', 0x00),
                ExpectedIvsType = IvsType.AdobeJapan,
                ExpectedString = char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE0100),
                IsVariation = true,
                IsAdobeJapan = true,
                IsHanyoDenshi = false,
                IsMojiJoho = false,
                IsValidAsAdobeJapan = true,
                IsValidAsHanyoDenshi = false,
                IsValidAsMojiJoho = false
            }
        };
        yield return new object[]
        {
            new TestData {
                KanjiFunc =  () => new KanjiChar('博', 0x02),
                ExpectedIvsType = IvsType.HDandMJ,
                ExpectedString=  char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE0102),
                IsVariation = true,
                IsAdobeJapan= false,
                IsHanyoDenshi= true,
                IsMojiJoho= true,
                IsValidAsAdobeJapan= false,
                IsValidAsHanyoDenshi= true,
                IsValidAsMojiJoho= true
            }
        };
        yield return new object[]
        {
            new TestData {
                KanjiFunc = () =>  new KanjiChar('博', 0x07),
                ExpectedIvsType = IvsType.HanyoDenshi,
                ExpectedString = char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE0107),
                IsVariation = true,
                IsAdobeJapan = false,
                IsHanyoDenshi = true,
                IsMojiJoho = false,
                IsValidAsAdobeJapan = false,
                IsValidAsHanyoDenshi = true,
                IsValidAsMojiJoho = false
            }
        };
        yield return new object[] {
            new TestData {

                KanjiFunc = () => new KanjiChar('博', 0x0A),
                ExpectedIvsType= IvsType.MojiJoho,
                ExpectedString = char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE010A),
                IsVariation= true,
                IsAdobeJapan= false,
                IsHanyoDenshi= false,
                IsMojiJoho= true,
                IsValidAsAdobeJapan = false,
                IsValidAsHanyoDenshi = false,
                IsValidAsMojiJoho = true
            }
        };
        yield return new object[] {
            new TestData {
                KanjiFunc = () => new KanjiChar('博', 0x10),
                ExpectedIvsType =  IvsType.Unknown,
                ExpectedString = char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE0110),
                IsVariation= true,
                IsAdobeJapan= false,
                IsHanyoDenshi= false,
                IsMojiJoho= false,
                IsValidAsAdobeJapan= false,
                IsValidAsHanyoDenshi= false,
                IsValidAsMojiJoho= false
            }
        };
    }

    [TestMethod]
    [DynamicData(nameof(IvsSamples))]
    public void IvsTypeTest(TestData dt)
    {
        var kanji = dt.KanjiFunc();
        Assert.AreEqual(dt.ExpectedIvsType, kanji.IvsType);
    }

    [TestMethod]
    [DynamicData(nameof(IvsSamples))]
    public void ToStringTest(TestData dt)
    {
        var kanji = dt.KanjiFunc();
        Assert.AreEqual(dt.ExpectedString, kanji.ToString());
    }
}
