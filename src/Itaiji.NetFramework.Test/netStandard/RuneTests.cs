// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using Itaiji.NetFramework;
using Itaiji.Text;
using System.Globalization;

namespace Kanji.Test
{
    [TestClass]
    public sealed partial class RuneTests
    {
        [DataRow('0', '0', '0', "en-US")]
        [DataRow('a', 'A', 'a', "en-US")]
        [DataRow('i', 'I', 'i', "en-US")]
        [DataRow('i', '\u0130', 'i', "tr-TR")]
        [DataRow('z', 'Z', 'z', "en-US")]
        [DataRow('A', 'A', 'a', "en-US")]
        [DataRow('I', 'I', 'i', "en-US")]
        [DataRow('I', 'I', '\u0131', "tr-TR")]
        [DataRow('Z', 'Z', 'z', "en-US")]
        [DataRow('\u00DF', '\u00DF', '\u00DF', "de-DE")] // U+00DF LATIN SMALL LETTER SHARP S -- n.b. ToUpper doesn't create the majuscule form
        [DataRow('\u0130', '\u0130', 'i', "tr-TR")] // U+0130 LATIN CAPITAL LETTER I WITH DOT ABOVE
        [DataRow('\u0131', 'I', '\u0131', "tr-TR")] // U+0131 LATIN SMALL LETTER DOTLESS I
        [DataRow('\u1E9E', '\u1E9E', '\u00DF', "de-DE")] // U+1E9E LATIN CAPITAL LETTER SHARP S
        [DataRow(0x10400, 0x10400, 0x10428, "en-US")] // U+10400 DESERET CAPITAL LETTER LONG I
        [DataRow(0x10428, 0x10400, 0x10428, "en-US")] // U+10428 DESERET SMALL LETTER LONG I
        [TestMethod]
        public void Casing_CultureAware(int original, int upper, int lower, string culture)
        {
            var rune = new Rune(original);
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            Assert.AreEqual(new Rune(upper), Rune.ToUpper(rune, cultureInfo));
            Assert.AreEqual(new Rune(lower), Rune.ToLower(rune, cultureInfo));
        }

        // Invariant ToUpper / ToLower doesn't modify Turkish I or majuscule Eszett
        [DataRow('0', '0', '0')]
        [DataRow('a', 'A', 'a')]
        [DataRow('i', 'I', 'i')]
        [DataRow('z', 'Z', 'z')]
        [DataRow('A', 'A', 'a')]
        [DataRow('I', 'I', 'i')]
        [DataRow('Z', 'Z', 'z')]
        [DataRow('\u00DF', '\u00DF', '\u00DF')] // U+00DF LATIN SMALL LETTER SHARP S
        [DataRow('\u0130', '\u0130', '\u0130')] // U+0130 LATIN CAPITAL LETTER I WITH DOT ABOVE
        [DataRow('\u0131', '\u0131', '\u0131')] // U+0131 LATIN SMALL LETTER DOTLESS I
        [DataRow('\u1E9E', '\u1E9E', '\u1E9E')] // U+1E9E LATIN CAPITAL LETTER SHARP S
        [TestMethod]
        public void Casing_Invariant(int original, int upper, int lower)
        {
            var rune = new Rune(original);
            Assert.AreEqual(new Rune(upper), Rune.ToUpperInvariant(rune));
            Assert.AreEqual(new Rune(lower), Rune.ToLowerInvariant(rune));
        }

        // HybridGlobalization on Apple mobile platforms has issues with casing dotless I
        [DataRow('0', '0', '0')]
        [DataRow('a', 'A', 'a')]
        [DataRow('i', 'I', 'i')]
        [DataRow('z', 'Z', 'z')]
        [DataRow('A', 'A', 'a')]
        [DataRow('I', 'I', 'i')]
        [DataRow('Z', 'Z', 'z')]
        [DataRow('\u00DF', '\u00DF', '\u00DF')] // U+00DF LATIN SMALL LETTER SHARP S
        [DataRow('\u0130', '\u0130', '\u0130')] // U+0130 LATIN CAPITAL LETTER I WITH DOT ABOVE
        [DataRow('\u0131', '\u0131', '\u0131')] // U+0131 LATIN SMALL LETTER DOTLESS I
        [DataRow(0x10400, 0x10400, 0x10428)] // U+10400 DESERET CAPITAL LETTER LONG I
        [DataRow(0x10428, 0x10400, 0x10428)] // U+10428 DESERET SMALL LETTER LONG I
        [TestMethod]
        public void ICU_Casing_Invariant(int original, int upper, int lower)
        {
            var rune = new Rune(original);
            Assert.AreEqual(new Rune(upper), Rune.ToUpperInvariant(rune));
            Assert.AreEqual(new Rune(lower), Rune.ToLowerInvariant(rune));
        }

        [TestMethod]
        [DynamicData(nameof(GeneralTestData_BmpCodePoints_NoSurrogates))]
        public void Ctor_Cast_Char_Valid(GeneralTestData testData)
        {
            Rune rune = new Rune(checked((char)testData.ScalarValue));
            Rune runeFromCast = (Rune)(char)testData.ScalarValue;

            Assert.AreEqual(rune, runeFromCast);
            Assert.AreEqual(testData.ScalarValue, rune.Value);
            Assert.AreEqual(testData.IsAscii, rune.IsAscii);
            Assert.AreEqual(testData.IsBmp, rune.IsBmp);
            Assert.AreEqual(testData.Plane, rune.Plane);
        }

