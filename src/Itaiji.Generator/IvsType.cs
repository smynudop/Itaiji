namespace Kanji.Generator;


[Flags]
public enum IvsType : int
{
    None = 0,
    AdobeJapan = 1 << 0,
    HanyoDenshi = 1 << 1,
    MojiJoho = 1 << 2,
    Other = 1 << 3,

    HDandMJ = HanyoDenshi | MojiJoho,
}
