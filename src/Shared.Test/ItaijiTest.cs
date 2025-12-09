#nullable disable
using Itaiji.Extensions;
using System.Diagnostics;
using System.Text;
#if NETFRAMEWORK
using Itaiji.Text;
#endif 
namespace Itaiji.Test;

[TestClass]
public sealed partial class ItaijiTest
{
    public const char HirosiChar = (char)0x535A;
    public const char HokkeHigh = (char)0xD867;
    public const char HokkeLow = (char)0xDE3D;
    public const char KamiChar = '神';
    public const char VS1Char = (char)0xFE00;

    public const char VS17High = (char)0xDB40;
    public const char VS17Low = (char)0xDD00;

    public static Rune Hirosi = new Rune('博');
    public static Rune Hokke => new Rune(0x29E3D); // 𩸽
    public static Rune VS17 => new Rune(0xE0100); // 異体字セレクタE0100
    public static Rune Kami = new Rune('神');
    public static Rune VS1 => new Rune(0xFE00); // 異体字セレクタ1



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


    [TestMethod]
    [DynamicData(nameof(EqualsTestDataSamples))]
    public void EqualsTest(EqualsTestData data)
    {
        var strA = data.A().Select(rune => rune.ToString()).Aggregate((a, b) => a + b);
        var strB = data.B().Select(rune => rune.ToString()).Aggregate((a, b) => a + b);
        Assert.AreEqual(data.Expected, ItaijiUtility.EqualsIgnoreIvs(strA, strB));
    }

    [TestMethod]
    [DynamicData(nameof(FindIndexTestDataSamples))]
    public void ContainsIgnoreIvsTest(FindIndexTestData data)
    {
        Assert.AreEqual(data.ExpectContainsIgnoreIvs, ItaijiUtility.Contains(data.Source(), data.Target(), IvsComparison.IgnoreIvs));
        Assert.AreEqual(data.ExpectContainsExactly, ItaijiUtility.Contains(data.Source(), data.Target(), IvsComparison.ExactMatch));
    }

    [TestMethod]
    [DynamicData(nameof(FindIndexTestDataSamples))]
    public void FindIndexIgnoreIvsTest(FindIndexTestData data)
    {
        Assert.AreEqual(data.ExpectedIndexAndLength(), ItaijiUtility.FindIndex(data.Source(), data.Target(), IvsComparison.IgnoreIvs));
    }

    [TestMethod]
    [DynamicData(nameof(EnumerateTestDataSamples))]
    public void EnumerateKanjiTest(EnumerateTestData data)
    {
        CollectionAssert.AreEqual(data.ExpectedKanjiChars(), data.Source().EnumerateKanji().ToList());
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
    public void ValidTest(TestData dt)
    {
        var str = dt.KanjiFunc().ToString();
        Assert.AreEqual(!dt.IsValidAsAdobeJapan, str.HasInvalidVariationSelectorAsAdobeJapan1());
        Assert.AreEqual(!dt.IsValidAsHanyoDenshi, str.HasInvalidVariationSelectorAsHanyoDenshi());
        Assert.AreEqual(!dt.IsValidAsMojiJoho, str.HasInvalidVariationSelectorAsMojiJoho());
    }



    [TestMethod]
    [DynamicData(nameof(IvsSamples))]
    public void IvsTypeTest(TestData dt)
    {
        var kanji = dt.KanjiFunc();
        Assert.AreEqual(dt.ExpectedIvsType, kanji.GetVsCollectionType());
    }

    [TestMethod]
    [DynamicData(nameof(IvsSamples))]
    public void ToStringTest(TestData dt)
    {
        var kanji = dt.KanjiFunc();
        Assert.AreEqual(dt.ExpectedString, kanji.ToString());
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
        Assert.AreEqual("あいうえお", ItaijiUtility.RemoveIvs("あいうえお"));
        Assert.AreEqual("山本博", ItaijiUtility.RemoveIvs(new string(['山', '本', '博', VS17High, VS17Low])));
        Assert.AreEqual("", ItaijiUtility.RemoveIvs(new string([VS17High, VS17Low])));

    }

    [TestMethod]
    public void Memory_StandardizedVariants()
    {
        // 測定前の確保バイト数を取得
        long allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();

        // --- 測定対象 ---
        var largeHashSet = Library.StandardizedVariants;
        // --- 測定対象 ---

        // 測定後の確保バイト数を取得
        long allocatedBytesAfter = GC.GetAllocatedBytesForCurrentThread();

        long memoryAllocated = allocatedBytesAfter - allocatedBytesBefore;

        Debug.WriteLine($"StandardizedVariants: {memoryAllocated / 1024.0:N2} KB");

        Assert.IsGreaterThan(0, memoryAllocated);

        // このメソッドはGCの実行を必要としないため、よりクリーンです。
        // GC.KeepAlive(largeHashSet) を呼んでおくと、最適化でオブジェクトが消されるのを防げます。
        GC.KeepAlive(largeHashSet);

    }

    [TestMethod]
    public void Memory_JpIvsList()
    {
        // 測定前の確保バイト数を取得
        long allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();

        // --- 測定対象 ---
        var largeHashSet = Library.JpIvsList;
        // --- 測定対象 ---

        // 測定後の確保バイト数を取得
        long allocatedBytesAfter = GC.GetAllocatedBytesForCurrentThread();

        long memoryAllocated = allocatedBytesAfter - allocatedBytesBefore;

        Debug.WriteLine($"StandardizedVariants: {memoryAllocated / 1024.0:N2} KB");

        Assert.IsGreaterThan(0, memoryAllocated);

        // このメソッドはGCの実行を必要としないため、よりクリーンです。
        // GC.KeepAlive(largeHashSet) を呼んでおくと、最適化でオブジェクトが消されるのを防げます。
        GC.KeepAlive(largeHashSet);

    }
}
