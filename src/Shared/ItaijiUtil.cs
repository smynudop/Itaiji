#if NETSTANDARD2_0 || NETFRAMEWORK
using Itaiji.Text;
#endif

namespace Itaiji;

/// <summary>
/// 
/// </summary>
public static class ItaijiUtil
{
    /// <summary>
    /// 異体字を無視して文字列が等しいか調べます。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool EqualsIgnoreIvs(string a, string b)
    {
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
            if (kanjiA.BaseRune != kanjiB.BaseRune)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 異体字を無視して文字列の中にキーワードが含まれているか調べます。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="keyword"></param>
    /// <returns>存在する場合は、開始indexと文字列の長さ。存在しない場合は、(-1, 0)</returns>
    public static bool ContainsIgnoreIvs(string str, string keyword)
    {
        return TryFindIndexIgnoreIvs(str, keyword, out _, out _);
    }

#if NET47_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// 異体字を無視して文字列の中にキーワードが含まれているか調べます。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="keyword"></param>
    /// <returns>存在する場合は、開始indexと文字列の長さ。存在しない場合は、(-1, 0)</returns>
    public static (int index, int length) FindIndexIgnoreIvs(string str, string keyword)
    {
        TryFindIndexIgnoreIvs(str, keyword, out var index, out var length);
        return (index, length);
    }
#endif

    /// <summary>
    /// 異体字を無視して文字列の中にキーワードが含まれているか調べます。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="keyword"></param>
    /// <param name="index">開始index</param>
    /// <param name="length">文字列の長さ</param>
    /// <returns>存在する場合は、開始indexと文字列の長さ。存在しない場合は、(-1, 0)</returns>
    public static bool TryFindIndexIgnoreIvs(string str, string keyword, out int index, out int length)
    {
        // KMP法の準備
        var keywordKanjis = keyword.EnumerateKanji().ToArray();
#if NETFRAMEWORK
        var kanjiEnumerator = str.EnumerateKanji();
#else
        var kanjiEnumerator = str.AsSpan().EnumerateKanji();
#endif

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
            if (kanjiEnumerator.Current.BaseRune == keywordKanjis[cursor].BaseRune)
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
}