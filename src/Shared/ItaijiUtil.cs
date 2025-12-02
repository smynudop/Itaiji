#if NETSTANDARD2_0 || NETFRAMEWORK
using Itaiji.Text;
#endif

namespace Itaiji;

public static class ItaijiUtil
{
    public static bool EqualsIgnoreIvs(string a, string b)
    {
        var enumeratorA = a.EnumerateKanji();
        var enumeratorB = b.EnumerateKanji();
        while (true)
        {
            var hasA = enumeratorA.MoveNext();
            var hasB = enumeratorB.MoveNext();
            if (!hasA && !hasB)
            {
                return true;
            }
            if (hasA != hasB)
            {
                return false;
            }
            var kanjiA = enumeratorA.Current;
            var kanjiB = enumeratorB.Current;
            if (kanjiA.BaseRune != kanjiB.BaseRune)
            {
                return false;
            }
        }
    }
}