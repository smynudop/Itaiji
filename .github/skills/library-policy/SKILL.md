---
name: library-policy
description: ライブラリの実装方針をまとめたドキュメントです。
---

# ライブラリ方針
- .NET 5+向けと、.NET Framework向けの2つのパッケージを提供します。
- .NET Framework向けの実装では、`Span<T>` や一部APIは使用できません。

## API実装方針
- 機能はItaijiUtilityクラスに静的メソッドとして実装します。
- StringExtensionsクラスに、string型の拡張メソッドを実装します。
- サロゲートペアへの対応のため、(charではなく)Runeを基本的な文字の単位として扱います。.net Framework向けにはRuneをバックポートして提供します。
- KanjiChar構造体を実装します。ベースとなるRuneと、IVS/SVSを表すRuneの2つのメンバを持ち、異体字を扱えます。あくまで漢字を表すための構造体であり、厳密にUnicodeの書記素を表せるものではありません。


## KanjiChar 構造体の設計方針
- メンバとして`_BaseRune`と`_VariationSelector`の2つのRuneを持ちます。
- `_BaseRune`
  - 基本となるRuneです。常に有効な漢字を表すRuneが入ります。
- `_VariationSelector`
  - IVS/SVSを表すRuneです。IVS/SVSがない場合はU+0000になります。
  - publicにはnullableなプロパティ`VariationSelector`を提供します。IVS/SVSがない場合はnullを返します。
  - Internalには`NonNullVariationSelector`プロパティを提供します。IVS/SVSがない場合はU+0000を返します。パフォーマンスの観点からわずかに有利です。
- KanjiCharの比較の種類は以下の2種類です。Equalsは後者となります。
  - BaseRuneのみを比較する方法
  - VariationSelectorも比較する方法
- ライブラリ検索用の値は`GetCode()`で取得できます。

