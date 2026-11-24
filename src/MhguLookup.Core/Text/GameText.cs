using System.Text.RegularExpressions;

namespace MhguLookup.Core.Text;

/// <summary>A run of the game's text: either a line break or some text, optionally coloured.</summary>
/// <param name="Color">A <c>#RRGGBB</c> colour, or null for the default text colour.</param>
public sealed record GameTextPart(string Text, string? Color, bool IsBreak)
{
    public static GameTextPart Break { get; } = new("", null, true);

    /// <summary>The game's plain red, which the UI swaps for a red that reads well on a dark background.</summary>
    public bool IsDefaultHighlight => Color == "#FF0000";
}

public sealed record GameTextResult(bool Centred, IReadOnlyList<GameTextPart> Parts);

/// <summary>
/// Parses the game's own message markup: <c>&lt;COL RRGGBBAA&gt;…&lt;/COL&gt;</c> colours a run and
/// <c>&lt;CNTR&gt;</c> centres the text. Any other tag is dropped.
/// The game breaks lines to fit its text boxes; those breaks are joined back into running text
/// (a word hyphenated at a break keeps its hyphen), except in centred text and where the game
/// leaves a blank line. A "(Supply Item)" or "(Account Item)" tag always gets a line of its own.
/// </summary>
public static partial class GameText
{
    [GeneratedRegex(@"\s*(<COL [0-9A-Fa-f]{8}>\((?:Supply|Account) Item\)</COL>)")]
    private static partial Regex SupplyTag();

    [GeneratedRegex(@"\n{2,}")]
    private static partial Regex BlankLines();

    [GeneratedRegex(@"(<COL [0-9A-Fa-f]{8}>|</COL>|<[^>]*>|\n)")]
    private static partial Regex Tokens();

    [GeneratedRegex(@"^<COL ([0-9A-Fa-f]{6})[0-9A-Fa-f]{2}>$")]
    private static partial Regex ColourTag();

    // Marks where a Supply/Account tag goes onto its own line, so the line joining below leaves it alone.
    private static readonly string KeepBreak = ((char)0x2028).ToString();

    public static GameTextResult Parse(string? text)
    {
        if (string.IsNullOrEmpty(text)) return new GameTextResult(false, []);

        var centred = text.Contains("<CNTR>");
        var marked = SupplyTag().Replace(text, KeepBreak + "$1");
        var flowed = centred
            ? marked
            : string.Join("\n\n", BlankLines().Split(marked).Select(p => p.Replace("-\n", "-").Replace('\n', ' ')));
        flowed = flowed.Replace(KeepBreak, "\n").Replace("<CNTR>", "");

        var parts = new List<GameTextPart>();
        string? colour = null;
        foreach (var part in Tokens().Split(flowed))
        {
            if (part.Length == 0) continue;
            var col = ColourTag().Match(part);
            if (col.Success) { colour = "#" + col.Groups[1].Value.ToUpperInvariant(); continue; }
            if (part == "</COL>") { colour = null; continue; }
            if (part == "\n") { parts.Add(GameTextPart.Break); continue; }
            if (part.StartsWith('<')) continue;
            parts.Add(new GameTextPart(part, colour, false));
        }
        return new GameTextResult(centred, parts);
    }
}
