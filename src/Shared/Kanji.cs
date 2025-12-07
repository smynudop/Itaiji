using System.Diagnostics;
using Itaiji.Extensions;
#if NETSTANDARD2_0 || NETFRAMEWORK
using Itaiji.Text;
#endif

namespace Itaiji;

/// <summary>
/// 漢字1文字を表します。
/// </summary>
[DebuggerDisplay("{DebuggerString}")]
public struct KanjiChar : IEquatable<KanjiChar>, IComparable<KanjiChar>
{
    /// <summary>
    /// ベースのRune
    /// </summary>
    /// <remarks>
    /// 文字列に異体字セレクタが単独で含まれていた場合や、連続した異体字セレクタが含まれていた場合などは、
    /// BaseRuneが異体字セレクタとなるケースも存在します。
    /// </remarks> 
    public readonly Rune BaseRune => _BaseRune;

    private readonly Rune _BaseRune;

    /// <summary>
    /// 異体字セレクター。存在しない場合はnull
    /// </summary>
    /// <remarks>0xE0100～0xE01EFの範囲</remarks>
    public readonly Rune? VariationSelector
    {
        get => _VariationSelector.Value != 0 ? _VariationSelector : null;
    }

    internal readonly Rune NonNullVariationSelector
    {
        get => _VariationSelector;
    }

    private readonly Rune _VariationSelector;

    /// <summary>
    /// ベースのRuneから、異体字セレクターのないKanjiCharを生成します。
    /// </summary>
    /// <param name="_base"></param>
    public KanjiChar(Rune _base) : this(_base, default) { }

    /// <summary>
    /// ベースのRuneと異体字セレクターのRuneからKanjiCharを生成します。
    /// </summary>
    /// <param name="_base"></param>
    /// <param name="ivs"></param>
    /// <exception cref="ArgumentException">異体字セレクタが無効だった場合</exception>"
    public KanjiChar(Rune _base, Rune ivs)
    {
        if (!ivs.IsIVS() && ivs != default)
        {
            throw new ArgumentException("異体字セレクターの範囲外の値です。", nameof(ivs));
        }
        this._BaseRune = _base;
        this._VariationSelector = ivs;
    }

    /// <summary>
    /// ベースとなる文字と異体字セレクターからKanjiCharを生成します。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="ivs"></param>
    /// <exception cref="ArgumentException">ベースとなる文字が1文字ではなかった場合、または異体字セレクタが無効だった場合</exception>
    public KanjiChar(string str, Rune ivs)
    {
        if (!ivs.IsIVS() && ivs != default)
        {
            throw new ArgumentException("異体字セレクターの範囲外の値です。", nameof(ivs));
        }
        var runes = str.EnumerateRunes();
        if (!runes.MoveNext())
        {
            throw new ArgumentException("ベース文字がありません。", nameof(str));
        }
        this._BaseRune = runes.Current;
        if (runes.MoveNext())
        {
            throw new ArgumentException("ベース文字が複数あります。", nameof(str));
        }
        this._VariationSelector = ivs;
    }

    /// <summary>
    /// ベースとなる文字と異体字セレクターからKanjiCharを生成します。
    /// </summary>
    /// <param name="str"></param>
    /// <exception cref="ArgumentException">ベースとなる文字が表せない場合</exception>
    public KanjiChar(string str)
    {
        var runes = str.EnumerateRunes();
        if (!runes.MoveNext())
        {
            throw new ArgumentException("ベース文字がありません。", nameof(str));
        }
        this._BaseRune = runes.Current;
        if (runes.MoveNext())
        {
            if (runes.Current.IsIVS())
            {
                this._VariationSelector = runes.Current;
                if (runes.MoveNext())
                {
                    throw new ArgumentException("ベース文字が複数あります。", nameof(str));
                }
            }
            else
            {
                throw new ArgumentException("ベース文字が無効です。", nameof(str));
            }
        }
        else
        {
            this._VariationSelector = default;
        }
    }

    /// <summary>
    /// ベースのcharから、異体字セレクターのないKanjiCharを生成します。
    /// </summary>
    /// <param name="_base"></param>
    public KanjiChar(char _base)
    {
        this._BaseRune = new Rune(_base);
        this._VariationSelector = default;
    }

