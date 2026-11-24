using System.Text.RegularExpressions;
using MhguLookup.Core.Models;

namespace MhguLookup.Core.Lists;

/// <summary>The listing orders and groupings that several tabs share or that need more than a sort.</summary>
public static partial class Grouping
{
    /// <summary>The quest categories, in the Quests tab's order.</summary>
    public static IReadOnlyList<string> QuestCategories { get; } = ["Village", "Hub", "Special Permit", "Arena", "Event", "Training"];

    /// <summary>The ranks, in order, for the categories listed by rank.</summary>
    public static IReadOnlyList<string> QuestRanks { get; } = ["Village Low Rank", "Low Rank", "High Rank", "G Rank"];

    /// <summary>A Special Permit quest's Deviant group: its number is 40XXYY, XX the Deviant and YY the permit level.</summary>
    public static int PermitGroup(Quest q) => q.Id / 100 % 100;

    /// <summary>
    /// Each Special Permit group's Deviant: the monster most of its quests target. The quest board records no group,
    /// and some levels add another monster, so it is decided by vote.
    /// </summary>
    public static IReadOnlyDictionary<int, MonsterRef> PermitMonsters(IEnumerable<Quest> quests)
    {
        var votes = new Dictionary<int, Dictionary<string, int>>();
        var names = new Dictionary<string, string>();
        foreach (var q in quests)
        {
            if (q.Category != "Special Permit" || q.Target?.Monster is not { } m) continue;
            var g = PermitGroup(q);
            var v = votes.TryGetValue(g, out var existing) ? existing : votes[g] = [];
            v[m] = v.GetValueOrDefault(m) + 1;
            if (q.Monsters.FirstOrDefault(x => x.Id == m)?.Name is { } n) names[m] = n;
        }
        return votes.ToDictionary(
            kv => kv.Key,
            kv =>
            {
                // the first monster with the most votes, as a stable sort would give
                var best = kv.Value.Aggregate((a, b) => b.Value > a.Value ? b : a).Key;
                return new MonsterRef(best, names.GetValueOrDefault(best, ""));
            });
    }

    /// <summary>The quests a monster is a boss in, in the Quests tab's order: category, stars, then the board's order.</summary>
    public static IReadOnlyList<Quest> QuestsWith(IEnumerable<Quest> quests, string monsterId) =>
        quests.Where(q => q.Monsters.Any(m => m.Id == monsterId))
            .OrderBy(q => IndexOf(QuestCategories, q.Category)).ThenBy(q => q.Level).ThenBy(q => q.Order)
            .ToList();

    [GeneratedRegex(" (I|II|III)$")]
    private static partial Regex TierSuffix();

    /// <summary>A Hunter Art's name without its tier: "Absolute Evasion II" → "Absolute Evasion".</summary>
    public static string ArtBaseName(string name) => TierSuffix().Replace(name, "");

    /// <summary>
    /// The weapon order of the Guild Card's weapon usage: GS, LS, SnS, DB, Hammer, HH, Lance, GL, SA, CB, IG, LBG, HBG, Bow.
    /// </summary>
    public static IReadOnlyList<string> GuildCardOrder { get; } =
        ["w00", "w07", "w01", "w11", "w02", "w12", "w03", "w09", "w08", "w14", "w13", "w06", "w04", "w10"];

    /// <summary>Each weapon's colour on the Guild Card, as the MHGU Save Editor shows it.</summary>
    public static IReadOnlyDictionary<string, string> GuildCardColors { get; } = new Dictionary<string, string>
    {
        ["w00"] = "#ff505b", ["w07"] = "#9beaf1", ["w01"] = "#dfd65f", ["w11"] = "#6ac083", ["w02"] = "#c3a3d2",
        ["w12"] = "#f89a64", ["w03"] = "#9fbcff", ["w09"] = "#f4baf5", ["w08"] = "#aaaaaa", ["w14"] = "#fc5800",
        ["w13"] = "#f5f5f5", ["w06"] = "#acd56b", ["w04"] = "#f8899c", ["w10"] = "#55edc4",
    };

    /// <summary>An Art group: one Art and its tiers, keyed by the first tier's id.</summary>
    public sealed record ArtGroup(string Key, string Name, IReadOnlyList<HunterArt> Tiers)
    {
        public string? Weapon => Tiers[0].Weapon;
        public int Category => Tiers[0].Category;
    }

    /// <summary>
    /// The Hunter Arts with each Art's tiers together, generic Arts first, then by weapon in the Guild Card's order,
    /// each weapon's Arts in the game's own order.
    /// </summary>
    public static IReadOnlyList<ArtGroup> ArtGroups(IEnumerable<HunterArt> arts) =>
        arts.GroupBy(a => ArtBaseName(a.Name))
            .Select(g => new ArtGroup(g.First().Id.ToString(), g.Key, g.ToList()))
            .OrderBy(g => g.Weapon is null ? -1 : IndexOf(GuildCardOrder, g.Weapon))
            .ThenBy(g => g.Category)
            .ToList();

    /// <summary>The index in the list, or -1 (as JavaScript's indexOf, so unknown values sort first).</summary>
    public static int IndexOf(IReadOnlyList<string> list, string value)
    {
        for (var i = 0; i < list.Count; i++) if (list[i] == value) return i;
        return -1;
    }
}
