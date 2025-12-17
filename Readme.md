Itaiji
===

# 概要
Itaijiは、.NETでUnicodeの異体字(Ideographic Variation Sequence)を便利に扱うためのライブラリです。
.Net 5+, .Net Framework 3.5/4.0/4.5/4.6/4.7/4.8 に対応しています。

https://www.nuget.org/packages/Itaiji/
https://www.nuget.org/packages/Itaiji.NetFramework/

# 主な機能
- 文字列から異体字セレクタの削除
	- IVS/SVSの両方に対応
	- IVSのうち、SVSで表されるものはSVSで表すオプションあり
- 互換漢字を統合漢字+SVSを用いた表現に変換
	- Adobe-Japan1, Moji_Johoの表現への変換にも対応	
- 異体字がどのコレクションに含まれるかの判定
- 特定のコレクションで扱えない異体字セレクタが含まれるかの判定
- 異体字セレクタを考慮した文字列の比較・検索
- 異体字セレクタやサロゲートペアを考慮した文字の列挙
- .NET Framework向けにSystem.Text.Rune互換構造体を提供

詳細は[APIドキュメント](https://smynudop.github.io/Itaiji/index.html)をご覧ください。