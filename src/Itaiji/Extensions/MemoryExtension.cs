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
}
