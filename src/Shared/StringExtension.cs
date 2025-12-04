namespace Itaiji;

/// <summary>
/// 文字列の拡張メソッドを提供します。
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 文字列を異体字セレクターを考慮した漢字列として列挙します。
    /// 漢字以外の文字（絵文字など）はRuneごとに分割されることに注意してください
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static KanjiEnumerator EnumerateKanji(this string str)
    {
        return new KanjiEnumerator(str);
    }

    /// <summary>
    /// 異体字セレクタを考慮したときの文字列の長さを取得します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int LengthAsKanji(this string str)
    {
        return new KanjiEnumerator(str).Count();
    }

    /// <summary>
    /// 文字列を異体字セレクターを考慮した漢字の配列に分割します。
    /// 漢字以外の文字（絵文字など）はRuneごとに分割されることに注意してください
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static KanjiChar[] SplitKanji(this string str)
    {
        return new KanjiEnumerator(str).ToArray();
    }

    /// <summary>
    /// Adobe-Japan1として無効な異体字セレクタを含まないかどうかを判定します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsValidAsAdobeJapan(this string str)
    {
        foreach (var kanji in str.EnumerateKanji())
        {
            if (kanji.IvsType != IvsType.None && !kanji.IvsType.HasBitFlag(IvsType.AdobeJapan))
            {
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Hanyo-Denshiとして無効な異体字セレクタを含まないかどうかを判定します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsValidAsHanyoDenshi(this string str)
    {
        foreach (var kanji in str.EnumerateKanji())
        {
            if (kanji.IvsType != IvsType.None && !kanji.IvsType.HasBitFlag(IvsType.HanyoDenshi))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Hanyo-Denshiとして無効な異体字セレクタを含まないかどうかを判定します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsValidAsMojiJoho(this string str)
    {
        foreach (var kanji in str.EnumerateKanji())
        {
            if (kanji.IvsType != IvsType.None && !kanji.IvsType.HasBitFlag(IvsType.MojiJoho))
            {
                return false;
            }
        }
        return true;
    }


}
