namespace Itaiji.Test;

[TestClass]
public class ItaijiTestSpan
{
    [TestMethod]
    [Timeout(100, CooperativeCancellation = true)]
    public void EnumerateKanjiTest1()
    {
        var str = "辻太郎"; // ふつうの辻
        var list = new List<KanjiChar>();
        foreach (var k in str.AsSpan().EnumerateKanji())
        {
            list.Add(k);
        }
        CollectionAssert.AreEqual(new KanjiChar[3] {
            new KanjiChar('辻'),
            new KanjiChar('太'),
            new KanjiChar('郎')
        }, list);
    }

    [TestMethod]
    public void EnumerateKanjiTest2()
    {
        var str = "辻󠄀太郎"; // adobe-japanのツジ
        var list = new List<KanjiChar>();
        foreach (var k in str.AsSpan().EnumerateKanji())
        {
            list.Add(k);
        }
        var en = str.AsSpan().EnumerateKanji();
        CollectionAssert.AreEqual(new KanjiChar[3] {
            new KanjiChar('辻', 0x00),
            new KanjiChar('太'),
            new KanjiChar('郎')
        }, list);
    }


}
