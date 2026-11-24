namespace MhguLookup.Core.Models;

// monsters.json, quests.json, gathering.json, items.json, combos.json, styles.json

/// <summary>monsters.json</summary>
/// <param name="ZoneTypes">The damage types a hit-zone row has, in display order.</param>
/// <param name="Classes">The monster classes, in listing order.</param>
public sealed record MonsterData(IReadOnlyList<string> ZoneTypes, IReadOnlyList<string> Classes, IReadOnlyList<Monster> Monsters)
{
    public static MonsterData Empty { get; } = new([], [], []);
}

/// <param name="Quests">[rank, how many quests have it at that rank]</param>
/// <param name="ListClass">Listed under this class's heading instead of its own.</param>
/// <param name="Rewards">Per rank (Low Rank, High Rank, G Rank).</param>
public sealed record Monster(
    string Id,
    string Name,
    bool Small,
    string Class,
    IReadOnlyList<Amount> Quests,
    string? ListClass,
    HitZones? Hz,
    IReadOnlyDictionary<string, MonsterRewards>? Rewards);

/// <summary>A monster's hit zones: each zone's damage row, then its parts.</summary>
public sealed record HitZones(IReadOnlyList<HitZone> Zones, IReadOnlyList<MonsterPart> Parts);

/// <param name="Row">The damage taken, keyed by zone type (cut, impact, fire, …).</param>
public sealed record HitZone(string Name, IReadOnlyDictionary<string, int> Row);

/// <param name="Extract">The Kinsect extract colour: red, orange, white or green.</param>
public sealed record MonsterPart(string Name, int Stagger, int? Break, string? Extract);

/// <summary>Every carve and drop list the game has for a monster at one rank.</summary>
public sealed record MonsterRewards(
    IReadOnlyList<RewardList>? Carve,
    IReadOnlyList<RewardList>? Tail,
    IReadOnlyList<RewardList>? Mine,
    IReadOnlyList<RewardList>? Capture,
    IReadOnlyList<RewardList>? Shiny,
    IReadOnlyList<RewardList>? Gather,
    IReadOnlyList<BreakList>? Break);

/// <param name="Band">A Deviant's level band.</param>
/// <param name="Slot">The slot of a shiny drop or gathered list (some monsters have several).</param>
/// <param name="Levels">The Special Permit levels that use this band.</param>
public sealed record RewardList(int? Band, int? Slot, string? Levels, IReadOnlyList<RewardRow> Rows);

/// <param name="Name">The parts the break comes from, as the Parts table names them.</param>
public sealed record BreakList(int N, int Band, string? Levels, string? Name, IReadOnlyList<RewardRow> Rows);

/// <summary>A boss's stats in a quest: HP with the range the quest's difficulty rolls, and the multipliers.</summary>
public sealed record QuestStats(int Hp, int HpMin, int HpMax, double Atk, double Def, double Stagger, IReadOnlyList<StatusStat> Status);

/// <summary>A quest from quests.json.</summary>
/// <param name="Others">The quest text's other monsters.</param>
/// <param name="Category">Village, Hub, Arena, Event, Special Permit or Training.</param>
/// <param name="Stars">"3★", "G2★", "Permit Lv 4".</param>
/// <param name="Order">The quest boards' order.</param>
/// <param name="Time">Minutes.</param>
public sealed record Quest(
    int Id,
    string Title,
    string Client,
    string Desc,
    string Others,
    string Objective,
    QuestTarget? Target,
    string Failure,
    string Subquest,
    string Category,
    string Stars,
    int Level,
    string Rank,
    bool Key,
    int Order,
    string Locale,
    int Time,
    int Fee,
    int Reward,
    int Hrp,
    int SubReward,
    int SubHrp,
    IReadOnlyList<string> Requirements,
    IReadOnlyList<QuestMonster> Monsters);

/// <summary>A quest's main target: a monster, or an item to deliver.</summary>
public sealed record QuestTarget(string? Monster, int? Item);

/// <summary>A locale's gathering points, by rank and area.</summary>
public sealed record GatherMap(int Map, string Name, IReadOnlyList<GatherRank> Ranks);

public sealed record GatherRank(string Rank, IReadOnlyList<GatherArea> Areas);

public sealed record GatherArea(int Area, string Name, IReadOnlyList<GatherPoint> Points);

/// <summary>
/// A gathering point. A fixed point has <see cref="Items"/>; a random point rolls one of its <see cref="Outcomes"/>;
/// a fishing spot has a list per bait.
/// </summary>
/// <param name="Kind">Gathering, Ore Mining, Insect Catching or Fishing.</param>
public sealed record GatherPoint(
    string Kind,
    IReadOnlyList<ChanceItem>? Items,
    bool Random,
    IReadOnlyList<GatherOutcome>? Outcomes,
    IReadOnlyList<GatherBait>? Baits)
{
    /// <summary>Every item the point can give.</summary>
    public IEnumerable<ChanceItem> AllItems =>
        Items ?? Outcomes?.SelectMany(o => o.Items) ?? Baits?.SelectMany(b => b.Items) ?? [];
}

public sealed record GatherOutcome(int Chance, IReadOnlyList<ChanceItem> Items);

public sealed record GatherBait(string Bait, IReadOnlyList<ChanceItem> Items);

/// <param name="Buy">Only for an item the shop sells.</param>
public sealed record Item(int Id, string Name, string Desc, int Carry, int? Buy, int Sell);

/// <param name="Ids">The item ids of A, B and the result.</param>
public sealed record Combo(string A, string B, string Result, int Rate, int[] Ids);

/// <summary>A Hunting Style: the style select's summary and description, and its SP State.</summary>
public sealed record Style(string Key, string Name, string Short, string Full, string Sp);
