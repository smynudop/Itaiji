namespace Itaiji.Extensions;

/// <summary>
/// 
/// </summary>
public static class ItaijiMemoryExtension
{
    /// <summary>
    /// 文字列を異体字セレクターを考慮した漢字列として列挙します。
    /// 漢字以外の文字（絵文字など）はRuneごとに分割されることに注意してください
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static SpanKanjiEnumerator EnumerateKanji(this ReadOnlySpan<char> str)
    {
        return new SpanKanjiEnumerator(str);
    }

    /// <summary>
    /// 文字列から異体字セレクタを除去します。
    /// </summary>
    /// <param name="source">対象の文字列</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveVariationSelector(this ReadOnlySpan<char> source)
            => ItaijiUtility.RemoveVariationSelector(source);

    /// <summary>
    /// 文字列からIVSを除去します。
    /// </summary>
    /// <param name="source">対象の文字列</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveIvs(this ReadOnlySpan<char> source)
            => ItaijiUtility.RemoveIvs(source);

    /// <summary>
    /// 文字列からIVSを除去します。
    /// </summary>
    /// <param name="source">対象の文字列</param>
    /// <param name="option">異体字セレクター除去のオプション</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveIvs(this ReadOnlySpan<char> source, RemoveIvsOption option)
            => ItaijiUtility.RemoveIvs(source, option);

    /// <summary>
    /// CJK互換漢字を,SVSを使用した表現に変換します。
    /// </summary>
    /// <param name="source">変換する文字列</param>
    /// <returns></returns>
    public static string ComvertCompabilityVariationSelector(this ReadOnlySpan<char> source)
            => ItaijiUtility.ConvertCompatibilityIdeographs(source);

    /// <summary>
    /// CJK互換漢字を,SVSまたはIVSを使用した表現に変換します。
    /// </summary>
    /// <param name="source">変換する文字列</param>
    /// <param name="option">変換先</param>
    /// <returns></returns>
    public static string ComvertCompabilityVariationSelector(this ReadOnlySpan<char> source, CIConvertOption option)
            => ItaijiUtility.ConvertCompatibilityIdeographs(source, option);
}
