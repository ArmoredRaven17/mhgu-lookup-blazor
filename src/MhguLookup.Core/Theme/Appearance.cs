using System.Globalization;
using System.Text.RegularExpressions;

namespace MhguLookup.Core.Theme;

/// <summary>A surface colour theme: it retints the window, panels and borders. The accent is separate.</summary>
public sealed record ColorPreset(string Key, string Name, string Window, string Base, string Panel, string Border, string Swatch);

/// <summary>The app's colour themes and the CSS variables each one sets. Ported from the desktop app's Appearance.cs.</summary>
public static partial class Appearance
{
    public const string DefaultSurface = "silver";
    public const string DefaultAccent = "#C04040";

    public static IReadOnlyList<ColorPreset> Presets { get; } =
    [
        new("silver", "Silver", "#1E1E1F", "#272728", "#232324", "#444546", "#BFC3C7"),
        new("sand", "Sand", "#1F1D19", "#282621", "#24221E", "#464237", "#C2B48C"),
        new("fog", "Fog", "#1D1E1F", "#262728", "#222324", "#434446", "#A8AEB4"),
        new("taupe", "Taupe", "#1F1D1B", "#282523", "#242220", "#46413C", "#9A8C7C"),
        new("steel", "Steel", "#1A1E22", "#22272C", "#20242A", "#3A434C", "#6E8090"),
        new("mocha", "Mocha", "#221C18", "#2D251F", "#28211C", "#4B3F36", "#8C7361"),
        new("slate", "Slate", "#1B1E24", "#232830", "#21262E", "#3A4250", "#5A6B80"),
        new("charcoal", "Charcoal", "#161616", "#1E1E1E", "#1B1B1B", "#333333", "#5A5A5A"),
        new("red", "Red", "#241616", "#2C1C1C", "#2A1919", "#4A3030", "#C04040"),
        new("scarlet", "Scarlet", "#221513", "#2D1B18", "#281816", "#4B302C", "#E0402A"),
        new("salmon", "Salmon", "#221A18", "#2D221F", "#281E1C", "#4B3B36", "#E8917A"),
        new("coral", "Coral", "#261812", "#2E1F18", "#2B1C16", "#4C3A2E", "#E07050"),
        new("rust", "Rust", "#221814", "#2D201A", "#281C17", "#4B372E", "#B0542C"),
        new("brown", "Brown", "#211913", "#292019", "#261D16", "#463829", "#9C6B40"),
        new("orange", "Orange", "#241C12", "#2D2318", "#2A2016", "#4A3A24", "#C0772E"),
        new("amber", "Amber", "#241E10", "#2D2616", "#2A2414", "#4C4024", "#D8A028"),
        new("gold", "Gold", "#24200E", "#2E2913", "#2A2512", "#4E4320", "#E6B41E"),
        new("yellow", "Yellow", "#23220E", "#2C2B14", "#292813", "#4A4824", "#ECD92E"),
        new("butter", "Butter", "#211F14", "#2A2719", "#262319", "#47432E", "#ECDD8E"),
        new("chartreuse", "Chartreuse", "#202214", "#2A2D1A", "#252817", "#474B2F", "#C2D838"),
        new("olive", "Olive", "#1E2112", "#262A18", "#232717", "#404826", "#8FA83C"),
        new("lime", "Lime", "#1C2210", "#232C16", "#202915", "#3E4A24", "#9FCC30"),
        new("sage", "Sage", "#1C2218", "#242D1F", "#20281C", "#3D4B36", "#88A878"),
        new("green", "Green", "#15211A", "#1C2A22", "#1A271F", "#2E4438", "#3FA85A"),
        new("mint", "Mint", "#122520", "#182E28", "#162A24", "#2C4A40", "#44C88A"),
        new("teal", "Teal", "#122321", "#192C2A", "#172927", "#2C4744", "#2FBFB0"),
        new("cyan", "Cyan", "#10232A", "#172C34", "#152930", "#2A4750", "#2EB8C8"),
        new("sky", "Sky", "#14202C", "#1B2935", "#192530", "#2E4055", "#4AA8E0"),
        new("blue", "Blue", "#151B2A", "#1C2335", "#1A2030", "#2E3A55", "#4A7FE0"),
        new("navy", "Navy", "#121726", "#182038", "#161D32", "#28345A", "#3A5FC0"),
        new("cobalt", "Cobalt", "#141722", "#1A1E2D", "#171B28", "#2F354B", "#3A5CDC"),
        new("periwinkle", "Periwinkle", "#181A22", "#1F212D", "#1C1D28", "#363A4B", "#8494EC"),
        new("indigo", "Indigo", "#1A1630", "#221C3C", "#1E1936", "#3A3160", "#6A5ACD"),
        new("lavender", "Lavender", "#201A30", "#28203C", "#251D36", "#423A60", "#9A86E0"),
        new("purple", "Purple", "#221630", "#2A1C3C", "#271936", "#443060", "#9050C0"),
        new("magenta", "Magenta", "#261430", "#2E1A3C", "#2B1736", "#4A3060", "#B040C0"),
        new("plum", "Plum", "#24142A", "#2C1A34", "#29172F", "#463058", "#A03C90"),
        new("fuchsia", "Fuchsia", "#22151F", "#2D1B28", "#281823", "#4B3044", "#D040A8"),
        new("raspberry", "Raspberry", "#22131A", "#2D1922", "#28161E", "#4B2D3B", "#C42C70"),
        new("rose", "Rose", "#261620", "#2E1C28", "#2B1926", "#4E3045", "#E06A9A"),
        new("ruby", "Ruby", "#221117", "#2D171E", "#28141A", "#4B2934", "#C81850"),
        new("maroon", "Maroon", "#28121A", "#301820", "#2D151D", "#4E2C3A", "#9C2848"),
        new("crimson", "Crimson", "#260F15", "#2E151C", "#2B1218", "#50303C", "#C03052"),
    ];

