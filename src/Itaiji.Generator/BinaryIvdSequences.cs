using Itaiji;

namespace Itaiji.Generator;

internal static class BinaryIvdSequences
{
    // IVD1, followed by a little-endian UInt32 record count.
    // Each sorted record: 3-byte little-endian base rune, 1-byte IVS index,
    // 1-byte IvsCollectionType flags. IVS index 0 means U+E0100.
    public static void Execute()
    {
        var entries = IvdSequences.ReadEntries()
            .OrderBy(entry => entry.Key.Item1.Value)
            .ThenBy(entry => entry.Key.Item2.Value)
            .ToArray();

        using var stream = File.Create("IVD_Sequences.bin");
        using var writer = new BinaryWriter(stream);
        writer.Write(new byte[] { (byte)'I', (byte)'V', (byte)'D', (byte)'1' });
        writer.Write((uint)entries.Length);

        foreach (var entry in entries)
        {
            var baseRune = entry.Key.Item1.Value;
            var ivsIndex = entry.Key.Item2.Value - 0xE0100;
            var flags = (int)entry.Value;
            if (baseRune > 0x10FFFF || ivsIndex is < 0 or > 0xEF || flags is < 0 or > byte.MaxValue)
            {
                throw new InvalidDataException("IVD entry cannot be represented by the binary format.");
            }

            writer.Write((byte)baseRune);
            writer.Write((byte)(baseRune >> 8));
            writer.Write((byte)(baseRune >> 16));
            writer.Write((byte)ivsIndex);
            writer.Write((byte)flags);
        }
    }
}
