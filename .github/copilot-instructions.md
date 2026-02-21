# Itaiji

repository: smynudop/Itaiji

Itaiji は .NET 向けの異体字処理ライブラリです。
主な機能として、異体字の正規化、異体字の検索、異体字の変換などを提供します。

## 本ライブラリの対象とする字
本ライブラリは主に日本で使用される異体字(Adobe-Japan1/Hanyo-Denshi/Moji_Joho)を対象としています。
日本で使用する漢字についてはSVS(Standardized Variation Sequence)も扱うことができます。

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




