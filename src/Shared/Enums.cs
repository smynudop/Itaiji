namespace Itaiji;

/// <summary>
/// 異体字が有効なコレクションを表します。
/// 同一のコードポイントを複数のセットで共有する可能性があるため、ビットマスクで表現されます。
/// </summary>
[Flags]
public enum IvsCollectionType : int
{
    /// <summary>
    /// この字は異体字セレクタを持ちません。
    /// </summary>
    None = 0,
    /// <summary>
    /// adobe-Japan1として有効な異体字です。
    /// </summary>
    AdobeJapan = 1 << 0,
    /// <summary>
    /// Hanyo-Denshiとして有効な異体字です。
    /// </summary>
    HanyoDenshi = 1 << 1,
    /// <summary>
    /// Moji_Johoとして有効な異体字です。
    /// </summary>
    MojiJoho = 1 << 2,
    /// <summary>
    /// CJK互換異体字として有効な異体字です。
    /// </summary>
    CJKCompatibilityIdeographs = 1<< 3,

    /// <summary>
    /// このライブラリでは未知の異体字です。
    /// </summary>
    Unknown = 1 << 31,

    /// <summary>
    /// Hanyo-DenshiおよびMoji-Johoとして有効な異体字です。
    /// </summary>
    HDandMJ = HanyoDenshi | MojiJoho,
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

/// <summary>
/// 互換漢字を変換するときの変換先を指定します。
/// </summary>
public enum CIConvertOption
{
    /// <summary>
    /// SVSに変換します。
    /// </summary>
    ToSvs,
    /// <summary>
    /// Adobe-Japan1のIVSに変換します。
    /// </summary>
    ToAdobeJapan1,
    /// <summary>
    /// Moji_JohoのIVSに変換します。
    /// </summary>
    ToMojiJoho,
}

/// <summary>
/// Ivsを除去する方法を指定します。
/// </summary>
public enum RemoveIvsOption
{
    /// <summary>
    /// 異体字セレクタをすべて除去します。
    /// </summary>
    RemoveAll,
    /// <summary>
    /// Svsとして表現可能なものはSvsに変換し、それ以外は除去します。
    /// </summary>
    RemoveToSvs,
}