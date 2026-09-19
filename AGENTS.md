# Itaiji

github repository: smynudop/Itaiji

Itaiji は .NET 向けの異体字処理ライブラリです。
主な機能として、異体字の正規化、異体字の検索、異体字の変換などを提供します。

## ライブラリ概要
Readme.mdを参照のこと。

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
* 外部ライブラリへの依存をしない。
* ライブラリであるためパフォーマンスに配慮する。
	* `Span<T>` 等は.net Coreターゲットのみに使用する。

# バージョン管理について
* 現在は0.xであるため、破壊的変更があってもminor バージョン、軽微な修正等はpatch バージョンで更新します。

## バージョンを上げる際の手順
* `src/Itaiji/Itaiji.csproj`, `src/Itaiji.NetFramework/Itaiji.NetFramework.csproj` の両方の `<VersionPrefix>`, `<AssemblyVersion>`, `<FileVersion>` タグを更新する。




