using System;
using System.Collections.Generic;
using System.Text;
using Itaiji.Text;

namespace Itaiji;

/// <summary>
/// 文字列に関する拡張メソッドのポリフィル
/// </summary>
public static class StringExtensionPolyFill
{
    /// <summary>
    /// 文字列からRuneを列挙します。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static StringRuneEnumerator EnumerateRunes(this string str)
    {
        return new StringRuneEnumerator(str);
    }
}
