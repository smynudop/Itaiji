# 導入方法

nugetからインストールしてください。


そのあと、以下のusingディレクティブを追加してください。
```csharp
using Itaiji;
```
```vbnet
Imports Itaiji
```

## Rune構造体(.NET Framework向け)

.NET FrameworkではSystem.Text.Runeが利用できないため、Itaiji.NetFrameworkパッケージには同等の機能を持つRune構造体が含まれています。
Itaiji.Text名前空間に含まれており、以下のusingディレクティブを追加して使用してください。

```csharp
using Itaiji.Text;
```

```vbnet
Imports Itaiji.Text
```