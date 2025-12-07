Itaiji
===

# 概要
Itaijiは、.NETでUnicodeの異体字(Ideographic Variation Sequence)を便利に扱うためのライブラリです。
.Net 5+, .Net Framework 3.5/4.0/4.5/4.6/4.7/4.8 に対応しています。

# 主な機能
- 文字列から異体字セレクタの削除
- 異体字セレクタを考慮した文字列の比較・検索
- 異体字セレクタやサロゲートペアを考慮した文字の列挙

# IVSとは
IVS（Ideographic Variation Sequence）は、同じ漢字の異なる形状を表現するためのUnicodeの仕組みです。

有名な話ですが、渡辺さんの「ナベ」の漢字には、バリエーションが数十種類あると言われています。
それらすべてにunicodeのコードポイントが割り当てられているわけではなく、
基本となる漢字に対して異体字セレクタを組み合わせることで、特定の形状を表現しています。

なお、異体字セレクタにはIVSのほかに、SVS(Standardized Variation Sequence)もありますが、
Itaijiは現状、IVSのみを対象としています。

# サロゲートペアについて
.NETの文字列はUTF-16で扱われます。
unicodeのコードポイントがU+FFFFを超える場合、
1文字が2つの16ビットコードユニット（サロゲートペア）で表現されます。
IVSそのものもサロゲートペアで表現されます。

サロゲートペアで表される漢字の有名な例として、𩸽(ほっけ)や𠮷(つちよし)があります。

そのため、文字列を単純にcharの配列として扱うと、サロゲートペアが分割されてしまい、
正しい文字列操作ができなくなることがあります。

Itaijiは、System.Text.Runeを活用して、サロゲートペアを正しく扱います。
Runeを実装しない.NET Framework向けには、互換構造体を提供しています。

# 使用例
```csharp
using Itaiji;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var name = "渡辺󠄀";

//異体字を除去する
var normalized = ItaijiUtility.RemoveIvs(name);

// 異体字を考慮して比較する
var isEqual = ItaijiUtility.Equals("渡辺󠄀", "渡辺");

// 異体字を考慮して部分文字列を検索する
var (index, length) = ItaijiUtility.IndexOf("渡辺󠄀さん", "辺");

// 異体字やサロゲートペアを考慮して文字を列挙する
foreach (var ch in ItaijiUtility.EnumerateKanji("渡辺󠄀さん"))
{
	Console.WriteLine(ch);
}
```

