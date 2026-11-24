using System.Text.RegularExpressions;

namespace MhguLookup.Core.Theme;

/// <summary>
/// The colours the app gives the game's elements, statuses, bow shots, coatings and ammo: the MHFU LookUp's palette
/// for every kind it had, plus colours for the kinds GU added (marked GU).
/// </summary>
public static partial class GameColors
{
    /// <summary>Element and status colours, keyed by the game's own names. Blast is GU's: the lime of its icon.</summary>
    public static IReadOnlyDictionary<string, string> Element { get; } = new Dictionary<string, string>
    {
        ["Fire"] = "#FF4D2E", ["Water"] = "#4A9EFF", ["Thunder"] = "#F5C400", ["Ice"] = "#7FD8F0", ["Dragon"] = "#B060E0",
        ["Poison"] = "#C040C8", ["Paralysis"] = "#D4960A", ["Sleep"] = "#B0CCE8",
        ["Blast"] = "#ADD56D",
    };

    public static string? ElementColor(string? element) => element is not null && Element.TryGetValue(element, out var c) ? c : null;

    [GeneratedRegex(@"^(\w+) Lv(\d)")]
    private static partial Regex ShotPattern();

    /// <summary>
    /// A Bow charge shot's colour: its type's hue, paler at Lv1 and stronger at Lv5. Heavy is GU's: purple.
    /// </summary>
    public static string? ShotColor(string shot)
    {
        var m = ShotPattern().Match(shot);
        if (!m.Success) return null;
        var lv = Math.Clamp(int.Parse(m.Groups[2].Value), 1, 5);
        var off = (int)Math.Round(200 + (88 - 200) * (lv - 1) / 4.0, MidpointRounding.AwayFromZero);
        (int R, int G, int B)? rgb = m.Groups[1].Value switch
        {
            "Rapid" => (off, off, 255),
            "Spread" => (off, 255, off),
            "Pierce" => (255, off, off),
            "Heavy" => (Math.Min(255, off + 60), off, 255),
            _ => null,
        };
        return rgb is { } c ? $"rgb({c.R},{c.G},{c.B})" : null;
    }

    private static readonly (Regex Pattern, string Color)[] Coatings =
    [
        (new("^Power"), "#FF5252"), (new("^Poison"), "#B060E0"), (new("^Para"), "#F5C400"), (new("^Sleep"), "#7FD8F0"),
        (new(@"^C\.Range"), "#FFFFFF"), (new("^Paint"), "#FF7FBF"),
        (new("^Elem"), "#FF8C42"),      // GU: orange
        (new("^Exhaust"), "#A3AEE0"),   // GU: slate blue
        (new("^Blast"), "#ADD56D"),     // GU: lime, the game's own Blast icon colour
    ];

    public static string? CoatingColor(string name) => Coatings.FirstOrDefault(c => c.Pattern.IsMatch(name)).Color;

    private static readonly (Regex Pattern, string Color)[] Ammos =
    [
        (new("^Normal"), "#6A9CFF"), (new("^Pellet"), "#66CC66"), (new("^Pierce"), "#FF6A6A"),
        (new("^Crag"), "#1FC8B4"), (new("^Clust"), "#7B68EE"),
        (new(@"^(P\.)?Flaming"), "#FF4D2E"), (new(@"^(P\.)?Water"), "#4A9EFF"), (new(@"^(P\.)?Thunder"), "#F5C400"),
        (new(@"^(P\.)?Freeze"), "#7FD8F0"), (new("^Dragon"), "#B060E0"),
        (new("^Poison"), "#B060E0"), (new("^Para"), "#F5C400"), (new("^Sleep"), "#7FD8F0"),
        (new("^(Group )?Recover"), "#5FB85F"), (new("^Paint"), "#FF7FBF"), (new("^Tranq"), "#FF5252"),
        (new("^Demon Armor|^Super Armor|^Armor"), "#FFA040"), (new("^Demon"), "#FF5252"),
        (new("^Exhaust"), "#A3AEE0"),   // GU: as Exhaust Coating
        (new("^Blast"), "#ADD56D"),     // GU: as Blast Coating
        (new("^Slicing"), "#E6B85C"),   // GU: gold
        (new("^Remedy"), "#5FB85F"),    // GU: a recovery shot, Recover's green
        (new("^Dung"), "#B08850"),      // GU: brown
        (new("^Flash|^Dazzling"), "#FFFFFF"),  // GU: white
    ];

    public static string? AmmoColor(string name) => Ammos.FirstOrDefault(c => c.Pattern.IsMatch(name)).Color;

    /// <summary>Sharpness colours in the table's order, red to purple.</summary>
    public static IReadOnlyList<string> SharpColors { get; } = ["#d22b2b", "#e8822a", "#e8d22a", "#5cc23a", "#3a7fe0", "#f2f2f2", "#b05ad8"];

    public static IReadOnlyList<string> SharpNames { get; } = ["Red", "Orange", "Yellow", "Green", "Blue", "White", "Purple"];

    /// <summary>
    /// The length of each colour in a sharpness gauge: the table row (where each colour ends), cut at <paramref name="length"/>.
    /// </summary>
    public static int[] SharpSegments(IReadOnlyList<int> row, int length)
    {
        var segs = new int[row.Count];
        var prev = 0;
        for (var i = 0; i < row.Count; i++)
        {
            var end = Math.Min(row[i], length);
            segs[i] = Math.Max(0, end - prev);
            prev = Math.Max(prev, end);
        }
        return segs;
    }

    private static readonly HashSet<string> ElementZones = ["fire", "water", "thunder", "ice", "dragon"];

    /// <summary>
    /// A hit-zone cell's background, by value tier. Raw and element values run on different ranges, so each has its own
    /// thresholds, set at the same places in its spread over every hit-zone row in the game. Stun and exhaust use raw's.
    /// </summary>
    public static string HitZoneBackground(int value, string zoneType)
    {
        var (green, orange, yellow) = ElementZones.Contains(zoneType) ? (25, 15, 10) : (66, 46, 21);
        if (value >= green) return "#2e5e2e";    // dark green: very weak
        if (value >= orange) return "#6b3d00";   // dark orange
        if (value >= yellow) return "#5a5200";   // dark yellow
        return "#383838";                        // dark grey: resistant
    }

    private static readonly Dictionary<string, (double R, double G, double B)> Extracts = new()
    {
        ["red"] = (0.78, 0.16, 0.16), ["orange"] = (0.90, 0.52, 0.13), ["white"] = (0.88, 0.88, 0.90), ["green"] = (0.30, 0.68, 0.30),
    };

    /// <summary>A Kinsect extract's cell colours (as the Monster Viewer paints them), or null for an unknown extract.</summary>
    public static (string Background, string Text)? ExtractColors(string? extract)
    {
        if (extract is null || !Extracts.TryGetValue(extract, out var c)) return null;
        var lum = 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B;
        int B(double x) => (int)Math.Round(x * 255, MidpointRounding.AwayFromZero);
        return ($"rgb({B(c.R)},{B(c.G)},{B(c.B)})", lum > 0.45 ? "#101216" : "#f2f4f8");
    }
}
