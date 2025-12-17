namespace Itaiji.Extensions;

/// <summary>
/// 異体字に関する文字列の拡張メソッドを提供します。
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 異体字を考慮して文字列が等しいか調べます。
    /// </summary>
    /// <param name="a">比較対象の文字列（左辺）</param>
    /// <param name="b">比較対象の文字列（右辺）</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <returns>等しい場合はtrue、それ以外はfalseを返します。</returns>
    public static bool EqualsWithIvs(this string a, string b, IvsComparison comparison) 
            => ItaijiUtility.Equals(a, b, comparison);

    /// <summary>
    /// 異体字の違いを無視して文字列が等しいか調べます。
    /// </summary>
    /// <param name="a">比較対象の文字列（左辺）</param>
    /// <param name="b">比較対象の文字列（右辺）</param>
    /// <returns>異体字を無視して等しい場合はtrue、それ以外はfalseを返します。</returns>
    public static bool EqualsIgnoreIvs(this string a, string b)
            => ItaijiUtility.Equals(a, b, IvsComparison.IgnoreIvs);

    /// <summary>
    /// 異体字の違いを厳密に考慮して文字列が等しいか調べます。
    /// </summary>
    /// <remarks>このメソッドはstring.Equalsと同じ動作をします。</remarks>
    /// <param name="a">比較対象の文字列（左辺）</param>
    /// <param name="b">比較対象の文字列（右辺）</param>
    /// <returns>異体字も区別して等しい場合はtrue、それ以外はfalseを返します。</returns>
    public static bool EqualsRespectIvs(this string a, string b)
            => ItaijiUtility.Equals(a, b, IvsComparison.ExactMatch);

    /// <summary>
    /// 異体字を考慮して、文字列が指定した部分文字列を含むか調べます。
    /// </summary>
    /// <param name="source">検索対象の文字列</param>
    /// <param name="target">検索する部分文字列</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <returns>部分文字列が存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool ContainsWithIvs(this string source, string target, IvsComparison comparison)
            => ItaijiUtility.Contains(source, target, comparison);

    /// <summary>
    /// 異体字の違いを無視して、文字列が指定した部分文字列を含むか調べます。
    /// </summary>
    /// <param name="source">検索対象の文字列</param>
    /// <param name="target">検索する部分文字列</param>
    /// <returns>異体字を無視して部分文字列が存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool ContainsIgnoreIvs(this string source, string target)
            => ItaijiUtility.Contains(source, target, IvsComparison.IgnoreIvs);

    /// <summary>
    /// 異体字の違いを考慮して、文字列が指定した部分文字列を含むか調べます。
    /// </summary>
    /// <param name="source">検索対象の文字列</param>
    /// <param name="target">検索する部分文字列</param>
    /// <returns>異体字を区別して部分文字列が存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool ContainsRespectIvs(this string source, string target)
            => ItaijiUtility.Contains(source, target, IvsComparison.ExactMatch);

#if NET47_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// 異体字を考慮して、文字列が指定した部分文字列を含むか調べ、その開始indexとchar換算でのlengthを返します。
    /// </summary>
    /// <param name="source">検索対象の文字列</param>
    /// <param name="target">検索する部分文字列</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <returns>見つかった場合は開始indexとchar単位のlengthを返します。見つからなければ(-1,0)を返します。</returns>
    public static (int index, int length) FindIndexWithIvs(this string source, string target, IvsComparison comparison)
            => ItaijiUtility.FindIndex(source, target, comparison);

    /// <summary>
    /// 異体字をの違いを無視して、文字列が指定した部分文字列を含むか調べ、その開始indexとchar換算でのlengthを返します。
    /// </summary>
    /// <param name="source">検索対象の文字列</param>
    /// <param name="target">検索する部分文字列</param>
    /// <returns>見つかった場合は開始indexとchar単位のlengthを返します。見つからなければ(-1,0)を返します。</returns>
    public static (int index, int length) FindIndexIgnoreIvs(this string source, string target)
            => ItaijiUtility.FindIndex(source, target, IvsComparison.IgnoreIvs);

    /// <summary>
    /// 異体字をの違いを考慮して、文字列が指定した部分文字列を含むか調べ、その開始indexとchar換算でのlengthを返します。
    /// </summary>
    /// <param name="source">検索対象の文字列</param>
    /// <param name="target">検索する部分文字列</param>
    /// <returns>見つかった場合は開始indexとchar単位のlengthを返します。見つからなければ(-1,0)を返します。</returns>
    public static (int index, int length) FindIndexRespectIvs(this string source, string target)
            => ItaijiUtility.FindIndex(source, target, IvsComparison.ExactMatch);
