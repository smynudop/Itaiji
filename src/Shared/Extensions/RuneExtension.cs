#if NETSTANDARD2_0 || NETFRAMEWORK
using Itaiji.Text;
#endif

namespace Itaiji.Extensions;

/// <summary>
/// Runeの拡張メソッドを提供します。
/// </summary>
public static class RuneExtension
{
    /// <summary>
    /// このRuneが異体字セレクタ(IVS,0xE0100～0xE01EF またはSVS,0xFE00～0xFE0F)であるかどうかを判定します。
    /// </summary>
    /// <param name="rune"></param>
    /// <returns></returns>
    public static bool IsVariationSelector(this Rune rune)
    {
        return IsIvs(rune) || IsSvs(rune);
    }

    /// <summary>
    /// このRuneがStandardized Variation Sequence (0xFE00～0xFE0F)であるかどうかを判定します。
    /// </summary>
    /// <param name="rune"></param>
    /// <returns></returns>
    public static bool IsSvs(this Rune rune)
    {
        return 0xFE00 <= rune.Value && rune.Value <= 0xFE0F;

    }
    /// <summary>
    /// このRuneがIdeographic Variation Sequence(0xE0100～0xE01EF)であるかどうかを判定します。
    /// </summary>
    /// <param name="rune"></param>
    /// <returns></returns>
    public static bool IsIvs(this Rune rune)
    {
        return 0xE0100 <= rune.Value && rune.Value <= 0xE01EF;
    }
}
