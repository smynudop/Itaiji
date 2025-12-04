#nullable disable

using System.Text;
#if NETFRAMEWORK
using Itaiji.Text;
#endif 
namespace Itaiji.Test;

[TestClass]
public sealed class ItaijiTest
{
    public const char HirosiChar = (char)0x535A;
    public const char HokkeHigh = (char)0xD867;
    public const char HokkeLow = (char)0xDE3D;
    public const char VS17High = (char)0xDB40;
    public const char VS17Low = (char)0xDD00;

    public static Rune Hirosi = new Rune('博');
    public static Rune Hokke => new Rune(0x29E3D); // 𩸽
    public static Rune VS17 => new Rune(0xE0100); // 異体字セレクタE0100

    public class ConstructorTestData
    {
        public Func<KanjiChar> KanjiFunc { get; set; }
        public Rune ExpectedBaseRune { get; set; }
        public Rune? ExpectedVariationSelector { get; set; }
        public override string ToString() => $"BaseRune: U+{ExpectedBaseRune.Value:X}, VS: {(ExpectedVariationSelector.HasValue ? $"U+{ExpectedVariationSelector.Value.Value:X}" : "null")}";
    }

    public static IEnumerable<object[]> ConstructorTestDataSamples()
    {
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(Hirosi),
                ExpectedBaseRune = Hirosi,
                ExpectedVariationSelector = null
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(Hokke),
                ExpectedBaseRune = Hokke,
                ExpectedVariationSelector = null
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(VS17),
                ExpectedBaseRune = VS17,
                ExpectedVariationSelector = null
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(Hirosi, VS17),
                ExpectedBaseRune = Hirosi,
                ExpectedVariationSelector = VS17
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(Hokke, VS17),
                ExpectedBaseRune = Hokke,
                ExpectedVariationSelector = VS17
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(Hirosi, default),
                ExpectedBaseRune = Hirosi,
                ExpectedVariationSelector = null
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar('博'),
                ExpectedBaseRune = Hirosi,
                ExpectedVariationSelector = null
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(new string([HirosiChar]), VS17),
                ExpectedBaseRune = Hirosi,
                ExpectedVariationSelector = VS17
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(new string([HokkeHigh, HokkeLow]), VS17),
                ExpectedBaseRune = Hokke,
                ExpectedVariationSelector = VS17
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(new string([HirosiChar])),
                ExpectedBaseRune = Hirosi,
                ExpectedVariationSelector = null
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(new string([HokkeHigh, HokkeLow])),
                ExpectedBaseRune = Hokke,
                ExpectedVariationSelector = null
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(new string([HirosiChar, VS17High, VS17Low])),
                ExpectedBaseRune = Hirosi,
                ExpectedVariationSelector = VS17
            }
        };
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(new string([HokkeHigh, HokkeLow, VS17High, VS17Low])),
                ExpectedBaseRune = Hokke,
                ExpectedVariationSelector = VS17
            }
        };
    }

    [TestMethod]
    [DynamicData(nameof(ConstructorTestDataSamples))]
    public void ConstructorTest(ConstructorTestData data)
    {
        var kanji = data.KanjiFunc();
        Assert.AreEqual(data.ExpectedBaseRune, kanji.BaseRune);
        Assert.AreEqual(data.ExpectedVariationSelector, kanji.VariationSelector);
    }

    [TestMethod]
    public void ConstructorInvalidTest()
    {
        Assert.Throws<ArgumentException>(() => new KanjiChar(VS17High));

        Assert.Throws<ArgumentException>(() => new KanjiChar(Hirosi, Hirosi));

        Assert.Throws<ArgumentException>(() => new KanjiChar(VS17High));

        Assert.Throws<ArgumentException>(() => new KanjiChar(new string([]), VS17));
        Assert.Throws<ArgumentException>(() => new KanjiChar(new string([HirosiChar, HirosiChar]), VS17));
        Assert.Throws<ArgumentException>(() => new KanjiChar(new string([HirosiChar]), Hirosi));

        Assert.Throws<ArgumentException>(() => new KanjiChar(new string([])));
        Assert.Throws<ArgumentException>(() => new KanjiChar(new string([HirosiChar, HirosiChar])));
        Assert.Throws<ArgumentException>(() => new KanjiChar(new string([HirosiChar, VS17High, VS17Low, HirosiChar])));
    }

    public class EqualsTestData
    {
        public Func<Rune[]> A { get; set; }
        public Func<Rune[]> B { get; set; }
        public bool Expected { get; set; }
    }

    public static IEnumerable<object[]> EqualsTestDataSamples()
    {
        yield return new object[]
        {
            new EqualsTestData
            {
                A = () => new Rune[] { (Rune)'山', (Rune)'本', new Rune('博') },
                B = () => new Rune[] { (Rune)'山', (Rune)'本', new Rune('博'), new Rune(0xE0100) },
                Expected = true
            }
        };
        yield return new object[]
        {
            new EqualsTestData
            {
                A =() =>  new Rune[] { (Rune)'山', (Rune)'本', new Rune('博') },
                B = () => new Rune[] { (Rune)'山', (Rune)'本', new Rune('博'), new Rune(0xE0101) },
                Expected = true
            }
        };
        yield return new object[]
        {
            new EqualsTestData
            {
                A =() =>  new Rune[] { (Rune)'山', (Rune)'本', new Rune('博'), new Rune(0xE0100) },
                B = () => new Rune[] { (Rune)'山', (Rune)'本', new Rune('博'), new Rune(0xE0101) },
                Expected = true
            }
        };
        yield return new object[]
        {
            new EqualsTestData
            {
                A = () => new Rune[] { (Rune)'山', (Rune)'本', new Rune('博'), new Rune(0xE0100) },
                B = () => new Rune[] { (Rune)'山', (Rune)'本', new Rune('専'), new Rune(0xE0100) },
                Expected = false
            }
        };
    }
    [TestMethod]
    [DynamicData(nameof(EqualsTestDataSamples))]
    public void EqualsTest(EqualsTestData data)
    {
        var strA = data.A().Select(rune => rune.ToString()).Aggregate((a, b) => a + b);
        var strB = data.B().Select(rune => rune.ToString()).Aggregate((a, b) => a + b);
        Assert.AreEqual(data.Expected, ItaijiUtil.EqualsIgnoreIvs(strA, strB));
    }

    [TestMethod]
    public void ContainsIgnoreIvsTest()
    {
        Assert.IsTrue(ItaijiUtil.ContainsIgnoreIvs(
            new string(['私', 'は', '山', '本', '博', 'で', 'す']),
            new string(['山', '本', '博'])
        ));
        Assert.IsTrue(ItaijiUtil.ContainsIgnoreIvs(
            new string(['私', 'は', '山', '本', '博', VS17High, VS17Low, 'で', 'す']),
            new string(['山', '本', '博'])
        ));
        Assert.IsTrue(ItaijiUtil.ContainsIgnoreIvs(
            new string(['私', 'は', '山', '本', '博', 'で', 'す']),
            new string(['山', '本', '博', VS17High, VS17Low])
        ));
        Assert.IsTrue(ItaijiUtil.ContainsIgnoreIvs(
            new string(['私', VS17High, VS17Low, 'は', '山', '本', '博', 'で', 'す']),
            new string(['山', '本', '博'])
        ));
        Assert.IsFalse(ItaijiUtil.ContainsIgnoreIvs(
            new string(['私', 'は', '山', '本', '博', 'で', 'す']),
            new string(['山', '本', '専'])
        ));
    }

    [TestMethod]
    public void FindIndexIgnoreIvsTest()
    {
        Assert.AreEqual((2, 3), ItaijiUtil.FindIndexIgnoreIvs(
            new string(['私', 'は', '山', '本', '博', 'で', 'す']),
            new string(['山', '本', '博'])
        ));
        Assert.AreEqual((2, 5), ItaijiUtil.FindIndexIgnoreIvs(
            new string(['私', 'は', '山', '本', '博', VS17High, VS17Low, 'で', 'す']),
            new string(['山', '本', '博'])
        ));
        Assert.AreEqual((2, 3), ItaijiUtil.FindIndexIgnoreIvs(
            new string(['私', 'は', '山', '本', '博', 'で', 'す']),
            new string(['山', '本', '博', VS17High, VS17Low])
        ));
        Assert.AreEqual((4, 3), ItaijiUtil.FindIndexIgnoreIvs(
            new string(['私', VS17High, VS17Low, 'は', '山', '本', '博', 'で', 'す']),
            new string(['山', '本', '博'])
        ));
        Assert.AreEqual((-1, 0), ItaijiUtil.FindIndexIgnoreIvs(
            new string(['私', 'は', '山', '本', '博', 'で', 'す']),
            new string(['山', '本', '専'])
        ));
    }

    [TestMethod]
    [Timeout(100, CooperativeCancellation = true)]
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

    public class LengthData
    {
        public Func<KanjiChar> KanjiFunc { get; set; }
        public int ExpectedUtf8Length { get; set; }
        public int ExpectedUtf16Length { get; set; }
        public int ExpectedUtf32Length { get; set; }
    }

    public static IEnumerable<object[]> LengthDataSamples()
    {
        yield return new object[]
        {
            new LengthData
            {
                KanjiFunc = () => new KanjiChar('博'),
                ExpectedUtf8Length = 3,
                ExpectedUtf16Length = 1,
                ExpectedUtf32Length = 1
            }
        };
        yield return new object[]
        {
            new LengthData
            {
                KanjiFunc = () => new KanjiChar(new Rune(0x20000)),
                ExpectedUtf8Length = 4,
                ExpectedUtf16Length = 2,
                ExpectedUtf32Length = 1
            }
        };
        yield return new object[]
        {
            new LengthData
            {
                KanjiFunc = () => new KanjiChar('博', 0x00),
                ExpectedUtf8Length = 7,
                ExpectedUtf16Length = 3,
                ExpectedUtf32Length = 2
            }
        };
        yield return new object[]
{
            new LengthData
            {
                KanjiFunc = () => new KanjiChar(new Rune(0x20000), new Rune(0xE0100)),
                ExpectedUtf8Length = 8,
                ExpectedUtf16Length = 4,
                ExpectedUtf32Length = 2
            }
};
        yield return new object[]
        {
            new LengthData
            {
                KanjiFunc = () => default(KanjiChar), // '博' with VS2
                ExpectedUtf8Length = 1,
                ExpectedUtf16Length = 1,
                ExpectedUtf32Length = 1
            }
        };
    }

    [TestMethod]
    [DynamicData(nameof(LengthDataSamples))]
    public void Utf32SequenceLengthTest(LengthData dt)
    {
        Assert.AreEqual(dt.ExpectedUtf32Length, dt.KanjiFunc().Utf32SequenceLength);
    }

    [TestMethod]
    [DynamicData(nameof(LengthDataSamples))]
    public void Utf16SequenceLengthTest(LengthData dt)
    {
        Assert.AreEqual(dt.ExpectedUtf16Length, dt.KanjiFunc().Utf16SequenceLength);
    }

    [TestMethod]
    [DynamicData(nameof(LengthDataSamples))]
    public void Utf8SequenceLengthTest(LengthData dt)
    {
        Assert.AreEqual(dt.ExpectedUtf8Length, dt.KanjiFunc().Utf8SequenceLength);
    }

    [TestMethod]
    public void RemoveIvsTest()
    {
        Assert.AreEqual("あいうえお", ItaijiUtil.RemoveIvs("あいうえお"));
        Assert.AreEqual("山本博", ItaijiUtil.RemoveIvs(new string(['山', '本', '博', VS17High, VS17Low])));
        Assert.AreEqual("", ItaijiUtil.RemoveIvs(new string([VS17High, VS17Low])));

    }
}
