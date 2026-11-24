using System.Globalization;

namespace MhguLookup.Core.Text;

/// <summary>Number formatting shared by the pages, matching the React app's (en-US) output.</summary>
public static class Format
{
    /// <summary>1500 → "1,500"; 28.5 → "28.5".</summary>
    public static string N(double n) => n.ToString("#,0.###", CultureInfo.InvariantCulture);

    /// <summary>A plain number with no grouping: 0.7 → "0.7", 1.0 → "1".</summary>
    public static string Plain(double n) => n.ToString(CultureInfo.InvariantCulture);

    /// <summary>A skill or resistance value with its sign: +3, -2, 0.</summary>
    public static string Signed(int n) => n > 0 ? $"+{n}" : n.ToString(CultureInfo.InvariantCulture);

    /// <summary>The CSS class for a value's sign: positive, negative, or none for zero.</summary>
    public static string? SignClass(int n) => n > 0 ? "text-pos" : n < 0 ? "text-neg" : null;

    /// <summary>"an" before a vowel, else "a".</summary>
    public static string Article(string name) => name.Length > 0 && "AEIOUaeiou".Contains(name[0]) ? "an" : "a";

    /// <summary>Strips the game's markup tags, e.g. &lt;COL …&gt;.</summary>
    public static string PlainText(string s) => System.Text.RegularExpressions.Regex.Replace(s, "<[^>]*>", "");

    /// <summary>Joins the game's wrapped lines into running text (a word hyphenated at a break keeps its hyphen).</summary>
    public static string Flat(string s) => s.Replace("-\n", "-").Replace('\n', ' ');
}
