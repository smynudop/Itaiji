# リリース手順

このドキュメントでは、Itaijiライブラリの新バージョンをリリースする手順を説明します。

## 1. バージョン番号の更新

以下の2つのプロジェクトファイルでバージョン番号を更新します。

### 更新対象ファイル

- `src/Itaiji/Itaiji.csproj`
- `src/Itaiji.NetFramework/Itaiji.NetFramework.csproj`

### 更新箇所

各ファイルで以下の3つのプロパティを更新します：

```xml
<VersionPrefix>0.2.0</VersionPrefix>
<AssemblyVersion>0.2.0.0</AssemblyVersion>
<FileVersion>0.2.0.0</FileVersion>
```

バージョン番号は[セマンティックバージョニング](https://semver.org/lang/ja/)に従います：
- **Major** (X.0.0): 互換性のない変更
- **Minor** (0.X.0): 後方互換性のある機能追加
- **Patch** (0.0.X): 後方互換性のあるバグ修正

## 2. 変更のコミット

```powershell
git add src/Itaiji/Itaiji.csproj src/Itaiji.NetFramework/Itaiji.NetFramework.csproj
git commit -m "Bump version to X.Y.Z

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
```

## 3. タグの作成（推奨）

```powershell
git tag -a v0.2.0 -m "Release version 0.2.0"
```

## 4. GitHubへのプッシュ

```powershell
git push origin main
git push origin --tags
```

## 5. NuGetパッケージのビルド

### Itaijiパッケージ（.NET 5.0+）

```powershell
dotnet pack src\Itaiji\Itaiji.csproj -c Release -o .\nupkg
```

### Itaiji.NetFrameworkパッケージ（.NET Framework 3.5～4.8）

```powershell
dotnet pack src\Itaiji.NetFramework\Itaiji.NetFramework.csproj -c Release -o .\nupkg
```

ビルド完了後、`nupkg`フォルダに以下のファイルが生成されます：
- `Itaiji.X.Y.Z.nupkg`
- `Itaiji.NetFramework.X.Y.Z.nupkg`

## 6. パッケージの検証

生成されたパッケージを確認します：

```powershell
Get-ChildItem .\nupkg\*.nupkg | Select-Object Name, Length, LastWriteTime
```

## 7. NuGet.orgへの公開

### 方法1: dotnet CLIを使用

```powershell
dotnet nuget push .\nupkg\Itaiji.X.Y.Z.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkg\Itaiji.NetFramework.X.Y.Z.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### 方法2: NuGet.orgのWebインターフェース

1. https://www.nuget.org/ にログイン
2. "Upload Package"をクリック
3. 生成された`.nupkg`ファイルをアップロード
4. 両方のパッケージをアップロード

## トラブルシューティング

### ビルド時の警告について

以下の警告は既知の問題で、パッケージの機能には影響しません：

```
warning CS8618: null 非許容の フィールド '_CIDictionary' には、コンストラクターの終了時に null 以外の値が入っていなければなりません。
```

この警告は`Library_CompabilityIdeographs.cs`から発生していますが、実行時には正しく初期化されます。

## チェックリスト

リリース前に以下を確認してください：

- [ ] バージョン番号が両方の.csprojファイルで更新されている
- [ ] 変更がコミットされている
- [ ] GitHubにプッシュされている
- [ ] タグが作成されている（推奨）
- [ ] 両方のNuGetパッケージがビルドされている
- [ ] パッケージのサイズが妥当である
- [ ] テストが通っている
- [ ] NuGet.orgへの公開が完了している
