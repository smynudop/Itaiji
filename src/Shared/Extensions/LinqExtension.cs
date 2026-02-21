namespace Itaiji.Extensions;

/// <summary>
/// Linq拡張機能を提供します。
/// </summary>
public static class LinqExtensions
{
    /// <summary>
    /// IEnumerable&lt;KanjiChar&gt;を文字列に戻します。string.Join("", list)とほぼ同じです。
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string ToText(this IEnumerable<KanjiChar> list)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        var capacity = list is ICollection<KanjiChar> collection ? collection.Count : -1;
        var sb = new RuneStringBuilder(capacity);
        foreach (var item in list)
        {
            sb.Add(item);
        }
        return sb.ToString();
    }
}