#endif

    /// <summary>
    /// 異体字を考慮して、文字列が指定した部分文字列を含むか調べます。
    /// </summary>
    /// <param name="source">検索対象の文字列</param>
    /// <param name="target">検索する部分文字列</param>
    /// <param name="comparison">異体字の比較方法を指定する列挙値</param>
    /// <param name="index">見つかった場合に開始indexが格納されます（見つからない場合は-1）</param>    
    /// <param name="length">見つかった場合にchar単位のlengthが格納されます（見つからない場合は0）</param>
    /// <returns>部分文字列が存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool TryFindIndexWithIvs(this string source, string target, IvsComparison comparison, out int index, out int length)
    {
        return ItaijiUtility.TryFindIndex(source, target, comparison, out index, out length);
    }

    /// <summary>
    /// 異体字をの違いを無視して、文字列が指定した部分文字列を含むか調べます。
    /// </summary>
    /// <param name="source">検索対象の文字列</param>
    /// <param name="target">検索する部分文字列</param>
    /// <param name="index">見つかった場合に開始indexが格納されます（見つからない場合は-1）</param>    
    /// <param name="length">見つかった場合にchar単位のlengthが格納されます（見つからない場合は0）</param>
    /// <returns>部分文字列が存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool TryFindIndexIgnoreIvs(this string source, string target, out int index, out int length)
            => ItaijiUtility.TryFindIndex(source, target, IvsComparison.IgnoreIvs, out index, out length);

    /// <summary>
    /// 異体字をの違いを考慮して、文字列が指定した部分文字列を含むか調べます。
    /// </summary>
    /// <param name="source">検索対象の文字列</param>
    /// <param name="target">検索する部分文字列</param>
    /// <param name="index">見つかった場合に開始indexが格納されます（見つからない場合は-1）</param>    
    /// <param name="length">見つかった場合にchar単位のlengthが格納されます（見つからない場合は0）</param>
    /// <returns>部分文字列が存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool TryFindIndexRespectIvs(this string source, string target, out int index, out int length)
            => ItaijiUtility.TryFindIndex(source, target, IvsComparison.ExactMatch, out index, out length);


    /// <summary>
    /// 文字列から異体字セレクタを除去します。
    /// </summary>
    /// <param name="source">対象の文字列</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveVariationSelector(this string source)
            => ItaijiUtility.RemoveVariationSelector(source);

    /// <summary>
    /// 文字列からIVSを除去します。
    /// </summary>
    /// <param name="source">対象の文字列</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveIvs(this string source)
            => ItaijiUtility.RemoveIvs(source);

    /// <summary>
    /// 文字列からIVSを除去します。
    /// </summary>
    /// <param name="source">対象の文字列</param>
    /// <param name="option">異体字セレクター除去のオプション</param>
    /// <returns>異体字セレクターを除去した新しい文字列を返します。</returns>
    public static string RemoveIvs(this string source, RemoveIvsOption option)
            => ItaijiUtility.RemoveIvs(source, option);

    /// <summary>
    /// CJK互換漢字を,SVSを使用した表現に変換します。
    /// </summary>
    /// <param name="source">変換する文字列</param>
    /// <returns></returns>
    public static string ComvertCompabilityVariationSelector(this string source)
            => ItaijiUtility.ConvertCompatibilityIdeographs(source);

    /// <summary>
    /// CJK互換漢字を,SVSまたはIVSを使用した表現に変換します。
    /// </summary>
    /// <param name="source">変換する文字列</param>
    /// <param name="option">変換先</param>
    /// <returns></returns>
    public static string ComvertCompabilityVariationSelector(this string source, CIConvertOption option)
            => ItaijiUtility.ConvertCompatibilityIdeographs(source, option);

    /// <summary>
    /// 文字列を異体字セレクターを考慮した漢字列として列挙します。
    /// 書記素分割ではありません。
    /// 正確に書記素で分割したい場合には<see cref="System.Globalization.StringInfo.GetTextElementEnumerator(string)"></see>を使用してください。
    /// </summary>
    /// <param name="str">対象の文字列</param>
    /// <returns>漢字とその異体字情報を考慮した列挙子を返します。</returns>
    public static KanjiEnumerator EnumerateKanji(this string str)　=> new KanjiEnumerator(str);

    /// <summary>
    /// 異体字セレクタを考慮したときの文字列の長さを取得します。
    /// このメソッドは厳密な書記素数を返しません。
    /// 正確な書記素数を取得したい場合は<see cref="System.Globalization.StringInfo"></see>を使用してください。
    /// </summary>
    /// <param name="str">対象の文字列</param>
    /// <returns>異体字セレクタを考慮した文字列の長さ（UTF-16単位ではなく、漢字列としての長さ）を返します。</returns>
    public static int LengthAsKanji(this string str) => ItaijiUtility.LengthAsKanji(str);

    /// <summary>
    /// 特定のコレクションに対して無効な異体字を含むかどうかを判定します。
    /// </summary>
    /// <param name="str">調査する文字列</param>
    /// <param name="ivsCollectionType">対象となるコレクションの種類</param>
    /// <returns>指定したコレクションで無効な異体字を含む場合はtrueを返します。</returns>
    public static bool HasInvalidVariationSelector(this string str, IvsCollectionType ivsCollectionType) => ItaijiUtility.HasInvalidVariationSelector(str, ivsCollectionType);


    /// <summary>
    /// Adobe-Japan1として無効な異体字を含むかどうかを判定します。
    /// </summary>
    /// <param name="str">調査する文字列</param>
    /// <returns>Adobe-Japan1として無効な異体字を含む場合はtrueを返します。</returns>
    public static bool HasInvalidVariationSelectorAsAdobeJapan1(this string str) => ItaijiUtility.HasInvalidVariationSelectorAsAdobeJapan1(str);


    /// <summary>
    /// Hanyo-Denshiとして無効な異体字を含むかどうかを判定します。
    /// </summary>
    /// <param name="str">調査する文字列</param>
    /// <returns>Hanyo-Denshiとして無効な異体字を含む場合はtrueを返します。</returns>
    public static bool HasInvalidVariationSelectorAsHanyoDenshi(this string str) => ItaijiUtility.HasInvalidVariationSelectorAsHanyoDenshi(str);


    /// <summary>
    /// Moji_Johoとして無効な異体字を含むかどうかを判定します。
    /// </summary>
    /// <param name="str">調査する文字列</param>
    /// <returns>Moji_Johoとして無効な異体字を含む場合はtrueを返します。</returns>
    public static bool HasInvalidVariationSelectorAsMojiJoho(this string str) => ItaijiUtility.HasInvalidVariationSelectorAsMojiJoho(str);



}
