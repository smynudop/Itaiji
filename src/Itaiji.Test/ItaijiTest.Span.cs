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

    [TestMethod]
    [DynamicData(nameof(RemoveIvsTestDataSamples))]
    public void RemoveIvsSpanTest(RemoveIvsTestData data)
    {
        var source = data.Source();
        Assert.AreEqual(data.ExpectedRemoveAll(), ItaijiUtility.RemoveVariationSelector(source.AsSpan()));
        Assert.AreEqual(data.ExpectedRemoveOnlyIvs(), ItaijiUtility.RemoveIvs(source.AsSpan(), RemoveIvsOption.RemoveAll));
        Assert.AreEqual(data.ExpectedRemoveOnlyIvsToSvs(), ItaijiUtility.RemoveIvs(source.AsSpan(), RemoveIvsOption.RemoveToSvs));
    }



    [TestMethod]
    [DynamicData(nameof(CIConvertTestDataSamples))]
    public void CIConvertSpanTest(CIConvertTestData data)
    {
        var baseStr = data.CIRune().AsSpan();
        Assert.AreEqual(
            data.SvsRune(),
            ItaijiUtility.ConvertCompatibilityIdeographs(baseStr, CIConvertOption.ToSvs)
            );
        Assert.AreEqual(
            data.AdobeJapan1IvsRune(),
            ItaijiUtility.ConvertCompatibilityIdeographs(baseStr, CIConvertOption.ToAdobeJapan1)
            );
        Assert.AreEqual(
            data.MojiJohoIvsRune(),
            ItaijiUtility.ConvertCompatibilityIdeographs(baseStr, CIConvertOption.ToMojiJoho)
            );
    }
}