    /// <summary>
    /// ベースのcharと異体字セレクタから、異体字セレクターのないKanjiCharを生成します。
    /// </summary>
    /// <param name="_base"></param>
    /// <param name="ivs"></param>
    internal KanjiChar(char _base, byte ivs)
    {
        this._BaseRune = new Rune(_base);
        this._VariationSelector = new Rune(0xE0100 | ivs);
    }

    /// <summary>
    /// 異体字セレクタが属するIVSコレクションの種類を取得します。
    /// ライブラリが対応していない異体字の場合は<see cref="IvsCollectionType.Unknown"/>を返します。
    /// </summary>
    public IvsCollectionType GetIvsCollectionType()
    {
        if (_VariationSelector == default)
        {
            return IvsCollectionType.None;
        }
        else if (Library.JpIvsList.TryGetValue(this.GetHashCode(), out var type))
        {
            return (IvsCollectionType)type;
        }
        else
        {
            return IvsCollectionType.Unknown;
        }
    }

    /// <summary>
    /// この文字が異体字セレクターを持つかどうかを取得します。
    /// </summary>
    public readonly bool IsVariation { get => _VariationSelector != default; }

    /// <summary>
    /// Utf32におけるシーケンス長を取得します。
    /// </summary>
    /// <remarks>デフォルト(U+0000)の場合、値は1となります。</remarks>
    public readonly int Utf32SequenceLength
    {
        get => _VariationSelector != default ? 2 : 1;
    }

    /// <summary>
    /// Utf16におけるシーケンス長を取得します。char換算の長さと等価です。
    /// </summary>
    /// <remarks>デフォルト(U+0000)の場合、値は1となります。</remarks>
    public readonly int Utf16SequenceLength
    {
        get => _BaseRune.Utf16SequenceLength
            + (_VariationSelector != default ? _VariationSelector.Utf16SequenceLength : 0);
    }

    /// <summary>
    /// Utf8におけるシーケンス長を取得します。
    /// </summary>
    /// <remarks>デフォルト(U+0000)の場合、値は1となります。</remarks>
    public readonly int Utf8SequenceLength
    {
        get => _BaseRune.Utf8SequenceLength
            + (_VariationSelector != default ? _VariationSelector.Utf8SequenceLength : 0);
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object? other)
    {
        return other is KanjiChar otherKanji && Equals(otherKanji);
    }

    /// <inheritdoc/>
    public readonly bool Equals(KanjiChar other)
    {
        return _BaseRune == other.BaseRune
            && _VariationSelector == other.NonNullVariationSelector;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator <(KanjiChar left, KanjiChar right) => left.CompareTo(right) < 0;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator <=(KanjiChar left, KanjiChar right) => left.CompareTo(right) <= 0;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator >(KanjiChar left, KanjiChar right) => left.CompareTo(right) > 0;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator >=(KanjiChar left, KanjiChar right) => left.CompareTo(right) >= 0;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public readonly int CompareTo(KanjiChar other)
    {
        var baseCompare = _BaseRune.CompareTo(other.BaseRune);
        if (baseCompare != 0)
        {
            return baseCompare;
        }
        return _VariationSelector.CompareTo(other.NonNullVariationSelector);
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return (_BaseRune.Value << 8) | (_VariationSelector.Value & 0xFF); // defaultの場合は0なので問題なし
    }

    /// <inheritdoc/>
    public override readonly string ToString()
    {
#if NETFRAMEWORK
        var buffer = new char[4];
        var len = _BaseRune.EncodeToUtf16(buffer);
        if (_VariationSelector != default)
        {
            len += _VariationSelector.EncodeToUtf16(buffer, len);
        }
        return new string(buffer, 0, len);
#else
        Span<char> buffer = stackalloc char[4];
        var len = _BaseRune.EncodeToUtf16(buffer);
        if (_VariationSelector != default)
        {
            len += _VariationSelector.EncodeToUtf16(buffer.Slice(len));
        }
        return new string(buffer.Slice(0, len));
#endif
    }

    private readonly string DebuggerString
    {
        get
        {
            if (_VariationSelector == default)
            {
                return $"{_BaseRune} U+{_BaseRune.Value:X}";
            }
            else
            {
                return $"{_BaseRune}{_VariationSelector} U+{_BaseRune.Value:X} U+{_VariationSelector.Value:X}";
            }
        }
    }
}
