namespace Itaiji;

internal static class EnumExtension
{
    /// <summary>
    /// HasFlagの独自実装(.net 5未満は遅いらしい)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static bool HasBitFlag(this IvsType value, IvsType flag)
    {
        return (value & flag) == flag;
    }
}
