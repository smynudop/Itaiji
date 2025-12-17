#nullable disable
using Itaiji.Extensions;
using System.Diagnostics;
using System.Text;
#if NETFRAMEWORK
using Itaiji.Text;
#endif 
namespace Itaiji.Test;

public sealed partial class ItaijiTest
{
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
                KanjiFunc = () => new KanjiChar(VS1),
                ExpectedBaseRune = VS1,
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
                KanjiFunc = () => new KanjiChar(Kami, VS1),
                ExpectedBaseRune = Kami,
                ExpectedVariationSelector = VS1
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
        yield return new object[]
        {
            new ConstructorTestData
            {
                KanjiFunc = () => new KanjiChar(new string([KamiChar, VS1Char])),
                ExpectedBaseRune = Kami,
                ExpectedVariationSelector = VS1
            }
        };
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

    public class FindIndexTestData
    {
        public Func<string> Source { get; set; }
        public Func<string> Target { get; set; }
        public Func<(int, int)> ExpectedIndexAndLength { get; set; }
        public bool ExpectContainsExactly { get; set; }
        public bool ExpectContainsIgnoreIvs { get; set; }
    }

    public static IEnumerable<object[]> FindIndexTestDataSamples()
    {
        yield return new object[]
        {
            new FindIndexTestData
            {
                Source = () => new string (['私', 'は', '山', '本', '博', 'で', 'す']),
                Target = () => new string (['山', '本', '博']),
                ExpectedIndexAndLength = () => (2, 3),
                ExpectContainsExactly = true,
                ExpectContainsIgnoreIvs = true
            }
        };
        yield return new object[]
        {
            new FindIndexTestData
            {
                Source = () => new string (['私', 'は', '山', '本', '博', VS17High, VS17Low, 'で', 'す']),
                Target = () => new string (['山', '本', '博']),
                ExpectedIndexAndLength =  () => (2, 5),
                ExpectContainsExactly = false,
                ExpectContainsIgnoreIvs = true
            }
        };
        yield return new object[]
        {
            new FindIndexTestData
            {
                Source = () => new string (['私', 'は', '山', '本', '博', 'で', 'す']),
                Target = () => new string (['山', '本', '博', VS17High, VS17Low]),
                ExpectedIndexAndLength =  () => (2, 3),
                ExpectContainsExactly = false,
                ExpectContainsIgnoreIvs = true
            }
        };
        yield return new object[]
        {
            new FindIndexTestData
            {
                Source = () => new string (['私', VS17High, VS17Low, 'は', '山', '本', '博', 'で', 'す']),
                Target = () => new string (['山', '本', '博']),
                ExpectedIndexAndLength =  () => (4, 3),
                ExpectContainsExactly = true,
                ExpectContainsIgnoreIvs = true
            }
        };
        yield return new object[]
        {
            new FindIndexTestData
            {
                Source = () => new string (['私', 'は', '山', '本', '博', 'で', 'す']),
                Target = () => new string (['山', '本', '専']),
                ExpectedIndexAndLength =  () => (-1, 0),
                ExpectContainsExactly = false,
                ExpectContainsIgnoreIvs = false
            }
        };
    }

