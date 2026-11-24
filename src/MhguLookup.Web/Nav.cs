namespace MhguLookup.Web;

/// <summary>A tab in the top bar.</summary>
/// <param name="Ported">
/// Whether the tab has been ported from the React app yet. An unported tab shows a page that links to
/// the same tab in the live React app.
/// </param>
public sealed record NavItem(string Path, string Label, bool Ported = false);

public static class Nav
{
    /// <summary>The live React version of the app, which every unported tab links to.</summary>
    public const string ReactAppUrl = "https://armoredraven17.github.io/MHGU-LookUp/";

    /// <summary>The tab the app opens on.</summary>
    public const string Home = "armorskills";

    /// <summary>The tabs, in the React app's order.</summary>
    public static IReadOnlyList<NavItem> Items { get; } =
    [
        new("bookmarks", "Bookmarks"),
        new("notes", "Notes"),
        new("monsters", "Monsters"),
        new("weapons", "Weapons"),
        new("armor", "Armor"),
        new("armorskills", "Armor Skills", Ported: true),
        new("decorations", "Decorations"),
        new("styles", "Hunting Styles"),
        new("arts", "Hunter Arts"),
        new("quests", "Quests"),
        new("gathering", "Gathering"),
        new("items", "Items"),
        new("combos", "Combo List"),
        new("canteen", "Canteen"),
        new("palicogear", "Palico Equipment"),
        new("palicoskills", "Palico Skills & Moves"),
        new("settings", "Settings"),
        new("help", "Help"),
        new("about", "About"),
    ];
}
