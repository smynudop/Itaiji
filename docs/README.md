# 開発ドキュメント

Itaijiライブラリの開発に関するドキュメントです。

## ドキュメント一覧

- [リリース手順](release.md) - バージョン更新とパッケージング手順

## フォルダ構成

```
Itaiji/
├── docs/              # 開発用ドキュメント（このフォルダ）
├── docfx/             # 公開用APIドキュメント
├── nupkg/             # 生成されたNuGetパッケージ
├── src/
│   ├── Itaiji/                      # .NET 5.0以降用ライブラリ
│   ├── Itaiji.NetFramework/         # .NET Framework用ライブラリ
│   ├── Itaiji.Test/                 # .NET用テスト
│   ├── Itaiji.NetFramework.Test/    # .NET Framework用テスト
│   ├── Itaiji.NetFramework35.Test/  # .NET Framework 3.5用テスト
│   ├── Itaiji.Generator/            # データ生成ツール
│   ├── Shared/                      # 共通コード
│   └── Shared.Test/                 # 共通テストコード
├── LICENSE.txt
├── Readme.md
└── THIRD-PARTY-NOTICES.txt
```

## 開発環境

### 必須

- .NET SDK 5.0以降
- .NET Framework 3.5, 4.0, 4.5, 4.6, 4.7, 4.8（.NET Frameworkパッケージのビルドに必要）

### 推奨

- Visual Studio 2022以降 または Visual Studio Code
- Git

## ビルド

### すべてのプロジェクトをビルド

```powershell
dotnet build
```

### リリースビルド

```powershell
dotnet build -c Release
```

## テスト

### すべてのテストを実行

```powershell
dotnet test
```

### 特定のプロジェクトのテストを実行

```powershell
dotnet test src\Itaiji.Test\Itaiji.Test.csproj
```

## パッケージング

詳細は[リリース手順](release.md)を参照してください。

## コーディング規約

プロジェクトルートの`<custom_instruction>`セクションを参照してください。主なポイント：

- C# 14を使用
- Nullable Reference Typesを有効化
- XML コメント・ドキュメンテーションを重視
- 日本語でドキュメントを記述
- 高パフォーマンスのため`Span<T>`/`Memory<T>`を積極的に使用
- .NET Frameworkでは`Span<T>`が使えないため、条件付きコンパイルで切り替え

## IVS (Ideographic Variation Sequence)

本ライブラリは以下のIVSコレクションに対応しています：

- Adobe-Japan1
- Hanyo-Denshi
- Moji_Joho（文字情報基盤）
- CJK互換漢字
- Standardized Variants (SVS)
- その他のIVDコレクション

## 用語

- **IVS**: Ideographic Variation Sequence（異体字セレクタ）
- **SVS**: Standardized Variants（標準化異体字）
- **CI**: Compatibility Ideographs（CJK互換漢字）
- **VS**: Variation Selector（異体字セレクタ）
