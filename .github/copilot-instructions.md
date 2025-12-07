# Itaiji

Itaiji は .NET 向けの異体字処理ライブラリです。
主な機能として、異体字の正規化、異体字の検索、異体字の変換などを提供します。

## 本ライブラリの対象とする字
本ライブラリは主に日本で使用される異体字(Adobe-Japan1/Hanyo-Denshi/Moji_Joho)を対象としています。
それ以外のIVSを用いる異体字については、扱うことはできますが、コレクションの判定はできません。

また、Standardized Variants(SVS)については現状サポートしていません。

# パッケージ構成
- Itaiji: .NET 5.0+ 向けのライブラリ
- Itaiji.NetFramework: .NET Framework 3.5+ 向けのライブラリ

1パッケージにせずに分けている理由は、本ライブラリが依存するSystem.Text.Rune が .NET Framework には存在せず、
アップグレード時のライブラリの互換性がないためです。
(System.Text.Rune を独自実装すれば可能ですが、お行儀がわるいため避けています)
.NET Framework 向けにはRuneをバックポートして含めています。

また、依存をなくすために、.NET Framework 向けには一部の機能(Spanを使用する機能など)を削減しています。



# フォルダ構成
- `src/`: ライブラリのソースコード
	- src/Itaiji/: Itaiji ライブラリの主要コード
	- src/Itaiji.Test/: ユニットテストコード
	- src/Itaiji.NetFramework/: .NET Framework 向けのプロジェクト
	- src/Itaiji.NetFramework.Test/: .NET Standard 向けのユニットテストコード
	- src/Shared/: 共通コード
	- src/Shared.Test/: テストコードの共通コード

---

# コーディング規約
- XMLコメント・ドキュメントを重視します。日本語で記載します。
- C# 14を使用します。新しい機能を積極的に使用します。
- Nullable Reference Types を有効にします。
- ライブラリであるため、省アロケーションを重視します。
	-　`Span<T>` や `Memory<T>` を積極的に使用します
	- .NET Frameworkでは `Span<T>` を使用できないため、代替コードを使用します。#if ディレクティブを使用して切り替えます。	 
	- 小サイズ・短寿命のバッファ確保には `stackalloc` を使用します。.NET Framework向けには`ArrayPool<T>` を使用します。	
	- 構造体はreadonlyにし、可能な限りBox化、防衛的コピーを避けます。
	- Linqは使用しません。foreachは使用しても構いません。

## 命名規則
- C# の一般的な命名規則に従います。
- Ideographic Variation Sequence はIvsと省略します。
	- 異体字セレクタ全般を扱う場合はVariationSelector(Vs)としますが、本ライブラリでは現在IVSのみを扱うため、Ivsとします。
- 「異体字セレクタを考慮する」メソッドは以下のように命名します。
	- IvsComparisonをユーザーが指定できる場合 -> メソッド名の末尾にWithIvsを付与
	- Ivsの違いを考慮する場合 -> メソッド名の末尾にRespectIvsを付与
	- Ivsの違いを無視する場合 -> メソッド名の末尾にIgnoreIvsを付与


# KanjiChar 構造体の設計方針
- メンバとしてRune2つを持ちます。1つ目が基本字、2つ目がIVSです。
- IVSを持たない場合、内部的には2つ目のRuneはU+0000とします。publicにはnullableで提供します。