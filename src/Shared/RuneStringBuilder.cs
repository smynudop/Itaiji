namespace Itaiji;

#if NET5_0_OR_GREATER

using System.Buffers;

internal ref struct RuneStringBuilder
{
    private char[] _array;
    private Span<char> _buffer;
    private int pos = 0;

    public RuneStringBuilder() : this(8)
    {
    }

    public RuneStringBuilder(int capacity)
    {
        capacity = Math.Max(capacity, 8);
        _array = ArrayPool<char>.Shared.Rent(capacity);
        _buffer = _array.AsSpan(0, capacity);
    }

    public void Add(Rune rune)
    {
        if (pos + rune.Utf16SequenceLength > _buffer.Length)
        {
            Grow();
        }
        pos += rune.EncodeToUtf16(_buffer.Slice(pos));
    }

    public void Add(KanjiChar kanji)
    {
        Add(kanji.BaseRune);
        if(kanji.IsVariation)
        {
            Add(kanji.VariationSelector!.Value);
        }
    }

    private void Grow()
    {
        var newCapacity = Math.Max(_buffer.Length * 2, 8);
        var newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        _buffer.Slice(0,pos).CopyTo(newArray.AsSpan());
        ArrayPool<char>.Shared.Return(_array);
        _array = newArray;
        _buffer = _array.AsSpan(0, newCapacity);
    }

    public override string ToString()
    {
        if(_array == null)
        {
            throw new ObjectDisposedException(nameof(RuneStringBuilder));
        }
        return new string(_buffer.Slice(0, pos));
    }

    public void Dispose()
    {
        if(_array == null)
        {
            return;
        }
        ArrayPool<char>.Shared.Return(_array);
        _array = null!;
        _buffer = Span<char>.Empty;
        pos = 0;
    }
}
#else

using Itaiji.Text;  

internal ref struct RuneStringBuilder
{
    private char[] _array;
    private int pos = 0;

    public RuneStringBuilder() : this(8)
    {
    }

    public RuneStringBuilder(int capacity)
    {
        capacity = Math.Max(capacity, 8);
        _array = new char[capacity];
    }

    public void Add(Rune rune)
    {
        if (pos + rune.Utf16SequenceLength > _array.Length)
        {
            Grow();
        }
        pos += rune.EncodeToUtf16(_array, pos);
    }

    public void Add(KanjiChar kanji)
    {
        Add(kanji.BaseRune);
        if(kanji.IsVariation)
        {
            Add(kanji.VariationSelector!.Value);
        }
    }

    private void Grow()
    {
        var newCapacity = Math.Max(_array.Length * 2, 8);
        var newArray = new char[newCapacity];
        _array.CopyTo(newArray, 0);
        _array = newArray;
    }

    public override string ToString()
    {
        if(_array == null)
        {
            throw new ObjectDisposedException(nameof(RuneStringBuilder));
        }
        return new string(_array, 0, pos);
    }

    public void Dispose()
    {
        if(_array == null)
        {
            return;
        }
        _array = null!;
        pos = 0;
    }
}
#endif