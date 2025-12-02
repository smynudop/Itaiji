using System.Text;
#if NETSTANDARD2_0 || NETFRAMEWORK
using Itaiji.Text;
#endif

namespace Itaiji;

/// <summary>
/// Runeの拡張メソッドを提供します。
/// </summary>
public static class RuneExtension
{
    /// <summary>
    /// このRuneが異体字セレクタ(0xE0100～0xE01EF)であるかどうかを判定します。
    /// </summary>
    /// <param name="rune"></param>
    /// <returns></returns>
    public static bool IsIVS(this Rune rune)
    {
        return 0xE0100 <= rune.Value && rune.Value <= 0xE01EF;
    }
}