        [TestMethod]
        [DynamicData(nameof(BmpCodePoints_SurrogatesOnly))]
        public void Ctor_Cast_Char_Invalid_Throws(char ch)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Rune(ch));
            Assert.Throws<ArgumentOutOfRangeException>(() => (Rune)ch);
        }

        [TestMethod]
        [DynamicData(nameof(GeneralTestData_BmpCodePoints_NoSurrogates))]
        [DynamicData(nameof(GeneralTestData_SupplementaryCodePoints_ValidOnly))]
        public void Ctor_Cast_Int32_Valid(GeneralTestData testData)
        {
            Rune rune = new Rune((int)testData.ScalarValue);
            Rune runeFromCast = (Rune)(int)testData.ScalarValue;

            Assert.AreEqual(rune, runeFromCast);
            Assert.AreEqual(testData.ScalarValue, rune.Value);
            Assert.AreEqual(testData.IsAscii, rune.IsAscii);
            Assert.AreEqual(testData.IsBmp, rune.IsBmp);
            Assert.AreEqual(testData.Plane, rune.Plane);
        }

        [TestMethod]
        [DynamicData(nameof(BmpCodePoints_SurrogatesOnly))]
        [DynamicData(nameof(SupplementaryCodePoints_InvalidOnly))]
        public void Ctor_Cast_Int32_Invalid_Throws(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Rune(value));
            Assert.Throws<ArgumentOutOfRangeException>(() => (Rune)value);
        }

        [TestMethod]
        [DynamicData(nameof(GeneralTestData_BmpCodePoints_NoSurrogates))]
        [DynamicData(nameof(GeneralTestData_SupplementaryCodePoints_ValidOnly))]
        public void Ctor_Cast_UInt32_Valid(GeneralTestData testData)
        {
            Rune rune = new Rune((uint)testData.ScalarValue);
            Rune runeFromCast = (Rune)(uint)testData.ScalarValue;

            Assert.AreEqual(rune, runeFromCast);
            Assert.AreEqual(testData.ScalarValue, rune.Value);
            Assert.AreEqual(testData.IsAscii, rune.IsAscii);
            Assert.AreEqual(testData.IsBmp, rune.IsBmp);
            Assert.AreEqual(testData.Plane, rune.Plane);
        }

        [TestMethod]
        [DynamicData(nameof(BmpCodePoints_SurrogatesOnly))]
        [DynamicData(nameof(SupplementaryCodePoints_InvalidOnly))]
        public void Ctor_Cast_UInt32_Invalid_Throws(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Rune((uint)value));
            Assert.Throws<ArgumentOutOfRangeException>(() => (Rune)(uint)value);
        }

        [TestMethod]
        [DynamicData(nameof(SurrogatePairTestData_ValidOnly))]
        public void Ctor_SurrogatePair_Valid(char highSurrogate, char lowSurrogate, int expectedValue)
        {
            Assert.AreEqual(expectedValue, new Rune(highSurrogate, lowSurrogate).Value);
        }

        [TestMethod]
        [DynamicData(nameof(SurrogatePairTestData_InvalidOnly))]
        public void Ctor_SurrogatePair_Invalid(char highSurrogate, char lowSurrogate)
        {
            string expectedParamName = !char.IsHighSurrogate(highSurrogate) ? nameof(highSurrogate) : nameof(lowSurrogate);
            Assert.Throws<ArgumentOutOfRangeException>(() => new Rune(highSurrogate, lowSurrogate));
        }

        [TestMethod]
        [DataRow('A', 'a', -1)]
        [DataRow('A', 'A', 0)]
        [DataRow('a', 'A', 1)]
        [DataRow(0x10000, 0x10000, 0)]
        [DataRow('\uFFFD', 0x10000, -1)]
        [DataRow(0x10FFFF, 0x10000, 1)]
        public void CompareTo_And_ComparisonOperators(int first, int other, int expectedSign)
        {
            Rune a = new Rune(first);
            Rune b = new Rune(other);

            Assert.AreEqual(expectedSign, Math.Sign(a.CompareTo(b)));
            Assert.AreEqual(expectedSign, Math.Sign(((IComparable)a).CompareTo(b)));
            Assert.AreEqual(expectedSign < 0, a < b);
            Assert.AreEqual(expectedSign <= 0, a <= b);
            Assert.AreEqual(expectedSign > 0, a > b);
            Assert.AreEqual(expectedSign >= 0, a >= b);
        }

        [TestMethod]
        [DataRow(new char[0], OperationStatus.NeedMoreData, 0xFFFD, 0)] // empty buffer
        [DataRow(new char[] { '\u1234' }, OperationStatus.Done, 0x1234, 1)] // BMP char
        [DataRow(new char[] { '\u1234', '\ud800' }, OperationStatus.Done, 0x1234, 1)] // BMP char
        [DataRow(new char[] { '\ud83d', '\ude32' }, OperationStatus.Done, 0x1F632, 2)] // supplementary value (U+1F632 ASTONISHED FACE)
        [DataRow(new char[] { '\udc00' }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone low surrogate
        [DataRow(new char[] { '\udc00', '\udc00' }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone low surrogate
        [DataRow(new char[] { '\ud800' }, OperationStatus.NeedMoreData, 0xFFFD, 1)] // high surrogate at end of buffer
        [DataRow(new char[] { '\ud800', '\ud800' }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone high surrogate
        [DataRow(new char[] { '\ud800', '\u1234' }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone high surrogate
        public void DecodeFromUtf16(char[] data, OperationStatus expectedOperationStatus, int expectedRuneValue, int expectedCharsConsumed)
        {
            Assert.AreEqual<OperationStatus>(expectedOperationStatus, Rune.DecodeFromUtf16(data, out Rune actualRune, out int actualCharsConsumed));
            Assert.AreEqual(expectedRuneValue, actualRune.Value);
            Assert.AreEqual(expectedCharsConsumed, actualCharsConsumed);
        }

        [TestMethod]
        [DataRow(new char[0], OperationStatus.NeedMoreData, 0xFFFD, 0)] // empty buffer
        [DataRow(new char[] { '\u1234', '\u5678' }, OperationStatus.Done, 0x5678, 1)] // BMP char
        [DataRow(new char[] { '\udc00', '\ud800' }, OperationStatus.NeedMoreData, 0xFFFD, 1)] // high surrogate at end of buffer
        [DataRow(new char[] { '\ud83d', '\ude32' }, OperationStatus.Done, 0x1F632, 2)] // supplementary value (U+1F632 ASTONISHED FACE)
        [DataRow(new char[] { '\u1234', '\udc00' }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone low surrogate
        [DataRow(new char[] { '\udc00' }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone low surrogate
        public void DecodeLastFromUtf16(char[] data, OperationStatus expectedOperationStatus, int expectedRuneValue, int expectedCharsConsumed)
        {
            Assert.AreEqual(expectedOperationStatus, Rune.DecodeLastFromUtf16(data, out Rune actualRune, out int actualCharsConsumed));
            Assert.AreEqual(expectedRuneValue, actualRune.Value);
            Assert.AreEqual(expectedCharsConsumed, actualCharsConsumed);
        }

        [TestMethod]
        [DataRow(new byte[0], OperationStatus.NeedMoreData, 0xFFFD, 0)] // empty buffer
        [DataRow(new byte[] { 0x30 }, OperationStatus.Done, 0x0030, 1)] // ASCII byte
        [DataRow(new byte[] { 0x30, 0x40, 0x50 }, OperationStatus.Done, 0x0030, 1)] // ASCII byte
        [DataRow(new byte[] { 0x80 }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone continuation byte
        [DataRow(new byte[] { 0x80, 0x80, 0x80 }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone continuation byte
        [DataRow(new byte[] { 0xC1 }, OperationStatus.InvalidData, 0xFFFD, 1)] // C1 is never a valid UTF-8 byte
        [DataRow(new byte[] { 0xF5 }, OperationStatus.InvalidData, 0xFFFD, 1)] // F5 is never a valid UTF-8 byte
        [DataRow(new byte[] { 0xC2 }, OperationStatus.NeedMoreData, 0xFFFD, 1)] // C2 is a valid byte; expecting it to be followed by a continuation byte
        [DataRow(new byte[] { 0xED }, OperationStatus.NeedMoreData, 0xFFFD, 1)] // ED is a valid byte; expecting it to be followed by a continuation byte
        [DataRow(new byte[] { 0xF4 }, OperationStatus.NeedMoreData, 0xFFFD, 1)] // F4 is a valid byte; expecting it to be followed by a continuation byte
        [DataRow(new byte[] { 0xC2, 0xC2 }, OperationStatus.InvalidData, 0xFFFD, 1)] // C2 not followed by continuation byte
        [DataRow(new byte[] { 0xC3, 0x90 }, OperationStatus.Done, 0x00D0, 2)] // [ C3 90 ] is U+00D0 LATIN CAPITAL LETTER ETH
        [DataRow(new byte[] { 0xC1, 0xBF }, OperationStatus.InvalidData, 0xFFFD, 1)] // [ C1 BF ] is overlong 2-byte sequence, all overlong sequences have maximal invalid subsequence length 1
        [DataRow(new byte[] { 0xE0, 0x9F }, OperationStatus.InvalidData, 0xFFFD, 1)] // [ E0 9F ] is overlong 3-byte sequence, all overlong sequences have maximal invalid subsequence length 1
        [DataRow(new byte[] { 0xE0, 0xA0 }, OperationStatus.NeedMoreData, 0xFFFD, 2)] // [ E0 A0 ] is valid 2-byte start of 3-byte sequence
        [DataRow(new byte[] { 0xED, 0x9F }, OperationStatus.NeedMoreData, 0xFFFD, 2)] // [ ED 9F ] is valid 2-byte start of 3-byte sequence
        [DataRow(new byte[] { 0xED, 0xBF }, OperationStatus.InvalidData, 0xFFFD, 1)] // [ ED BF ] would place us in UTF-16 surrogate range, all surrogate sequences have maximal invalid subsequence length 1
        [DataRow(new byte[] { 0xEE, 0x80 }, OperationStatus.NeedMoreData, 0xFFFD, 2)] // [ EE 80 ] is valid 2-byte start of 3-byte sequence
        [DataRow(new byte[] { 0xF0, 0x8F }, OperationStatus.InvalidData, 0xFFFD, 1)] // [ F0 8F ] is overlong 4-byte sequence, all overlong sequences have maximal invalid subsequence length 1
        [DataRow(new byte[] { 0xF0, 0x90 }, OperationStatus.NeedMoreData, 0xFFFD, 2)] // [ F0 90 ] is valid 2-byte start of 4-byte sequence
        [DataRow(new byte[] { 0xF4, 0x90 }, OperationStatus.InvalidData, 0xFFFD, 1)] // [ F4 90 ] would place us beyond U+10FFFF, all such sequences have maximal invalid subsequence length 1
        [DataRow(new byte[] { 0xE2, 0x88, 0xB4 }, OperationStatus.Done, 0x2234, 3)] // [ E2 88 B4 ] is U+2234 THEREFORE
        [DataRow(new byte[] { 0xE2, 0x88, 0xC0 }, OperationStatus.InvalidData, 0xFFFD, 2)] // [ E2 88 ] followed by non-continuation byte, maximal invalid subsequence length 2
        [DataRow(new byte[] { 0xF0, 0x9F, 0x98 }, OperationStatus.NeedMoreData, 0xFFFD, 3)] // [ F0 9F 98 ] is valid 3-byte start of 4-byte sequence
        [DataRow(new byte[] { 0xF0, 0x9F, 0x98, 0x20 }, OperationStatus.InvalidData, 0xFFFD, 3)] // [ F0 9F 98 ] followed by non-continuation byte, maximal invalid subsequence length 3
        [DataRow(new byte[] { 0xF0, 0x9F, 0x98, 0xB2 }, OperationStatus.Done, 0x1F632, 4)] // [ F0 9F 98 B2 ] is U+1F632 ASTONISHED FACE
        public void DecodeFromUtf8(byte[] data, OperationStatus expectedOperationStatus, int expectedRuneValue, int expectedBytesConsumed)
        {
            Assert.AreEqual(expectedOperationStatus, Rune.DecodeFromUtf8(data, out Rune actualRune, out int actualBytesConsumed));
            Assert.AreEqual(expectedRuneValue, actualRune.Value);
            Assert.AreEqual(expectedBytesConsumed, actualBytesConsumed);
        }

        //[TestMethod]
        //[DataRow(new byte[] { 0x30 }, 0x0030)] // ASCII byte
        //[DataRow(new byte[] { 0xC3, 0x90 }, 0x00D0)] // [ C3 90 ] is U+00D0 LATIN CAPITAL LETTER ETH
        //[DataRow(new byte[] { 0xE2, 0x88, 0xB4 }, 0x2234)] // [ E2 88 B4 ] is U+2234 THEREFORE
        //[DataRow(new byte[] { 0xF0, 0x9F, 0x98, 0xB2 }, 0x1F632)] // [ F0 9F 98 B2 ] is U+1F632 ASTONISHED FACE
        //public void ParseUtf8(byte[] data, int expectedRuneValue)
        //{
        //    Assert.AreEqual(expectedRuneValue, Utf8SpanParsableHelper<Rune>.Parse(data, null).Value);
        //    Assert.IsTrue(Utf8SpanParsableHelper<Rune>.TryParse(data, null, out Rune actualRune));
        //    Assert.AreEqual(expectedRuneValue, actualRune.Value);
        //}

        //[TestMethod]
        //[DataRow(new byte[0])] // empty buffer
        //[DataRow(new byte[] { 0x30, 0x40, 0x50 })] // Multiple ASCII bytes
        //[DataRow(new byte[] { 0x80 })] // standalone continuation byte
        //[DataRow(new byte[] { 0x80, 0x80, 0x80 })] // standalone continuation byte
        //[DataRow(new byte[] { 0xC1 })] // C1 is never a valid UTF-8 byte
        //[DataRow(new byte[] { 0xF5 })] // F5 is never a valid UTF-8 byte
        //[DataRow(new byte[] { 0xC2 })] // C2 is a valid byte; expecting it to be followed by a continuation byte
        //[DataRow(new byte[] { 0xED })] // ED is a valid byte; expecting it to be followed by a continuation byte
        //[DataRow(new byte[] { 0xF4 })] // F4 is a valid byte; expecting it to be followed by a continuation byte
        //[DataRow(new byte[] { 0xC2, 0xC2 })] // C2 not followed by continuation byte
        //[DataRow(new byte[] { 0xC1, 0xBF })] // [ C1 BF ] is overlong 2-byte sequence, all overlong sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0xE0, 0x9F })] // [ E0 9F ] is overlong 3-byte sequence, all overlong sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0xE0, 0xA0 })] // [ E0 A0 ] is valid 2-byte start of 3-byte sequence
        //[DataRow(new byte[] { 0xED, 0x9F })] // [ ED 9F ] is valid 2-byte start of 3-byte sequence
        //[DataRow(new byte[] { 0xED, 0xBF })] // [ ED BF ] would place us in UTF-16 surrogate range, all surrogate sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0xEE, 0x80 })] // [ EE 80 ] is valid 2-byte start of 3-byte sequence
        //[DataRow(new byte[] { 0xF0, 0x8F })] // [ F0 8F ] is overlong 4-byte sequence, all overlong sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0xF0, 0x90 })] // [ F0 90 ] is valid 2-byte start of 4-byte sequence
        //[DataRow(new byte[] { 0xF4, 0x90 })] // [ F4 90 ] would place us beyond U+10FFFF, all such sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0xE2, 0x88, 0xC0 })] // [ E2 88 ] followed by non-continuation byte, maximal invalid subsequence length 2
        //[DataRow(new byte[] { 0xF0, 0x9F, 0x98 })] // [ F0 9F 98 ] is valid 3-byte start of 4-byte sequence
        //[DataRow(new byte[] { 0xF0, 0x9F, 0x98, 0x20 })] // [ F0 9F 98 ] followed by non-continuation byte, maximal invalid subsequence length 3
        //public void ParseUtf8_Invalid(byte[] data)
        //{
        //    Assert.Throws<FormatException>(() => Utf8SpanParsableHelper<Rune>.Parse(data, null));
        //    Assert.IsFalse(Utf8SpanParsableHelper<Rune>.TryParse(data, null, out Rune actualRune));
        //    Assert.AreEqual(Rune.ReplacementChar, actualRune);
        //}

        [TestMethod]
        [DataRow(new byte[0], OperationStatus.NeedMoreData, 0xFFFD, 0)] // empty buffer
        [DataRow(new byte[] { 0x30 }, OperationStatus.Done, 0x0030, 1)] // ASCII byte
        [DataRow(new byte[] { 0x30, 0x40, 0x50 }, OperationStatus.Done, 0x0050, 1)] // ASCII byte
        [DataRow(new byte[] { 0x80 }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone continuation byte
        [DataRow(new byte[] { 0x80, 0x80, 0x80 }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone continuation byte
        [DataRow(new byte[] { 0x80, 0x80, 0x80, 0x80 }, OperationStatus.InvalidData, 0xFFFD, 1)] // standalone continuation byte
        [DataRow(new byte[] { 0x80, 0x80, 0x80, 0xC2 }, OperationStatus.NeedMoreData, 0xFFFD, 1)] // [ C2 ] at end of buffer, valid 1-byte start of 2-byte sequence
        [DataRow(new byte[] { 0xC1 }, OperationStatus.InvalidData, 0xFFFD, 1)] // [ C1 ] is never a valid byte
        [DataRow(new byte[] { 0x80, 0xE2, 0x88, 0xB4 }, OperationStatus.Done, 0x2234, 3)] // [ E2 88 B4 ] is U+2234 THEREFORE
        [DataRow(new byte[] { 0xF0, 0x9F, 0x98, 0xB2 }, OperationStatus.Done, 0x1F632, 4)] // [ F0 9F 98 B2 ] is U+1F632 ASTONISHED FACE
        [DataRow(new byte[] { 0xE2, 0x88, 0xB4, 0xB2 }, OperationStatus.InvalidData, 0xFFFD, 1)] // [ B2 ] is standalone continuation byte
        [DataRow(new byte[] { 0x80, 0x62, 0x80, 0x80 }, OperationStatus.InvalidData, 0xFFFD, 1)] // [ 80 ] is standalone continuation byte
        [DataRow(new byte[] { 0xF0, 0x9F, 0x98, }, OperationStatus.NeedMoreData, 0xFFFD, 3)] // [ F0 9F 98 ] is valid 3-byte start of 4-byte sequence
        public void DecodeLastFromUtf8(byte[] data, OperationStatus expectedOperationStatus, int expectedRuneValue, int expectedBytesConsumed)
        {
            Assert.AreEqual(expectedOperationStatus, Rune.DecodeLastFromUtf8(data, out Rune actualRune, out int actualBytesConsumed));
            Assert.AreEqual(expectedRuneValue, actualRune.Value);
            Assert.AreEqual(expectedBytesConsumed, actualBytesConsumed);
        }

        [TestMethod]
        [DataRow(0, 0, true)]
        [DataRow(0x10FFFF, 0x10FFFF, true)]
        [DataRow(0xFFFD, 0xFFFD, true)]
        [DataRow(0xFFFD, 0xFFFF, false)]
        [DataRow('a', 'a', true)]
        [DataRow('a', 'A', false)]
        [DataRow('a', 'b', false)]
        public void Equals_OperatorEqual_OperatorNotEqual(int first, int other, bool expected)
        {
            Rune a = new Rune(first);
            Rune b = new Rune(other);

            Assert.AreEqual(expected, Object.Equals(a, b));
            Assert.AreEqual(expected, a.Equals(b));
            Assert.AreEqual(expected, a.Equals((object)b));
            Assert.AreEqual(expected, a == b);
            Assert.AreNotEqual(expected, a != b);
        }

        [TestMethod]
        [DataRow('a', 'a', StringComparison.CurrentCulture, true)]
        [DataRow('a', 'A', StringComparison.CurrentCulture, false)]
        [DataRow(0x1F600, 0x1F600, StringComparison.CurrentCulture, true)]
        [DataRow(0x1F601, 0x1F600, StringComparison.CurrentCulture, false)]
        [DataRow('a', 'a', StringComparison.CurrentCultureIgnoreCase, true)]
        [DataRow('a', 'A', StringComparison.CurrentCultureIgnoreCase, true)]
        [DataRow(0x1F600, 0x1F600, StringComparison.CurrentCultureIgnoreCase, true)]
        [DataRow(0x1F601, 0x1F600, StringComparison.CurrentCultureIgnoreCase, false)]
        [DataRow('a', 'a', StringComparison.InvariantCulture, true)]
        [DataRow('a', 'A', StringComparison.InvariantCulture, false)]
        [DataRow(0x1F600, 0x1F600, StringComparison.InvariantCulture, true)]
        [DataRow(0x1F601, 0x1F600, StringComparison.InvariantCulture, false)]
        [DataRow('a', 'a', StringComparison.InvariantCultureIgnoreCase, true)]
        [DataRow('a', 'A', StringComparison.InvariantCultureIgnoreCase, true)]
        [DataRow(0x1F600, 0x1F600, StringComparison.InvariantCultureIgnoreCase, true)]
        [DataRow(0x1F601, 0x1F600, StringComparison.InvariantCultureIgnoreCase, false)]
        [DataRow('a', 'a', StringComparison.Ordinal, true)]
        [DataRow('a', 'A', StringComparison.Ordinal, false)]
        [DataRow(0x1F600, 0x1F600, StringComparison.Ordinal, true)]
        [DataRow(0x1F601, 0x1F600, StringComparison.Ordinal, false)]
        [DataRow('a', 'a', StringComparison.OrdinalIgnoreCase, true)]
        [DataRow('a', 'A', StringComparison.OrdinalIgnoreCase, true)]
        [DataRow(0x1F600, 0x1F600, StringComparison.OrdinalIgnoreCase, true)]
        [DataRow(0x1F601, 0x1F600, StringComparison.OrdinalIgnoreCase, false)]
        public void Equals_StringComparison(int first, int other, StringComparison comparisonType, bool expected)
        {
            Rune a = new Rune(first);
            Rune b = new Rune(other);

            Assert.AreEqual(expected, a.Equals(b, comparisonType));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow('a')]
        [DataRow('\uFFFD')]
        [DataRow(0x10FFFF)]
        public void GetHashCodeTests(int scalarValue)
        {
            Assert.AreEqual(scalarValue, new Rune(scalarValue).GetHashCode());
        }

        [TestMethod]
        [DataRow("a", 0, (int)'a')]
        [DataRow("ab", 1, (int)'b')]
        [DataRow("x\U0001F46Ey", 3, (int)'y')]
        [DataRow("x\U0001F46Ey", 1, 0x1F46E)] // U+1F46E POLICE OFFICER
        public void GetRuneAt_TryGetRuneAt_Utf16_Success(string inputString, int index, int expectedScalarValue)
        {
            // GetRuneAt
            Assert.AreEqual(expectedScalarValue, Rune.GetRuneAt(inputString, index).Value);

            // TryGetRuneAt
            Assert.IsTrue(Rune.TryGetRuneAt(inputString, index, out Rune rune));
            Assert.AreEqual(expectedScalarValue, rune.Value);
        }

        // Our unit test runner doesn't deal well with malformed literal strings, so
        // we smuggle it as a char[] and turn it into a string within the test itself.
        [TestMethod]
        [DataRow(new char[] { 'x', '\uD83D', '\uDC6E', 'y' }, 2)] // attempt to index into the middle of a UTF-16 surrogate pair
        [DataRow(new char[] { 'x', '\uD800', 'y' }, 1)] // high surrogate not followed by low surrogate
        [DataRow(new char[] { 'x', '\uDFFF', '\uDFFF' }, 1)] // attempt to start at a low surrogate
        [DataRow(new char[] { 'x', '\uD800' }, 1)] // end of string reached before could complete surrogate pair
        public void GetRuneAt_TryGetRuneAt_Utf16_InvalidData(char[] inputCharArray, int index)
        {
            string inputString = new string(inputCharArray);

            // GetRuneAt
            Assert.Throws<ArgumentException>(() => Rune.GetRuneAt(inputString, index));

            // TryGetRuneAt
            Assert.IsFalse(Rune.TryGetRuneAt(inputString, index, out Rune rune));
            Assert.AreEqual(0, rune.Value);
        }

        [TestMethod]
        public void GetRuneAt_TryGetRuneAt_Utf16_BadArgs()
        {
            // null input
            Assert.Throws<ArgumentNullException>(() => Rune.GetRuneAt(null!, 0));

            // negative index specified
            Assert.Throws<ArgumentOutOfRangeException>(() => Rune.GetRuneAt("hello", -1));

            // index goes past end of string
            Assert.Throws<ArgumentOutOfRangeException>(() => Rune.GetRuneAt(string.Empty, 0));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void GetNumericValue(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.NumericValue, Rune.GetNumericValue(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void GetUnicodeCategory(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.UnicodeCategory, Rune.GetUnicodeCategory(new Rune(testData.RawValue)));
        }

        //[TestMethod]
        //public void GetUnicodeCategory_AllInputs()
        //{
        //    // This tests calls Rune.GetUnicodeCategory for every possible input, ensuring that
        //    // the runtime agrees with the data in the core Unicode files.

        //    foreach (Rune rune in AllRunes())
        //    {
        //        if (UnicodeData.GetUnicodeCategory(rune.Value) != Rune.GetUnicodeCategory(rune))
        //        {
        //            // We'll build up the exception message ourselves so the dev knows what code point failed.
        //            throw EqualException.ForMismatchedValues(
        //                expected: UnicodeData.GetUnicodeCategory(rune.Value),
        //                actual: Rune.GetUnicodeCategory(rune),
        //                banner: FormattableString.Invariant($@"Rune.GetUnicodeCategory(U+{rune.Value:X4}) returned wrong value."));
        //        }
        //    }
        //}

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsControl(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsControl, Rune.IsControl(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsDigit(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsDigit, Rune.IsDigit(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsLetter(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsLetter, Rune.IsLetter(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsLetterOrDigit(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsLetterOrDigit, Rune.IsLetterOrDigit(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsLower(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsLower, Rune.IsLower(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsNumber(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsNumber, Rune.IsNumber(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsPunctuation(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsPunctuation, Rune.IsPunctuation(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsSeparator(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsSeparator, Rune.IsSeparator(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsSymbol(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsSymbol, Rune.IsSymbol(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsUpper(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsUpper, Rune.IsUpper(new Rune(testData.RawValue)));
        }

        [TestMethod]
        [DynamicData(nameof(IsValidTestData))]
        public void IsValid(int scalarValue, bool expectedIsValid)
        {
            Assert.AreEqual(expectedIsValid, Rune.IsValid(scalarValue));
            Assert.AreEqual(expectedIsValid, Rune.IsValid((uint)scalarValue));
        }

        [TestMethod]
        [DynamicData(nameof(UnicodeInfoTestData_Latin1AndSelectOthers))]
        public void IsWhiteSpace(UnicodeInfoTestData testData)
        {
            Assert.AreEqual(testData.IsWhiteSpace, Rune.IsWhiteSpace(new Rune(testData.RawValue)));
        }

        //[TestMethod]
        //public void IsWhiteSpace_AllInputs()
        //{
        //    // This tests calls Rune.IsWhiteSpace for every possible input, ensuring that
        //    // the runtime agrees with the data in the core Unicode files.

        //    foreach (Rune rune in AllRunes())
        //    {
        //        Assert.AreEqual(UnicodeData.IsWhiteSpace(rune.Value), Rune.IsWhiteSpace(rune));
        //    }
        //}

        [TestMethod]
        [DataRow((uint)0, (uint)0)]
        [DataRow((uint)0x80, (uint)0x80)]
        [DataRow((uint)0x80, (uint)0x100)]
        [DataRow((uint)0x100, (uint)0x80)]
        public void Operators_And_CompareTo(uint scalarValueLeft, uint scalarValueRight)
        {
            Rune left = new Rune(scalarValueLeft);
            Rune right = new Rune(scalarValueRight);

            Assert.AreEqual(scalarValueLeft == scalarValueRight, left == right);
            Assert.AreEqual(scalarValueLeft != scalarValueRight, left != right);
            Assert.AreEqual(scalarValueLeft < scalarValueRight, left < right);
            Assert.AreEqual(scalarValueLeft <= scalarValueRight, left <= right);
            Assert.AreEqual(scalarValueLeft > scalarValueRight, left > right);
            Assert.AreEqual(scalarValueLeft >= scalarValueRight, left >= right);
            Assert.AreEqual(Math.Sign(scalarValueLeft.CompareTo(scalarValueRight)), Math.Sign(left.CompareTo(right)));
            Assert.AreEqual(Math.Sign(((IComparable)scalarValueLeft).CompareTo(scalarValueRight)), Math.Sign(((IComparable)left).CompareTo(right)));
        }

        [TestMethod]
        [DataRow((uint)0)]
        [DataRow((uint)0x10FFFF)]
        public void NonGenericCompareTo_NonNullAlwaysGreaterThanNull(uint scalarValue)
        {
            Assert.AreEqual(1, Math.Sign(((IComparable)new Rune(scalarValue)).CompareTo(null)));
        }

        [TestMethod]
        public void NonGenericCompareTo_GivenNonRuneArgument_ThrowsArgumentException()
        {
            IComparable rune = new Rune(0);

            Assert.Throws<ArgumentException>(() => rune.CompareTo(0 /* int32 */));
        }

        [TestMethod]
        public void ReplacementChar()
        {
            Assert.AreEqual(0xFFFD, Rune.ReplacementChar.Value);
        }

        [TestMethod]
        [DynamicData(nameof(GeneralTestData_BmpCodePoints_NoSurrogates))]
        public void TryCreate_Char_Valid(GeneralTestData testData)
        {
            Assert.IsTrue(Rune.TryCreate((char)testData.ScalarValue, out Rune result));
            Assert.AreEqual(testData.ScalarValue, result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(BmpCodePoints_SurrogatesOnly))]
        public void TryCreate_Char_Invalid(int scalarValue)
        {
            Assert.IsFalse(Rune.TryCreate((char)scalarValue, out Rune result));
            Assert.AreEqual(0, result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(SurrogatePairTestData_InvalidOnly))]
        public void TryCreate_SurrogateChars_Invalid(char highSurrogate, char lowSurrogate)
        {
            Assert.IsFalse(Rune.TryCreate(highSurrogate, lowSurrogate, out Rune result));
            Assert.AreEqual(0, result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(SurrogatePairTestData_ValidOnly))]
        public void TryCreate_SurrogateChars_Valid(char highSurrogate, char lowSurrogate, int expectedValue)
        {
            Assert.IsTrue(Rune.TryCreate(highSurrogate, lowSurrogate, out Rune result));
            Assert.AreEqual(expectedValue, result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(GeneralTestData_BmpCodePoints_NoSurrogates))]
        [DynamicData(nameof(GeneralTestData_SupplementaryCodePoints_ValidOnly))]
        public void TryCreate_Int32_Valid(GeneralTestData testData)
        {
            Assert.IsTrue(Rune.TryCreate((int)testData.ScalarValue, out Rune result));
            Assert.AreEqual(testData.ScalarValue, result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(BmpCodePoints_SurrogatesOnly))]
        [DynamicData(nameof(SupplementaryCodePoints_InvalidOnly))]
        public void TryCreate_Int32_Invalid(int scalarValue)
        {
            Assert.IsFalse(Rune.TryCreate((int)scalarValue, out Rune result));
            Assert.AreEqual(0, result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(GeneralTestData_BmpCodePoints_NoSurrogates))]
        [DynamicData(nameof(GeneralTestData_SupplementaryCodePoints_ValidOnly))]
        public void TryCreate_UInt32_Valid(GeneralTestData testData)
        {
            Assert.IsTrue(Rune.TryCreate((uint)testData.ScalarValue, out Rune result));
            Assert.AreEqual(testData.ScalarValue, result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(BmpCodePoints_SurrogatesOnly))]
        [DynamicData(nameof(SupplementaryCodePoints_InvalidOnly))]
        public void TryCreate_UInt32_Invalid(int scalarValue)
        {
            Assert.IsFalse(Rune.TryCreate((uint)scalarValue, out Rune result));
            Assert.AreEqual(0, result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(GeneralTestData_BmpCodePoints_NoSurrogates))]
        [DynamicData(nameof(GeneralTestData_SupplementaryCodePoints_ValidOnly))]
        public void ToStringTests(GeneralTestData testData)
        {
            Assert.AreEqual(new string(testData.Utf16Sequence), new Rune(testData.ScalarValue).ToString());
        }

        [TestMethod]
        [DynamicData(nameof(GeneralTestData_BmpCodePoints_NoSurrogates))]
        [DynamicData(nameof(GeneralTestData_SupplementaryCodePoints_ValidOnly))]
        public void TryEncodeToUtf16(GeneralTestData testData)
        {
            Rune rune = new Rune(testData.ScalarValue);
            Assert.AreEqual(testData.Utf16Sequence.Length, rune.Utf16SequenceLength);

            // First, try with a buffer that's too short

            char[] utf16Buffer = new char[rune.Utf16SequenceLength - 1];
            bool success = rune.TryEncodeToUtf16(utf16Buffer, out int charsWritten);
            Assert.IsFalse(success);
            Assert.AreEqual(0, charsWritten);

            Assert.Throws<ArgumentException>(() => rune.EncodeToUtf16(new char[rune.Utf16SequenceLength - 1]));

            // Then, try with a buffer that's appropriately sized

            utf16Buffer = new char[rune.Utf16SequenceLength];
            success = rune.TryEncodeToUtf16(utf16Buffer, out charsWritten);
            Assert.IsTrue(success);
            Assert.AreEqual(testData.Utf16Sequence.Length, charsWritten);
            Assert.IsTrue(utf16Buffer.SequenceEqual(testData.Utf16Sequence));

            utf16Buffer = new char[rune.Utf16SequenceLength];
            Assert.AreEqual(testData.Utf16Sequence.Length, rune.EncodeToUtf16(utf16Buffer));
            Assert.IsTrue(utf16Buffer.SequenceEqual(testData.Utf16Sequence));

            // Finally, try with a buffer that's too long (should succeed)

            utf16Buffer = new char[rune.Utf16SequenceLength + 1];
            success = rune.TryEncodeToUtf16(utf16Buffer, out charsWritten);
            Assert.IsTrue(success);
            Assert.AreEqual(testData.Utf16Sequence.Length, charsWritten);
            Assert.IsTrue(utf16Buffer.Take(testData.Utf16Sequence.Length).SequenceEqual(testData.Utf16Sequence));

            utf16Buffer = new char[rune.Utf16SequenceLength + 1];
            Assert.AreEqual(testData.Utf16Sequence.Length, rune.EncodeToUtf16(utf16Buffer));
            Assert.IsTrue(utf16Buffer.Take(testData.Utf16Sequence.Length).SequenceEqual(testData.Utf16Sequence));
        }

        [TestMethod]
        [DynamicData(nameof(GeneralTestData_BmpCodePoints_NoSurrogates))]
        [DynamicData(nameof(GeneralTestData_SupplementaryCodePoints_ValidOnly))]
        public void TryEncodeToUtf8(GeneralTestData testData)
        {
            Rune rune = new Rune(testData.ScalarValue);
            Assert.AreEqual(testData.Utf8Sequence.Length, actual: rune.Utf8SequenceLength);

            // First, try with a buffer that's too short

            byte[] utf8Buffer = new byte[rune.Utf8SequenceLength - 1];
            bool success = rune.TryEncodeToUtf8(utf8Buffer, out int bytesWritten);
            Assert.IsFalse(success);
            Assert.AreEqual(0, bytesWritten);

            Assert.Throws<ArgumentException>(() => rune.EncodeToUtf8(new byte[rune.Utf8SequenceLength - 1]));

            // Then, try with a buffer that's appropriately sized

            utf8Buffer = new byte[rune.Utf8SequenceLength];
            success = rune.TryEncodeToUtf8(utf8Buffer, out bytesWritten);
            Assert.IsTrue(success);
            Assert.AreEqual(testData.Utf8Sequence.Length, bytesWritten);
            Assert.IsTrue(utf8Buffer.SequenceEqual(testData.Utf8Sequence));

            utf8Buffer = new byte[rune.Utf8SequenceLength];
            Assert.AreEqual(testData.Utf8Sequence.Length, rune.EncodeToUtf8(utf8Buffer));
            Assert.IsTrue(utf8Buffer.SequenceEqual(testData.Utf8Sequence));

            // Finally, try with a buffer that's too long (should succeed)

            utf8Buffer = new byte[rune.Utf8SequenceLength + 1];
            success = rune.TryEncodeToUtf8(utf8Buffer, out bytesWritten);
            Assert.IsTrue(success);
            Assert.AreEqual(testData.Utf8Sequence.Length, bytesWritten);
            Assert.IsTrue(utf8Buffer.Take(testData.Utf8Sequence.Length).SequenceEqual(testData.Utf8Sequence));

            utf8Buffer = new byte[rune.Utf8SequenceLength + 1];
            Assert.AreEqual(testData.Utf8Sequence.Length, rune.EncodeToUtf8(utf8Buffer));
            Assert.IsTrue(utf8Buffer.Take(testData.Utf8Sequence.Length).SequenceEqual(testData.Utf8Sequence));
        }

        //
        // RunePosition tests
        //

        //private static void RunePosition_TestProps(Rune rune, int startIndex, int length, bool wasReplaced, RunePosition runePosition)
        //{
        //    Assert.AreEqual(rune, runePosition.Rune);
        //    Assert.AreEqual(startIndex, runePosition.StartIndex);
        //    Assert.AreEqual(length, runePosition.Length);
        //    Assert.AreEqual(wasReplaced, runePosition.WasReplaced);

        //    Assert.AreEqual(new RunePosition(rune, startIndex, length, wasReplaced), runePosition);
        //}

        //private static void RunePosition_TestEquals(RunePosition expected, RunePosition runePosition)
        //{
        //    if (expected.Rune == runePosition.Rune && expected.StartIndex == runePosition.StartIndex &&
        //        expected.Length == runePosition.Length && expected.WasReplaced == runePosition.WasReplaced)
        //    {
        //        Assert.AreEqual(expected, runePosition);
        //        Assert.AreEqual(runePosition, expected);

        //        Assert.IsTrue(expected.Equals(runePosition));
        //        Assert.IsTrue(runePosition.Equals(expected));

        //        Assert.IsTrue(((object)expected).Equals(runePosition));
        //        Assert.IsTrue(((object)runePosition).Equals(expected));

        //        Assert.IsTrue(expected == runePosition);
        //        Assert.IsTrue(runePosition == expected);

        //        Assert.IsFalse(expected != runePosition);
        //        Assert.IsFalse(runePosition != expected);

        //        Assert.AreEqual(expected.GetHashCode(), runePosition.GetHashCode());
        //    }
        //    else
        //    {
        //        Assert.AreNotEqual(expected, runePosition);
        //        Assert.AreNotEqual(runePosition, expected);

        //        Assert.IsFalse(expected.Equals(runePosition));
        //        Assert.IsFalse(runePosition.Equals(expected));

        //        Assert.IsFalse(((object)expected).Equals(runePosition));
        //        Assert.IsFalse(((object)runePosition).Equals(expected));

        //        Assert.IsFalse(expected == runePosition);
        //        Assert.IsFalse(runePosition == expected);

        //        Assert.IsTrue(expected != runePosition);
        //        Assert.IsTrue(runePosition != expected);
        //    }
        //}

        //private static void RunePosition_TestDeconstruct(RunePosition runePosition)
        //{
        //    {
        //        (Rune rune, int startIndex) = runePosition;
        //        Assert.AreEqual(runePosition.Rune, rune);
        //        Assert.AreEqual(runePosition.StartIndex, startIndex);
        //    }
        //    {
        //        (Rune rune, int startIndex, int length) = runePosition;
        //        Assert.AreEqual(runePosition.Rune, rune);
        //        Assert.AreEqual(runePosition.StartIndex, startIndex);
        //        Assert.AreEqual(runePosition.Length, length);
        //    }
        //}

        //[TestMethod]
        //public void RunePosition_DefaultTest()
        //{
        //    RunePosition runePosition = default;
        //    RunePosition_TestProps(default, 0, 0, false, runePosition);
        //    RunePosition_TestEquals(default, runePosition);
        //    RunePosition_TestDeconstruct(runePosition);

        //    runePosition = new RunePosition();
        //    RunePosition_TestProps(default, 0, 0, false, runePosition);
        //    RunePosition_TestEquals(default, runePosition);
        //    RunePosition_TestDeconstruct(runePosition);
        //}

        //[TestMethod]
        //public void EnumerateRunePositions_Empty()
        //{
        //    {
        //        RunePosition.Utf16Enumerator enumerator = RunePosition.EnumerateUtf16([]).GetEnumerator();
        //        Assert.IsFalse(enumerator.MoveNext());
        //    }
        //    {
        //        RunePosition.Utf8Enumerator enumerator = RunePosition.EnumerateUtf8([]).GetEnumerator();
        //        Assert.IsFalse(enumerator.MoveNext());
        //    }
        //}

        //[TestMethod]
        //[DataRow(new char[0])] // empty
        //[DataRow(new char[] { 'x', 'y', 'z' })]
        //[DataRow(new char[] { 'x', '\uD86D', '\uDF54', 'y' })] // valid surrogate pair
        //[DataRow(new char[] { 'x', '\uD86D', 'y' })] // standalone high surrogate
        //[DataRow(new char[] { 'x', '\uDF54', 'y' })] // standalone low surrogate
        //[DataRow(new char[] { 'x', '\uD86D' })] // standalone high surrogate at end of string
        //[DataRow(new char[] { 'x', '\uDF54' })] // standalone low surrogate at end of string
        //[DataRow(new char[] { 'x', '\uD86D', '\uD86D', 'y' })] // two high surrogates should be two replacement chars
        //[DataRow(new char[] { 'x', '\uFFFD', 'y' })] // literal U+FFFD
        //public void EnumerateRunePositions_Battery16(char[] chars)
        //{
        //    // Test data is smuggled as char[] instead of straight-up string since the test framework
        //    // doesn't like invalid UTF-16 literals.

        //    RunePosition.Utf16Enumerator enumerator = RunePosition.EnumerateUtf16(chars).GetEnumerator();

        //    int expectedIndex = 0;
        //    while (enumerator.MoveNext())
        //    {
        //        bool wasReplaced = Rune.DecodeFromUtf16(chars.AsSpan(expectedIndex), out Rune expectedRune, out int charsConsumed) != OperationStatus.Done;
        //        RunePosition runePosition = enumerator.Current;

        //        RunePosition_TestProps(expectedRune, expectedIndex, charsConsumed, wasReplaced, runePosition);

        //        expectedIndex += charsConsumed;
        //    }
        //    Assert.AreEqual(chars.Length, expectedIndex);
        //}

        //[TestMethod]
        //[DataRow(new byte[0])] // empty
        //[DataRow(new byte[] { 0x30, 0x40, 0x50 })]
        //[DataRow(new byte[] { 0x31, 0x80, 0x41 })] // standalone continuation byte
        //[DataRow(new byte[] { 0x32, 0xC1, 0x42 })] // C1 is never a valid UTF-8 byte
        //[DataRow(new byte[] { 0x33, 0xF5, 0x43 })] // F5 is never a valid UTF-8 byte
        //[DataRow(new byte[] { 0x34, 0xC2, 0x44 })] // C2 is a valid byte; expecting it to be followed by a continuation byte
        //[DataRow(new byte[] { 0x35, 0xED, 0x45 })] // ED is a valid byte; expecting it to be followed by a continuation byte
        //[DataRow(new byte[] { 0x36, 0xF4, 0x46 })] // F4 is a valid byte; expecting it to be followed by a continuation byte
        //[DataRow(new byte[] { 0x37, 0xC2, 0xC2, 0x47 })] // C2 not followed by continuation byte
        //[DataRow(new byte[] { 0x38, 0xC3, 0x90, 0x48 })] // [ C3 90 ] is U+00D0 LATIN CAPITAL LETTER ETH
        //[DataRow(new byte[] { 0x39, 0xC1, 0xBF, 0x49 })] // [ C1 BF ] is overlong 2-byte sequence, all overlong sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0x40, 0xE0, 0x9F, 0x50 })] // [ E0 9F ] is overlong 3-byte sequence, all overlong sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0x41, 0xE0, 0xA0, 0x51 })] // [ E0 A0 ] is valid 2-byte start of 3-byte sequence
        //[DataRow(new byte[] { 0x42, 0xED, 0x9F, 0x52 })] // [ ED 9F ] is valid 2-byte start of 3-byte sequence
        //[DataRow(new byte[] { 0x43, 0xED, 0xBF, 0x53 })] // [ ED BF ] would place us in UTF-16 surrogate range, all surrogate sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0x44, 0xEE, 0x80, 0x54 })] // [ EE 80 ] is valid 2-byte start of 3-byte sequence
        //[DataRow(new byte[] { 0x45, 0xF0, 0x8F, 0x55 })] // [ F0 8F ] is overlong 4-byte sequence, all overlong sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0x46, 0xF0, 0x90, 0x56 })] // [ F0 90 ] is valid 2-byte start of 4-byte sequence
        //[DataRow(new byte[] { 0x47, 0xF4, 0x90, 0x57 })] // [ F4 90 ] would place us beyond U+10FFFF, all such sequences have maximal invalid subsequence length 1
        //[DataRow(new byte[] { 0x48, 0xE2, 0x88, 0xB4, 0x58 })] // [ E2 88 B4 ] is U+2234 THEREFORE
        //[DataRow(new byte[] { 0x49, 0xE2, 0x88, 0xC0, 0x59 })] // [ E2 88 ] followed by non-continuation byte, maximal invalid subsequence length 2
        //[DataRow(new byte[] { 0x50, 0xF0, 0x9F, 0x98, 0x60 })] // [ F0 9F 98 ] is valid 3-byte start of 4-byte sequence
        //[DataRow(new byte[] { 0x51, 0xF0, 0x9F, 0x98, 0x20, 0x61 })] // [ F0 9F 98 ] followed by non-continuation byte, maximal invalid subsequence length 3
        //[DataRow(new byte[] { 0x52, 0xF0, 0x9F, 0x98, 0xB2, 0x62 })] // [ F0 9F 98 B2 ] is U+1F632 ASTONISHED FACE
        //public void EnumerateRunePositions_Battery8(byte[] bytes)
        //{
        //    RunePosition.Utf8Enumerator enumerator = RunePosition.EnumerateUtf8(bytes).GetEnumerator();

        //    int expectedIndex = 0;
        //    while (enumerator.MoveNext())
        //    {
        //        bool wasReplaced = Rune.DecodeFromUtf8(bytes.AsSpan(expectedIndex), out Rune expectedRune, out int charsConsumed) != OperationStatus.Done;
        //        RunePosition runePosition = enumerator.Current;

        //        RunePosition_TestProps(expectedRune, expectedIndex, charsConsumed, wasReplaced, runePosition);

        //        expectedIndex += charsConsumed;
        //    }
        //    Assert.AreEqual(bytes.Length, expectedIndex);
        //}

        //[TestMethod]
        //public void EnumerateRunePositions_DoesNotReadPastEndOfSpan()
        //{
        //    // As an optimization, reading scalars from a string *may* read past the end of the string
        //    // to the terminating null. This optimization is invalid for arbitrary spans, so this test
        //    // ensures that we're not performing this optimization here.

        //    {
        //        ReadOnlySpan<char> span = "xy\U0002B754z".AsSpan(1, 2); // well-formed string, but span splits surrogate pair

        //        List<int> enumeratedValues = new List<int>();
        //        foreach (RunePosition runePosition in RunePosition.EnumerateUtf16(span))
        //        {
        //            enumeratedValues.Add(runePosition.Rune.Value);
        //        }
        //        Assert.AreEqual(new int[] { 'y', '\uFFFD' }, enumeratedValues.ToArray());
        //    }

        //    {
        //        ReadOnlySpan<byte> span = "xy\U0002B754z"u8.Slice(1, 2); // well-formed string, but span splits surrogate pair

        //        List<int> enumeratedValues = new List<int>();
        //        foreach (RunePosition runePosition in RunePosition.EnumerateUtf8(span))
        //        {
        //            enumeratedValues.Add(runePosition.Rune.Value);
        //        }
        //        Assert.AreEqual(new int[] { 'y', '\uFFFD' }, enumeratedValues.ToArray());
        //    }
        //}

        //[TestMethod]
        //public void EnumerateRunePositions_ResetEnumeration()
        //{
        //    string text = "AB\U0002B754CD";
        //    byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(text);

        //    {
        //        RunePosition.Utf16Enumerator enumerator = RunePosition.EnumerateUtf16(text).GetEnumerator();
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('A', enumerator.Current.Rune.Value);

        //        enumerator.Reset();

        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('A', enumerator.Current.Rune.Value);
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('B', enumerator.Current.Rune.Value);
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual(0x2B754, enumerator.Current.Rune.Value);
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('C', enumerator.Current.Rune.Value);
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('D', enumerator.Current.Rune.Value);

        //        Assert.IsFalse(enumerator.MoveNext());
        //        enumerator.Reset();
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('A', enumerator.Current.Rune.Value);
        //    }

        //    {
        //        RunePosition.Utf8Enumerator enumerator = RunePosition.EnumerateUtf8(utf8Bytes).GetEnumerator();
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('A', enumerator.Current.Rune.Value);

        //        enumerator.Reset();

        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('A', enumerator.Current.Rune.Value);
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('B', enumerator.Current.Rune.Value);
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual(0x2B754, enumerator.Current.Rune.Value);
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('C', enumerator.Current.Rune.Value);
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('D', enumerator.Current.Rune.Value);

        //        Assert.IsFalse(enumerator.MoveNext());
        //        enumerator.Reset();
        //        Assert.IsTrue(enumerator.MoveNext());
        //        Assert.AreEqual('A', enumerator.Current.Rune.Value);
        //    }
        //}
    }
}
