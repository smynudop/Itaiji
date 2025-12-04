namespace Itaiji;

internal static class SpanRuneHelper
{
    internal static int ReadFirstRuneFromUtf16Buffer(ReadOnlySpan<char> input, out int length)
    {

        if (input.IsEmpty)
        {
            length = 0;
            return -1;
        }

        // Optimistically assume input is within BMP.
        length = 1;
        uint returnValue = input[0];

        if (char.IsSurrogate((char)returnValue))
        {
            if (!char.IsHighSurrogate((char)returnValue))
            {
                return -1;
            }

            // Treat 'returnValue' as the high surrogate.

            if (input.Length <= 1)
            {
                return -1; // not an argument exception - just a "bad data" failure
            }

            char potentialLowSurrogate = input[1];
            if (!char.IsLowSurrogate(potentialLowSurrogate))
            {
                return -1;
            }
            length = 2;
            returnValue = (uint)char.ConvertToUtf32((char)returnValue, (char)potentialLowSurrogate);
        }

        return (int)returnValue;
    }
}
