using System.Diagnostics;
using System.Text;
#if NETSTANDARD2_0 || NETFRAMEWORK
using Itaiji.Text;
#endif

namespace Itaiji;

/// <summary>
/// 漢字1文字を表します。
/// </summary>
[DebuggerDisplay("{DebuggerString}")]
public struct KanjiChar : IEquatable<KanjiChar>
{
    /// <summary>
    /// ベースの文字
    /// </summary>
    public readonly Rune BaseRune;

    /// <summary>
    /// 異体字セレクター（存在しない場合はnull）。0xE0100〜0xE01EFの範囲。
    /// </summary>
    public readonly Rune? VariationSelector;

    /// <summary>
    /// 異体字の種類を表します
    /// </summary>
    public readonly IvsType IvsType;

    /// <summary>
    /// ベースの文字と異体字セレクターからKanjiCharを生成します。
    /// </summary>
    /// <param name="_base"></param>
    /// <param name="ivs"></param>
    public KanjiChar(Rune _base, Rune? ivs)
    {
        this.BaseRune = _base;
        this.VariationSelector = ivs;
        this.IvsType = JudgeIvsType();
    }

    /// <summary>
    /// ベースの文字から、異体字セレクターのないKanjiCharを生成します。
    /// </summary>
    /// <param name="_base"></param>
    public KanjiChar(Rune _base)
    {
        this.BaseRune = _base;
        this.VariationSelector = null;
        this.IvsType = JudgeIvsType();
    }

    /// <summary>
    /// ベースのcharから、異体字セレクターのないKanjiCharを生成します。
    /// </summary>
    /// <param name="_base"></param>
    public KanjiChar(char _base)
    {
        this.BaseRune = new Rune(_base);
        this.VariationSelector = null;
        this.IvsType = JudgeIvsType();
    }

    /// <summary>
    /// ベースのcharと異体字セレクタから、異体字セレクターのないKanjiCharを生成します。
    /// </summary>
    /// <param name="_base"></param>
    /// <param name="ivs"></param>
    public KanjiChar(char _base, byte ivs)
    {
        this.BaseRune = new Rune(_base);
        this.VariationSelector = new Rune(0xE0100 | ivs);
        this.IvsType = JudgeIvsType();
    }

    private IvsType JudgeIvsType()
    {
        if (VariationSelector == null)
        {
            return IvsType.None;
        }
        else if (Library.JpIvsList.TryGetValue(this.GetHashCode(), out var type))
        {
            return (IvsType)type;
        }
        else
        {
            return IvsType.Unknown;
        }
    }

    /// <summary>
    /// この文字が異体字セレクターを持つかどうか
    /// </summary>
    public readonly bool IsVariation { get => VariationSelector != null; }

    /// <summary>
    /// Utf32におけるシーケンス長を取得
    /// </summary>
    public readonly int Utf32SequenceLength
    {
        get => VariationSelector.HasValue ? 2 : 1;
    }

    /// <summary>
    /// Utf16におけるシーケンス長を取得
    /// </summary>
    public readonly int Utf16SequenceLength { 
        get => BaseRune.Utf16SequenceLength 
            + VariationSelector?.Utf16SequenceLength ?? 0; 
    }

    /// <summary>
    /// Utf16におけるシーケンス長を取得
    /// </summary>
    public readonly int Utf8SequenceLength { 
        get => BaseRune.Utf8SequenceLength 
            + VariationSelector?.Utf8SequenceLength ?? 0; 
    }

    /// <summary>
    /// Adobe-Japan1として有効な異体字セレクタを持つかどうか
    /// </summary>
    public readonly bool IsAdobeJapan => IvsType.HasBitFlag(IvsType.AdobeJapan);

    /// <summary>
    /// Hanyo-Denshiとして有効な異体字セレクタを持つかどうか
    /// </summary>
    public readonly bool IsHanyoDenshi => IvsType.HasBitFlag(IvsType.HanyoDenshi);

    /// <summary>
    /// Moji-Johoとして有効な異体字セレクタを持つかどうか
    /// </summary>
    public readonly bool IsMojiJoho => IvsType.HasBitFlag(IvsType.MojiJoho);

    /// <inheritdoc/>
    public override readonly bool Equals(object? other)
    {
        return other is KanjiChar otherKanji ? Equals(otherKanji) : false;
    }

    /// <inheritdoc/>
    public readonly bool Equals(KanjiChar other)
    {
        return BaseRune == other.BaseRune 
            && VariationSelector == other.VariationSelector;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(KanjiChar left, KanjiChar right)
    {
        return left.Equals(right);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(KanjiChar left, KanjiChar right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        var hash1 = BaseRune.GetHashCode();
        var hash2 = VariationSelector?.GetHashCode() ?? 0;
        return (hash1 << 8) | (hash2 & 0xFF);
    }

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        return BaseRune.ToString() + VariationSelector?.ToString() ?? "";
    }

    private readonly string DebuggerString
    {
        get
        {
            if (VariationSelector is null)
            {
                return $"{BaseRune} U+{BaseRune.Value:X}";
            }
            else
            {
                return $"{BaseRune}{VariationSelector} U+{BaseRune.Value:X} U+{VariationSelector.Value.Value:X}";
            }
        }
    }
}