    public class TestData
    {
        public Func<KanjiChar> KanjiFunc { get; set; }
        public IvsCollectionType ExpectedIvsType { get; set; }
        public string ExpectedString { get; set; }
        public bool IsVariation { get; set; }
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
                ExpectedIvsType = IvsCollectionType.None,
                ExpectedString = char.ConvertFromUtf32(0x535A),
                IsVariation = false,
                IsValidAsAdobeJapan = true,
                IsValidAsHanyoDenshi = true,
                IsValidAsMojiJoho = true
            }
        };
        yield return new object[]
        {
            new TestData {
                KanjiFunc = () => new KanjiChar('博', 0x00),
                ExpectedIvsType = IvsCollectionType.AdobeJapan,
                ExpectedString = char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE0100),
                IsVariation = true,
                IsValidAsAdobeJapan = true,
                IsValidAsHanyoDenshi = false,
                IsValidAsMojiJoho = false
            }
        };
        yield return new object[]
        {
            new TestData {
                KanjiFunc =  () => new KanjiChar('博', 0x02),
                ExpectedIvsType = IvsCollectionType.HDandMJ,
                ExpectedString=  char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE0102),
                IsVariation = true,
                IsValidAsAdobeJapan= false,
                IsValidAsHanyoDenshi= true,
                IsValidAsMojiJoho= true
            }
        };
        yield return new object[]
        {
            new TestData {
                KanjiFunc = () =>  new KanjiChar('博', 0x07),
                ExpectedIvsType = IvsCollectionType.HanyoDenshi,
                ExpectedString = char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE0107),
                IsVariation = true,
                IsValidAsAdobeJapan = false,
                IsValidAsHanyoDenshi = true,
                IsValidAsMojiJoho = false
            }
        };
        yield return new object[] {
            new TestData {

                KanjiFunc = () => new KanjiChar('博', 0x0A),
                ExpectedIvsType= IvsCollectionType.MojiJoho,
                ExpectedString = char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE010A),
                IsVariation= true,
                IsValidAsAdobeJapan = false,
                IsValidAsHanyoDenshi = false,
                IsValidAsMojiJoho = true
            }
        };
        yield return new object[] {
            new TestData {
                KanjiFunc = () => new KanjiChar('博', 0x10),
                ExpectedIvsType =  IvsCollectionType.Unknown,
                ExpectedString = char.ConvertFromUtf32(0x535A) + char.ConvertFromUtf32(0xE0110),
                IsVariation= true,
                IsValidAsAdobeJapan= false,
                IsValidAsHanyoDenshi= false,
                IsValidAsMojiJoho= false
            }
        };
        yield return new object[] {
            new TestData {
                KanjiFunc = () => new KanjiChar('神', 0xF0),
                ExpectedIvsType =  IvsCollectionType.CJKCompatibilityIdeographs,
                ExpectedString = char.ConvertFromUtf32(0x795E) + char.ConvertFromUtf32(0xFE00),
                IsVariation= true,
                IsValidAsAdobeJapan= false,
                IsValidAsHanyoDenshi= false,
                IsValidAsMojiJoho= false,
            }
        };
        yield return new object[] {
            new TestData {
                KanjiFunc = () => new KanjiChar('神', 0xF2),
                ExpectedIvsType =  IvsCollectionType.Unknown,
                ExpectedString = char.ConvertFromUtf32(0x795E) + char.ConvertFromUtf32(0xFE02),
                IsVariation= true,
                IsValidAsAdobeJapan= false,
                IsValidAsHanyoDenshi= false,
                IsValidAsMojiJoho= false,
            }
        };
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
                KanjiFunc = () => default(KanjiChar),
                ExpectedUtf8Length = 1,
                ExpectedUtf16Length = 1,
                ExpectedUtf32Length = 1
            }
        };
    }

    public class EnumerateTestData
    {
        public Func<string> Source { get; set; }
        public Func<List<KanjiChar>> ExpectedKanjiChars { get; set; }
    }

    public static IEnumerable<object[]> EnumerateTestDataSamples()
    {
        yield return new object[]
        {
            new EnumerateTestData
            {
                Source = () => new string (['山', '本', '博', VS17High, VS17Low]),
                ExpectedKanjiChars = () => new List<KanjiChar>
                {
                    new KanjiChar('山'),
                    new KanjiChar('本'),
                    new KanjiChar(new Rune('博'), VS17),
                }
            }
        };
        yield return new object[]
        {
            new EnumerateTestData
            {
                Source = () => new string (['山', '本', '神', VS1Char]),
                ExpectedKanjiChars = () => new List<KanjiChar>
                {
                    new KanjiChar('山'),
                    new KanjiChar('本'),
                    new KanjiChar(new Rune('神'), VS1),
                }
            }
        };
    }

    public class CIConvertTestData
    {
        public Func<string> CIRune { get; set; } 
        public Func<string> SvsRune { get; set; } 
        public Func<string> AdobeJapan1IvsRune { get; set; } 
        public Func<string> MojiJohoIvsRune { get; set; } 
    }

    public static IEnumerable<object[]> CIConvertTestDataSamples()
    {
        yield return new object[]
        {
            new CIConvertTestData
            {
                CIRune = () => $"私は{new Rune(0xFA19)}だ",
                SvsRune = () => $"私は{new Rune(0x795E)}{new Rune(0xFE00)}だ",
                AdobeJapan1IvsRune = () =>  $"私は{new Rune(0x795E)}{new Rune(0xE0100)}だ",
                MojiJohoIvsRune = () => $"私は{new Rune(0x795E)}{new Rune(0xE0103)}だ",
            }
        };
        yield return new object[]
{
            new CIConvertTestData
            {
                CIRune = () => $"私は{new Rune(0x795E)}{new Rune(0xFE00)}だ",
                SvsRune = () => $"私は{new Rune(0x795E)}{new Rune(0xFE00)}だ",
                AdobeJapan1IvsRune = () =>  $"私は{new Rune(0x795E)}{new Rune(0xE0100)}だ",
                MojiJohoIvsRune = () => $"私は{new Rune(0x795E)}{new Rune(0xE0103)}だ",
            }
};
        yield return new object[]
        {
            new CIConvertTestData
            {
                CIRune = () => string.Format("{0}{0}{0}", new Rune(0xFA19)),
                SvsRune = () => string.Format("{0}{0}{0}", new Rune(0x795E).ToString() + new Rune(0xFE00).ToString()),
                AdobeJapan1IvsRune = () => string.Format("{0}{0}{0}", new Rune(0x795E).ToString() + new Rune(0xE0100).ToString()),
                MojiJohoIvsRune = () => string.Format("{0}{0}{0}", new Rune(0x795E).ToString() + new Rune(0xE0103).ToString()),
            }
        };
    }

    public class RemoveIvsTestData
    {
        public Func<string> Source { get; set; }
        public Func<string> ExpectedRemoveAll { get; set; }
        public Func<string> ExpectedRemoveOnlyIvs { get; set; }
        public Func<string> ExpectedRemoveOnlyIvsToSvs { get; set; }

    }

    public static IEnumerable<object[]> RemoveIvsTestDataSamples()
    {
        yield return new object[]
        {
            new RemoveIvsTestData
            {
                Source = () => new string (['山', '本', '博', VS17High, VS17Low, '神', VS1Char]),
                ExpectedRemoveAll = () => new string (['山', '本', '博', '神']),
                ExpectedRemoveOnlyIvs = () => new string (['山', '本', '博', '神', VS1Char]),
                ExpectedRemoveOnlyIvsToSvs = () => new string (['山', '本', '博', '神', VS1Char]),
            }
        };
        yield return new object[]
        {
            new RemoveIvsTestData
            {
                Source = () => new string (['山', '本', '神', VS17High, VS17Low]),
                ExpectedRemoveAll = () => new string (['山', '本', '神']),
                ExpectedRemoveOnlyIvs = () => new string (['山', '本', '神']),
                ExpectedRemoveOnlyIvsToSvs = () => new string (['山', '本', '神', VS1Char]),
            }
        };

        yield return new object[]
{
            new RemoveIvsTestData
            {
                Source = () => new string (['山', '本', '神', VS20High, VS20Low]),
                ExpectedRemoveAll = () => new string (['山', '本', '神']),
                ExpectedRemoveOnlyIvs = () => new string (['山', '本', '神']),
                ExpectedRemoveOnlyIvsToSvs = () => new string (['山', '本', '神', VS1Char]),
            }
};
        yield return new object[]
{
            new RemoveIvsTestData
            {
                Source = () => new string ([VS17High, VS17Low]),
                ExpectedRemoveAll = () => new string ([]),
                ExpectedRemoveOnlyIvs = () => new string ([]),
                ExpectedRemoveOnlyIvsToSvs = () => new string ([]),
            }
};
        yield return new object[]
{
            new RemoveIvsTestData
            {
                Source = () => new string ([VS1Char]),
                ExpectedRemoveAll = () => new string ([]),
                ExpectedRemoveOnlyIvs = () => new string ([VS1Char]),
                ExpectedRemoveOnlyIvsToSvs = () => new string ([VS1Char]),
            }
};
    }
}