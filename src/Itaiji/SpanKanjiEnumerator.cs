// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using Itaiji.Extensions;
namespace Itaiji;

/// <summary>
/// 漢字を表す列挙子を提供します。
/// </summary>
public ref struct SpanKanjiEnumerator : IEnumerator<KanjiChar>
{
    private ReadOnlySpan<char> _remaining;
    private KanjiChar _current;
    private Rune _prev;

    internal SpanKanjiEnumerator(ReadOnlySpan<char> buffer)
    {
        _remaining = buffer;
        _current = default;
    }

    /// <inheritdoc/>
    public KanjiChar Current => _current;

    /// <inheritdoc/>
    public SpanKanjiEnumerator GetEnumerator() => this;

    private bool GetNextRune(out Rune value)
    {
        if (_remaining.IsEmpty)
        {
            // reached the end of the buffer
            value = default;
            return false;
        }

        int scalarValue = SpanRuneHelper.ReadFirstRuneFromUtf16Buffer(_remaining, out var length);
        if (scalarValue < 0)
        {
            // replace invalid sequences with U+FFFD
            scalarValue = Rune.ReplacementChar.Value;
        }

        value = new Rune(scalarValue);
        _remaining = _remaining.Slice(length);
        return true;
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        if (_prev == default)
        {
            // First rune
            if (!GetNextRune(out _prev))
            {
                // No rune
                _current = default;
                return false;
            }
        }

        if (!GetNextRune(out var next))
        {
            // No more rune
            _current = new KanjiChar(_prev);
            _prev = default;
            return true;
        }
        if (next.IsVariationSelector() && !_prev.IsVariationSelector())
        {
            _current = new KanjiChar(_prev, next);
            _prev = default;
            return true;
        }
        else
        {
            _current = new KanjiChar(_prev);
            _prev = next;
            return true;
        }
    }

    /// <inheritdoc />
    object IEnumerator.Current => Current;

    /// <inheritdoc />
    void IEnumerator.Reset() => throw new NotSupportedException();

    /// <inheritdoc />
    void IDisposable.Dispose() { }
}