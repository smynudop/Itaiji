---
name: csharp-coding-standard
description: C#のコーディング規約（ライブラリ独自の実装を含む）をまとめたドキュメントです。
---


# コーディング規約
- XMLコメント・ドキュメントを重視します。日本語で記載します。
- C# 14を使用します。新しい機能を積極的に使用します。
- Nullable Reference Types を有効にします。


## パフォーマンスの向上

.NET 5+向けにはパフォーマンスを重視したコーディングをします。.NET Frameworkに対応していない実装の場合は、#if ディレクティブを使用して実装を切り替えます。

- .NET 5+ 向けには `Span<T>` を積極的に使用します。.Net Framework向けには提供しません。
- 辞書データは遅延初期化を行います。


### 省アロケーション

ライブラリであるため、省アロケーションを重視します。

- 可能であればclassの代わりに構造体を使用します。可能な限りreadonlyにし、Box化や防衛的コピーを避けます。
- .NET5+では、小サイズ・短寿命のバッファ確保には `stackalloc` を使用します。サイズが大きい場合は `ArrayPool<T>` を活用します。stackallocの上限は1024バイトを目安とします。

```csharp
// not good (.NET Frameworkのみok)
var buffer = new byte[256]; // 256バイトの配列がヒープに確保される

// good - stackallocを使用して、256バイトのバッファがスタックに確保される
Span<byte> buffer = stackalloc byte[256];

// good - サイズが大きい場合はArrayPoolを使用して、ヒープに確保される配列の数を減らす
var buffer = ArrayPool<byte>.Shared.Rent(1024); // 1024バイトの配列がヒープに確保される
ArrayPool<byte>.Shared.Return(buffer); // 使用後は必ず返却する

// never - ループ内でstackallocを用いてバッファを確保するのは避ける。スタックオーバーフローのリスクがある。
for(int i = 0; i < 1000; i++)
{
    Span<byte> buffer = stackalloc byte[256]; // ループ内でのstackallocは避ける
    // 何らかの処理
}
```

- stringをKanji単位で列挙する場合は `EnumerateKanji()` と `foreach` を活用し、不要な配列やリストの確保を避けます。

```csharp
//bad - EnumerateKanji()の結果をListに変換しているため、余計な配列が確保される
var kanjiList = str.EnumerateKanji().ToList();
for(var i = 0; i < kanjiList.Count; i++)
{
  // 何らかの処理
}

//good - EnumerateKanji()の結果を直接foreachで列挙しているため、余計な配列が確保されない
var i = 0;
foreach(var kanji in str.EnumerateKanji())
{
  // 何らかの処理
  i++;
}
```

- パフォーマンスが重要な場面では、Linqは使用しません。

```csharp
// bad - Linqを使用しているため、余計なデリゲートやイテレータが生成される
var variantCount = str.EnumerateKanji().Where(k => k.IsVariant).Count();

// good - Linqを使用せず、直接ループで処理することで、余計なデリゲートやイテレータの生成を避ける
var variantCount = 0;
foreach(var kanji in str.EnumerateKanji())
{
    if(kanji.IsVariant)
    {
        variantCount++;
    }
}
```

- 新たな文字列を生成する処理では、`StringBuilder`を活用します。ただし、`StringBuilder`はRuneを受け取るオーバーロードがないため、Rune/KanjiChar単位での結合が必要な場合は `RuneStringBuilder` を使用します。



## 命名規則
- C# の一般的な命名規則に従います。
- Ideographic Variation Sequence はIvsと省略します。
- Standardized Variation Sequence はSvsと省略します。
- 異体字セレクタ全般を扱う場合はVariationSelector(Vs)とします。
- 「異体字セレクタを考慮する」メソッドは以下のように命名します。
	- IvsComparisonをユーザーが指定できる場合 -> メソッド名の末尾にWithIvsを付与
	- Ivsの違いを考慮する場合 -> メソッド名の末尾にRespectIvsを付与
	- Ivsの違いを無視する場合 -> メソッド名の末尾にIgnoreIvsを付与

