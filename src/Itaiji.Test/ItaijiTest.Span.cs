using Itaiji.Extensions;

namespace Itaiji.Test;

public partial class ItaijiTest
{

    private static IEnumerable<T> EnumerateHelper<T>(IEnumerator<T> items)
    {
        while (items.MoveNext())
        {
            yield return items.Current;
        }
    }

    [TestMethod]
    [DynamicData(nameof(EnumerateTestDataSamples))]
    public void EnumerateKanjiSpanTest(EnumerateTestData data)
    {
        var list = new List<KanjiChar>();
        var enumerator = data.Source().AsSpan().EnumerateKanji();   
        foreach (var item in enumerator) {
            list.Add(item);
        }


        CollectionAssert.AreEqual(data.ExpectedKanjiChars(),list);
    }
}
