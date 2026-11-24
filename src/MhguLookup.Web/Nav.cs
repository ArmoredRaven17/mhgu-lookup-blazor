namespace MhguLookup.Web;

/// <summary>A tab in the top bar.</summary>
public sealed record NavItem(string Path, string Label);

public static class Nav
{
    /// <summary>The original React version of the app.</summary>
    public const string ReactAppUrl = "https://armoredraven17.github.io/MHGU-LookUp/";

    /// <summary>The tab the app opens on, and the one an unknown address goes to.</summary>
    public const string Home = "weapons";

    /// <summary>The tabs, in the React app's order.</summary>
    public static IReadOnlyList<NavItem> Items { get; } =
    [
        new("bookmarks", "Bookmarks"),
        new("notes", "Notes"),
        new("monsters", "Monsters"),
        new("weapons", "Weapons"),
        new("armor", "Armor"),
        new("armorskills", "Armor Skills"),
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

    /// <summary>The kinds of page a bookmark or note can be on, in the order Bookmarks and Notes list them.</summary>
    public static IReadOnlyList<(string Type, string Header)> EntitySections { get; } =
    [
        ("monster", "Monsters"),
        ("weapon", "Weapons"),
        ("armor", "Armor"),
        ("armorskill", "Armor Skills"),
        ("decoration", "Decorations"),
        ("art", "Hunter Arts"),
        ("quest", "Quests"),
        ("gathering", "Gathering"),
        ("dish", "Canteen Dishes"),
        ("item", "Items"),
    ];
}
