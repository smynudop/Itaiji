// See https://aka.ms/new-console-template for more information
using Kanji.Generator;
using System.Text;
Console.OutputEncoding = System.Text.Encoding.UTF8;

var text = File.ReadAllText("IVD_Sequences.txt");

var dic = new Dictionary<(Rune, Rune), IvsType>(); 
foreach (var line in text.Split(["\n"], StringSplitOptions.None))
{
    if (line.StartsWith("#"))
    {
        continue; 
    }

    if(line.Trim() == "")
    {
        continue;
    }

    var cols = line.Split(';');
    var runes = cols[0].Split(' ');

    var firstRune = new Rune(Convert.ToInt32(runes[0], 16));
    var secondRune = new Rune(Convert.ToInt32(runes[1], 16));

    var t = (firstRune, secondRune);

    var type = cols[1].Trim() switch
    {
        "Adobe-Japan1" => IvsType.AdobeJapan,
        "Hanyo-Denshi" => IvsType.HanyoDenshi,
        "Moji_Joho" => IvsType.MojiJoho,
        _ => IvsType.Other
    };
    if(type == IvsType.Other)
    {
        Console.WriteLine(cols[1]);
        continue;
    }

    if(dic.ContainsKey(t) == false)
    {
        dic[t] = IvsType.None;
    }
    dic[t] |= type;
}

var builder = new StringBuilder();
builder.AppendLine("""
    namespace moji;

    public static class Library
    {
        public static readonly SortedList<int, int> JpIvsList = new ()
        {
    """);

var cnt = 0;
foreach(var g in dic.GroupBy(x => x.Key.Item1))
{
    var g1 = g.Select(x => x.Value).Distinct();
    if(g1.Contains(IvsType.HanyoDenshi) && g1.Contains(IvsType.MojiJoho) && g1.Contains(IvsType.HDandMJ))
    {
        Console.WriteLine($"複数IVS種別: U+{g.Key.Value:X}{g.Key}");
    }
}
foreach (var kvp in dic.OrderBy(x => x.Key.Item1).ThenBy(x => x.Key.Item2))
{
    if(cnt == 0)
    {
        builder.Append("        ");
    }
    var key = (kvp.Key.Item1.Value << 8) | (kvp.Key.Item2.Value & 0xFF);
    var value = (int)kvp.Value;
    builder.Append($"{{0x{key:X},{value}}},");
    cnt ++;
    if(cnt == 10)
    {
        builder.AppendLine("");
        cnt = 0;
    }
}
builder.AppendLine("""

        };
    }
    """);

File.WriteAllText("Library.cs", builder.ToString());
