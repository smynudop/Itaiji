
using Itaiji.Extensions;
using System.Buffers;

namespace Itaiji;

public partial class ItaijiUtility
{
    /// <summary>
    /// 文字列からIvs/Svsをすべて除去します。
    /// </summary>
    /// <param name="str">対象の文字列</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveVariationSelector(ReadOnlySpan<char> str)
    {
        using var sb = new RuneStringBuilder(str.Length);
        foreach (var rune in str.EnumerateRunes())
        {
            if(rune.IsVariationSelector())
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
    public static string RemoveIvs(ReadOnlySpan<char> str)
    {
        return RemoveIvs(str, RemoveIvsOption.RemoveAll);
    }

    /// <summary>
    /// 文字列からIvsを除去します。
    /// </summary>
    /// <param name="str">対象の文字列</param>
    /// <param name="option">除去方法</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveIvs(ReadOnlySpan<char> str, RemoveIvsOption option)
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
            if (Library.IvsToSvsDic.TryGetValue(kanji, out var svs))
            {
                sb.Add(svs);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// CJK互換漢字を,SVSを使用した表現に変換します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ConvertCompatibilityIdeographs(ReadOnlySpan<char> str)
    {
        return ItaijiUtility.ConvertCompatibilityIdeographs(str, CIConvertOption.ToSvs);
    }
    /// <summary>
    /// CJK互換漢字を,SVSまたはIVSを使用した表現に変換します。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    public static string ConvertCompatibilityIdeographs(ReadOnlySpan<char> str, CIConvertOption option)
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