    [GeneratedRegex("^#[0-9a-fA-F]{6}$")]
    private static partial Regex HexColour();

    /// <summary>A valid <c>#RRGGBB</c> colour, or the default accent.</summary>
    public static string ClampHex(string? hex)
    {
        var h = hex?.Trim() ?? "";
        return HexColour().IsMatch(h) ? h : DefaultAccent;
    }

    public static (int R, int G, int B) ToRgb(string hex)
    {
        var h = ClampHex(hex);
        int At(int i) => int.Parse(h.AsSpan(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return (At(1), At(3), At(5));
    }

    public static string Darken(string hex, double factor)
    {
        var (r, g, b) = ToRgb(hex);
        string C(int n) => ((int)Math.Round(n * factor, MidpointRounding.AwayFromZero)).ToString("x2");
        return $"#{C(r)}{C(g)}{C(b)}";
    }

    private static string RgbList(string hex) { var (r, g, b) = ToRgb(hex); return $"{r}, {g}, {b}"; }

    /// <summary>The CSS variables a surface theme sets. An unknown key falls back to the default theme.</summary>
    public static IReadOnlyDictionary<string, string> SurfaceVariables(string? key)
    {
        var p = Presets.FirstOrDefault(c => c.Key == key) ?? Presets.First(c => c.Key == DefaultSurface);
        return new Dictionary<string, string>
        {
            ["--bg"] = p.Window, ["--bg-rgb"] = RgbList(p.Window),
            ["--surface"] = p.Base, ["--surface-rgb"] = RgbList(p.Base),
            ["--panel"] = p.Panel, ["--panel-rgb"] = RgbList(p.Panel),
            ["--border"] = p.Border,
        };
    }

    /// <summary>The CSS variables the accent colour drives.</summary>
    public static IReadOnlyDictionary<string, string> AccentVariables(string? hex)
    {
        var h = ClampHex(hex);
        var (r, g, b) = ToRgb(h);
        return new Dictionary<string, string>
        {
            ["--accent"] = h,
            ["--accent-dim"] = Darken(h, 0.75),
            ["--header-bg"] = $"rgba({r}, {g}, {b}, 0.12)",
        };
    }
}
