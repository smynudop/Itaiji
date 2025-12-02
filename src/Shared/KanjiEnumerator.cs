using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#if NETSTANDARD2_0 || NETFRAMEWORK
using Itaiji.Text;
#endif

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Itaiji;

public struct KanjiEnumerator : IEnumerable<KanjiChar>, IEnumerator<KanjiChar>
{
    private readonly string _string;
    private KanjiChar _current;
    private Rune _prev;
    private int _nextIndex;

    public KanjiEnumerator(string str)
    {
        _current = default;
        _string = str;
        _prev = default;
    }

    private bool GetNextRune(out Rune rune)
    {
        if ((uint)_nextIndex >= _string.Length)
        {
            // reached the end of the string
            rune = default;
            return false;
        }

        if (!Rune.TryGetRuneAt(_string, _nextIndex, out rune))
        {
            // replace invalid sequences with U+FFFD
            rune = Rune.ReplacementChar;
        }

        // In UTF-16 specifically, invalid sequences always have length 1, which is the same
        // length as the replacement character U+FFFD. This means that we can always bump the
        // next index by the current scalar's UTF-16 sequence length. This optimization is not
        // generally applicable; for example, enumerating scalars from UTF-8 cannot utilize
        // this same trick.

        _nextIndex += rune.Utf16SequenceLength;
        return true;
    }

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
        if (next.IsIVS() && !_prev.IsIVS())
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

    public KanjiEnumerator GetEnumerator() => this;

    object? IEnumerator.Current => _current;
    public KanjiChar Current => _current;

    IEnumerator IEnumerable.GetEnumerator() => this;

    IEnumerator<KanjiChar> IEnumerable<KanjiChar>.GetEnumerator() => this;

    public void Reset()
    {
        _current = default;
        _prev = default;
        _nextIndex = 0;
    }

    // Make Dispose public so the compiler can call it for pattern-based foreach on the struct enumerator
    public void Dispose()
    {
        // no-op
    }
}
