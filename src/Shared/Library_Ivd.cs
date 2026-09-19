using System.IO;

namespace Itaiji;

internal static partial class Library
{
#if NET35
    private static volatile IvdTable? _ivd;
    private static readonly object IvdLock = new();

    private static IvdTable GetIvd()
    {
        var table = _ivd;
        if (table != null)
        {
            return table;
        }

        lock (IvdLock)
        {
            return _ivd ??= LoadIvd();
        }
    }
#else
    private static readonly Lazy<IvdTable> Ivd = new(LoadIvd);

    private static IvdTable GetIvd() => Ivd.Value;
#endif

    internal static IvsCollectionType GetIvsCollectionType(int key)
    {
        var table = GetIvd();
        var index = Array.BinarySearch(table.Keys, (uint)key);
        return index >= 0 ? (IvsCollectionType)table.Values[index] : IvsCollectionType.Unknown;
    }

    private static IvdTable LoadIvd()
    {
        using var stream = typeof(Library).Assembly.GetManifestResourceStream("Itaiji.IVD_Sequences.bin")
            ?? throw new InvalidDataException("The embedded IVD data is missing.");
        using var buffer = new MemoryStream();
        var chunk = new byte[8192];
        int bytesRead;
        while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) != 0)
        {
            buffer.Write(chunk, 0, bytesRead);
        }
        var data = buffer.ToArray();

        if (data.Length < 8 || data[0] != 'I' || data[1] != 'V' || data[2] != 'D' || data[3] != '1')
        {
            throw new InvalidDataException("The embedded IVD data has an unsupported format.");
        }

        var count = (uint)(data[4] | data[5] << 8 | data[6] << 16 | data[7] << 24);
        if (count > int.MaxValue || data.Length != 8L + count * 5L)
        {
            throw new InvalidDataException("The embedded IVD data has an invalid length.");
        }

        var keys = new uint[(int)count];
        var values = new byte[(int)count];
        for (var i = 0; i < keys.Length; i++)
        {
            var offset = 8 + i * 5;
            var baseRune = (uint)(data[offset] | data[offset + 1] << 8 | data[offset + 2] << 16);
            var selector = data[offset + 3];
            if (baseRune > 0x10FFFF || selector > 0xEF)
            {
                throw new InvalidDataException("The embedded IVD data contains an invalid Unicode value.");
            }

            var packedKey = (baseRune << 8) | selector;
            if (i > 0 && packedKey <= keys[i - 1])
            {
                throw new InvalidDataException("The embedded IVD data is not sorted by unique keys.");
            }

            keys[i] = packedKey;
            values[i] = data[offset + 4];
        }

        return new IvdTable(keys, values);
    }

    private sealed class IvdTable(uint[] keys, byte[] values)
    {
        public uint[] Keys { get; } = keys;
        public byte[] Values { get; } = values;
    }
}
