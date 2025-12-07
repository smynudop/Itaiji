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
    /// このライブラリでは未知の異体字です。
    /// </summary>
    Unknown = 1 << 10,

    /// <summary>
    /// Hanyo-DenshiおよびMoji-Johoとして有効な異体字です。
    /// </summary>
    HDandMJ = HanyoDenshi | MojiJoho,
}
