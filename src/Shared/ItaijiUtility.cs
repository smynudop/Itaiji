using System.Linq;
using Itaiji.Extensions;
#if NETSTANDARD2_0 || NETFRAMEWORK
using Itaiji.Text;
#else
using System.Buffers;
#endif

namespace Itaiji;

/// <summary>
/// 異体字を考慮した文字列操作のユーティリティ関数を提供します。
/// </summary>
public static partial class ItaijiUtility
{

    /// <summary>
    /// 異体字セレクタを考慮したときの文字列の長さを取得します。
    /// このメソッドは厳密な書記素数を返しません。
    /// 正確な書記素数を取得したい場合は <see cref="System.Globalization.StringInfo"/> を使用してください。
    /// </summary>
    /// <param name="str">対象の文字列</param>
    /// <returns>異体字セレクタを考慮した文字列の長さ（漢字列としての長さ）を返します。</returns>
    public static int LengthAsKanji(string str)
    {
        var enumerator = new KanjiEnumerator(str);
        int count = 0;
        while (enumerator.MoveNext())
        {
            count++;
        }
        return count;
    }

    private static Func<KanjiChar, KanjiChar, bool> GetEqualsFunc(IvsComparison comparison)
    {
        return comparison switch
        {
            IvsComparison.ExactMatch => (KanjiChar a, KanjiChar b) => a == b,
            IvsComparison.IgnoreIvs => (KanjiChar a, KanjiChar b) => a.BaseRune == b.BaseRune,
            _ => throw new System.ArgumentOutOfRangeException(nameof(comparison), comparison, null)
        };
    }

    private static int[] CreatePrefixTable(KanjiChar[] keywordKanjis)
    {
        var prefixTable = new int[keywordKanjis.Length];
        var length = 0;
        prefixTable[0] = 0;
        for (int i = 1; i < keywordKanjis.Length;)
        {
            if (keywordKanjis[i].BaseRune == keywordKanjis[length].BaseRune)
            {
                length++;
                prefixTable[i] = length;
                i++;
            }
            else if (length != 0)
            {
                length = prefixTable[length - 1];
            }
            else
            {
                prefixTable[i] = 0;
                i++;
            }
        }
        return prefixTable;
    }

    private static int[] BuildUtf16Offsets(KanjiChar[] kanjis)
    {
        var offsets = new int[kanjis.Length + 1];
        for (int i = 0; i < kanjis.Length; i++)
        {
            offsets[i + 1] = offsets[i] + kanjis[i].Utf16SequenceLength;
        }
        return offsets;
    }

    private static bool TryFindIndexCore(KanjiChar[] sourceKanjis, KanjiChar[] keywordKanjis, Func<KanjiChar, KanjiChar, bool> equalsFunc, bool findLast, out int index, out int length)
    {
        var prefixTable = CreatePrefixTable(keywordKanjis);
        var utf16Offsets = BuildUtf16Offsets(sourceKanjis);

        var lastIndex = -1;
        var lastLength = 0;
        int i = 0;
        int j = 0;

        while (i < sourceKanjis.Length)
        {
            if (equalsFunc(sourceKanjis[i], keywordKanjis[j]))
            {
                i++;
                j++;
                if (j == keywordKanjis.Length)
                {
                    var matchStart = i - j;
                    var matchIndex = utf16Offsets[matchStart];
                    var matchLength = utf16Offsets[matchStart + j] - matchIndex;

                    if (!findLast)
                    {
                        index = matchIndex;
                        length = matchLength;
                        return true;
                    }

                    lastIndex = matchIndex;
                    lastLength = matchLength;
                    j = prefixTable[j - 1];
                }
            }
            else if (j != 0)
            {
                j = prefixTable[j - 1];
            }
            else
            {
                i++;
            }
        }

        if (findLast && lastIndex >= 0)
        {
            index = lastIndex;
            length = lastLength;
            return true;
        }

        index = -1;
        length = 0;
        return false;
    }

