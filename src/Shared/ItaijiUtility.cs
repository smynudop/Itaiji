using System.Linq;
using Itaiji.Extensions;
#if NETSTANDARD2_0 || NETFRAMEWORK
using Itaiji.Text;
#endif

namespace Itaiji;

/// <summary>
/// 異体字を考慮した文字列操作のユーティリティ関数を提供します。
/// </summary>
public static class ItaijiUtility
{

    /// <summary>
    /// 異体字セレクタを考慮したときの文字列の長さを取得します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 異体字を無視して文字列が等しいか調べます。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
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
    /// 異体字を無視して文字列が等しいか調べます。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool EqualsIgnoreIvs(string a, string b)
    {
        return Equals(a, b, IvsComparison.IgnoreIvs);
    }

    /// <summary>
    /// 異体字を無視して文字列が等しいか調べます。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool EqualsExactMatch(string a, string b)
    {
        return Equals(a, b, IvsComparison.ExactMatch);
    }

    /// <summary>
    /// 異体字を無視して文字列の中にキーワードが含まれているか調べます。
    /// </summary>
    /// <param name="str">対象の文字列</param>
    /// <param name="keyword">調べる文字列</param>
    /// <param name="comparison">異体字の比較方法</param>   
    /// <returns>存在する場合は、開始indexと文字列の長さ。存在しない場合は、(-1, 0)</returns>
    public static bool Contains(string str, string keyword, IvsComparison comparison)
    {
        return TryFindIndex(str, keyword, comparison, out _, out _);
    }

#if NET47_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// 異体字を無視して文字列の中にキーワードが含まれているか調べます。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="keyword"></param>
    /// <param name="comparison">異体字の比較方法</param>
    /// <returns>存在する場合は、開始indexと文字列の長さ。存在しない場合は、(-1, 0)</returns>
    public static (int index, int length) FindIndex(string str, string keyword, IvsComparison comparison)
    {
        TryFindIndex(str, keyword, comparison, out var index, out var length);
        return (index, length);
    }
#endif

    /// <summary>
    /// 異体字を無視して文字列の中にキーワードが含まれているか調べます。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="keyword"></param>
    /// <param name="comparison">異体字の比較方法</param>
    /// <param name="index">開始index</param>
    /// <param name="length">文字列の長さ</param>
    /// <returns>存在する場合は、開始indexと文字列の長さ。存在しない場合は、(-1, 0)</returns>
    public static bool TryFindIndex(string str, string keyword, IvsComparison comparison, out int index, out int length)
    {
        Func<KanjiChar, KanjiChar, bool> equalsFunc = GetEqualsFunc(comparison);

        // KMP法の準備
        var keywordKanjis = keyword.EnumerateKanji().ToArray();
        var kanjiEnumerator = str.EnumerateKanji();

        var matchTable = new int[keywordKanjis.Length];
        matchTable[0] = -1;
        var j = -1;
        for (int i = 0; i < keywordKanjis.Length - 1; i++)
        {
            while (j >= 0 && keywordKanjis[i].BaseRune != keywordKanjis[j].BaseRune)
            {
                j = matchTable[j];
            }
            matchTable[i + 1] = j + 1;
            j++;
        }

        var cursor = 0;
        index = 0;
        var matchQueue = new Queue<KanjiChar>(keywordKanjis.Length);

        var hasNext = kanjiEnumerator.MoveNext();
        while (hasNext)
        {
            if (equalsFunc(kanjiEnumerator.Current, keywordKanjis[cursor]))
            {
                matchQueue.Enqueue(kanjiEnumerator.Current);
                cursor++;
                hasNext = kanjiEnumerator.MoveNext();
                if (cursor >= keywordKanjis.Length)
                {
                    length = 0;
                    foreach (var k in matchQueue)
                    {
                        length += k.Utf16SequenceLength;
                    }
                    return true;
                }
            }
            else
            {
                if (cursor > 0)
                {
                    for (var i = 0; i < cursor - matchTable[cursor]; i++)
                    {
                        var k = matchQueue.Dequeue();
                        index += k.Utf16SequenceLength;
                    }
                    cursor = matchTable[cursor];
                }
                else
                {
                    index += kanjiEnumerator.Current.Utf16SequenceLength;
                    hasNext = kanjiEnumerator.MoveNext();
                }
            }
        }
        index = -1;
        length = 0;
        return false;
    }

    /// <summary>
    /// 文字列から異体字セレクターをすべて取り除きます。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RemoveIvs(string str)
    {
#if NETFRAMEWORK
        int length = 0;
        char[] result = new char[str.Length];
        foreach (var rune in str.EnumerateRunes())
        {
            if (rune.IsIVS())
            {
                continue;
            }
            length += rune.EncodeToUtf16(result, length);
        }
        return new string(result, 0, length);

#else
        int length = 0;
        Span<char> result = new char[str.Length];
        foreach (var rune in str.AsSpan().EnumerateRunes())
        {
            if (rune.IsIVS())
            {
                continue;
            }
            length += rune.EncodeToUtf16(result.Slice(length));
        }
        return new string(result.Slice(0, length));
#endif
    }

    /// <summary>
    /// Adobe-Japan1として無効な異体字セレクタを含まないかどうかを判定します。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public static bool HasInvalidIvs(string str, IvsCollectionType targetType)
    {
        if(targetType == IvsCollectionType.None)
        {
            throw new System.ArgumentException("targetType must not be None.", nameof(targetType));
        }

        foreach (var kanji in str.EnumerateKanji())
        {
            var ivsType = kanji.GetIvsCollectionType();
            if (ivsType != IvsCollectionType.None && !ivsType.HasBitFlag(targetType))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Adobe-Japan1として無効な異体字セレクタを含むかどうかを判定します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool HasInvalidIvsAsAdobeJapan1(string str) => HasInvalidIvs(str, IvsCollectionType.AdobeJapan);

    /// <summary>
    /// Adobe-Japan1として無効な異体字セレクタを含まないかどうかを判定します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool HasInvalidIvsAsHanyoDenshi(string str) => HasInvalidIvs(str, IvsCollectionType.HanyoDenshi);

    /// <summary>
    /// Adobe-Japan1として無効な異体字セレクタを含まないかどうかを判定します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool HasInvalidIvsAsMojiJoho(string str) => HasInvalidIvs(str, IvsCollectionType.MojiJoho);


}

/// <summary>
/// IVSの比較方法を指定します。
/// </summary>
public enum IvsComparison
{
    /// <summary>
    /// 異体字セレクタまで含めて一致を調べます。
    /// </summary>
    ExactMatch,
    /// <summary>
    /// ベースのRuneが同じであれば、異体字セレクタの有無にかかわらず、同一視します。
    /// </summary>
    IgnoreIvs,
}