    /// <summary>
    /// 異体字を考慮して文字列が等しいか調べます。
    /// </summary>
    /// <param name="a">比較対象の文字列（左辺）</param>
    /// <param name="b">比較対象の文字列（右辺）</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <returns>等しい場合はtrue、それ以外はfalseを返します。</returns>
    public static bool Equals(string a, string b, IvsComparison comparison)
    { 

        Func<KanjiChar,KanjiChar, bool> equalsFunc = GetEqualsFunc(comparison);

        var enumeratorA = a.EnumerateKanji();
        var enumeratorB = b.EnumerateKanji();

        while (true)
        {
            var hasA = enumeratorA.MoveNext();
            var hasB = enumeratorB.MoveNext();
            if (!hasA && !hasB)
            {
                return true;
            }
            if (hasA != hasB)
            {
                return false;
            }
            var kanjiA = enumeratorA.Current;
            var kanjiB = enumeratorB.Current;
            if (!equalsFunc(kanjiA, kanjiB) )
                
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 異体字の違いを無視して文字列が等しいか調べます。
    /// </summary>
    /// <param name="a">比較対象の文字列（左辺）</param>
    /// <param name="b">比較対象の文字列（右辺）</param>
    /// <returns>異体字を無視して等しい場合はtrue、それ以外はfalseを返します。</returns>
    public static bool EqualsIgnoreIvs(string a, string b)
    {
        return Equals(a, b, IvsComparison.IgnoreIvs);
    }

    /// <summary>
    /// 異体字を厳密に考慮して文字列が等しいか調べます。
    /// </summary>
    /// <remarks>このメソッドはstring.Equalsと同じ動作をします。</remarks>
    /// <param name="a">比較対象の文字列（左辺）</param>
    /// <param name="b">比較対象の文字列（右辺）</param>
    /// <returns>異体字も区別して等しい場合はtrue、それ以外はfalseを返します。</returns>
    public static bool EqualsExactMatch(string a, string b)
    {
        return Equals(a, b, IvsComparison.ExactMatch);
    }

    /// <summary>
    /// 異体字を考慮して文字列の中にキーワードが含まれているか調べます。
    /// </summary>
    /// <param name="str">検索対象の文字列</param>
    /// <param name="keyword">検索する部分文字列</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <returns>部分文字列が存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool Contains(string str, string keyword, IvsComparison comparison)
    {
        return TryFindIndex(str, keyword, comparison, out _, out _);
    }

#if NET47_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// 異体字を考慮して文字列の中にキーワードが含まれているか調べ、その開始indexとchar換算でのlengthを返します。
    /// </summary>
    /// <param name="str">検索対象の文字列</param>
    /// <param name="keyword">検索する部分文字列</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <returns>見つかった場合は開始indexとchar単位のlengthを返します。見つからなければ(-1,0)を返します。</returns>
    public static (int index, int length) FindIndex(string str, string keyword, IvsComparison comparison)
    {
        TryFindIndex(str, keyword, comparison, out var index, out var length);
        return (index, length);
    }

    /// <summary>
    /// 異体字を考慮して文字列の中にキーワードが含まれているか調べ、最後に見つかった開始indexとchar換算でのlengthを返します。
    /// </summary>
    /// <param name="str">検索対象の文字列</param>
    /// <param name="keyword">検索する部分文字列</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <returns>見つかった場合は開始indexとchar単位のlengthを返します。見つからなければ(-1,0)を返します。</returns>
    public static (int index, int length) FindLastIndex(string str, string keyword, IvsComparison comparison)
    {
        TryFindLastIndex(str, keyword, comparison, out var index, out var length);
        return (index, length);
    }
#endif

    /// <summary>
    /// 異体字を考慮して文字列の中にキーワードが含まれているか調べます。
    /// </summary>
    /// <param name="str">検索対象の文字列</param>
    /// <param name="keyword">検索する部分文字列</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <param name="index">見つかった場合に開始indexが格納されます（見つからない場合は-1）</param>
    /// <param name="length">見つかった場合にchar単位のlengthが格納されます（見つからない場合は0）</param>
    /// <returns>部分文字列が存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool TryFindIndex(string str, string keyword, IvsComparison comparison, out int index, out int length)
    {
        Func<KanjiChar, KanjiChar, bool> equalsFunc = GetEqualsFunc(comparison);
        var keywordKanjis = keyword.EnumerateKanji().ToArray();
        var sourceKanjis = str.EnumerateKanji().ToArray();
        return TryFindIndexCore(sourceKanjis, keywordKanjis, equalsFunc, false, out index, out length);
    }

    /// <summary>
    /// 異体字を考慮して文字列の中にキーワードが含まれているか調べます。
    /// </summary>
    /// <param name="str">検索対象の文字列</param>
    /// <param name="keyword">検索する部分文字列</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <param name="index">見つかった場合に開始indexが格納されます（見つからない場合は-1）</param>
    /// <param name="length">見つかった場合にchar単位のlengthが格納されます（見つからない場合は0）</param>
    /// <returns>部分文字列が存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool TryFindLastIndex(string str, string keyword, IvsComparison comparison, out int index, out int length)
    {
        Func<KanjiChar, KanjiChar, bool> equalsFunc = GetEqualsFunc(comparison);
        var keywordKanjis = keyword.EnumerateKanji().ToArray();
        var sourceKanjis = str.EnumerateKanji().ToArray();
        return TryFindIndexCore(sourceKanjis, keywordKanjis, equalsFunc, true, out index, out length);
    }

    /// <summary>
    /// 異体字を考慮して、文字列中の指定した部分文字列を置換します。
    /// </summary>
    /// <param name="str">検索対象の文字列</param>
    /// <param name="keyword">置換対象の部分文字列</param>
    /// <param name="replacement">置換後の文字列</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <returns>置換後の文字列を返します。</returns>
    public static string Replace(string str, string keyword, string replacement, IvsComparison comparison)
    {
        Func<KanjiChar, KanjiChar, bool> equalsFunc = GetEqualsFunc(comparison);
        var keywordKanjis = keyword.EnumerateKanji().ToArray();
        var sourceKanjis = str.EnumerateKanji().ToArray();
        var prefixTable = CreatePrefixTable(keywordKanjis);
        var utf16Offsets = BuildUtf16Offsets(sourceKanjis);

        StringBuilder? sb = null;
        int lastCopyIndex = 0;
        int i = 0;
        int j = 0;

        while (i < sourceKanjis.Length)
        {
            if (equalsFunc(sourceKanjis[i], keywordKanjis[j]))
            {
                i++;
                j++;
                if (j == keywordKanjis.Length)
                {
                    var matchStart = i - j;
                    var matchIndex = utf16Offsets[matchStart];
                    var matchLength = utf16Offsets[matchStart + j] - matchIndex;

                    sb ??= new StringBuilder(str.Length);
                    sb.Append(str, lastCopyIndex, matchIndex - lastCopyIndex);
                    sb.Append(replacement);
                    lastCopyIndex = matchIndex + matchLength;
                    j = 0;
                }
            }
            else if (j != 0)
            {
                j = prefixTable[j - 1];
            }
            else
            {
                i++;
            }
        }

        if (sb is null)
        {
            return str;
        }

        sb.Append(str, lastCopyIndex, str.Length - lastCopyIndex);
        return sb.ToString();
    }


    /// <summary>
    /// 文字列からIvs/Svsをすべてを除去します。
    /// </summary>
    /// <param name="str">対象の文字列</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveVariationSelector(string str)
    {
        using var sb = new RuneStringBuilder(str.Length);
        foreach (var rune in str.EnumerateRunes())
        {
            if (rune.IsVariationSelector())
            {
                continue;
            }
            sb.Add(rune);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 文字列からIvsをすべて除去します。
    /// </summary>
    /// <param name="str">対象の文字列</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveIvs(string str)
    {
        return RemoveIvs(str, RemoveIvsOption.RemoveAll);
    }

    /// <summary>
    /// 文字列からIvsを除去します。
    /// </summary>
    /// <param name="str">対象の文字列</param>
    /// <param name="option">除去方法</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveIvs(string str, RemoveIvsOption option)
    {
        using var sb = new RuneStringBuilder(str.Length);
        foreach (var kanji in str.EnumerateKanji())
        {
            if (kanji.BaseRune.IsIvs())
            {
                continue;
            }
            sb.Add(kanji.BaseRune);
            if (kanji.IsSvs)
            {
                sb.Add(kanji.NonNullVariationSelector);
            }

            if (option == RemoveIvsOption.RemoveAll)
            {
                continue;
            }
            if(Library.IvsToSvsDic.TryGetValue(kanji, out var svs))
            {
                sb.Add(svs);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 特定のコレクションに対して無効な異体字を含むかどうかを判定します。
    /// </summary>
    /// <param name="str">調査する文字列</param>
    /// <param name="targetType">対象となるコレクションの種類</param>
    /// <returns>指定したコレクションで無効な異体字を含む場合はtrueを返します。</returns>
    public static bool HasInvalidVariationSelector(string str, IvsCollectionType targetType)
    {
        if(targetType == IvsCollectionType.None)
        {
            throw new System.ArgumentException("targetType must not be None.", nameof(targetType));
        }

        foreach (var kanji in str.EnumerateKanji())
        {
            var ivsType = kanji.GetVsCollectionType();
            if (ivsType != IvsCollectionType.None && !ivsType.HasBitFlag(targetType))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Adobe-Japan1として無効な異体字を含むかどうかを判定します。
    /// </summary>
    /// <param name="str">調査する文字列</param>
    /// <returns>Adobe-Japan1として無効な異体字を含む場合はtrueを返します。</returns>
    public static bool HasInvalidVariationSelectorAsAdobeJapan1(string str) => HasInvalidVariationSelector(str, IvsCollectionType.AdobeJapan);

    /// <summary>
    /// Hanyo-Denshiとして無効な異体字を含むかどうかを判定します。
    /// </summary>
    /// <param name="str">調査する文字列</param>
    /// <returns>Hanyo-Denshiとして無効な異体字を含む場合はtrueを返します。</returns>
    public static bool HasInvalidVariationSelectorAsHanyoDenshi(string str) => HasInvalidVariationSelector(str, IvsCollectionType.HanyoDenshi);

    /// <summary>
    /// Moji_Johoとして無効な異体字を含むかどうかを判定します。
    /// </summary>
    /// <param name="str">調査する文字列</param>
    /// <returns>Moji_Johoとして無効な異体字を含む場合はtrueを返します。</returns>
    public static bool HasInvalidVariationSelectorAsMojiJoho(string str) => HasInvalidVariationSelector(str, IvsCollectionType.MojiJoho);

    private static Func<CIInfo, Rune> GetFunc(CIConvertOption option)
    {
        return option switch
        {
            CIConvertOption.ToSvs => (CIInfo ci) => ci.SvsRune,
            CIConvertOption.ToAdobeJapan1 => (CIInfo ci) => ci.AdobeJapan1IvsRune,
            CIConvertOption.ToMojiJoho => (CIInfo ci) => ci.MojiJohoIvsRune,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// CJK互換漢字を,SVSを使用した表現に変換します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ConvertCompatibilityIdeographs(string str)
    {
       return ItaijiUtility.ConvertCompatibilityIdeographs(str, CIConvertOption.ToSvs);
    }

    /// <summary>
    /// CJK互換漢字を,SVSまたはIVSを使用した表現に変換します。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    public static string ConvertCompatibilityIdeographs(string str, CIConvertOption option)
    {
        //最大str.Length*3のバッファが必要なのだが、そこまで確保すると無駄が多そう・・・
        var runeFunc = GetFunc(option);
        using var sb = new RuneStringBuilder(str.Length * 2);
        foreach (var kanji in str.EnumerateKanji())
        {
            if (Library.CIDictionary.TryGetValue(kanji, out var ciInfo))
            {
                sb.Add(ciInfo.BaseRune);
                sb.Add(runeFunc(ciInfo));
            }
            else
            {
                sb.Add(kanji);
            }
        }
        return sb.ToString();
    }

}
