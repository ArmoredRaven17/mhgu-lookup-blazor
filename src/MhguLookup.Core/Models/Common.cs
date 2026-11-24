using System.Text.Json.Serialization;
using MhguLookup.Core.Json;

namespace MhguLookup.Core.Models;

// The data files' small positional rows (JSON arrays), each read by its converter in Json/Converters.cs.

/// <summary>A name and a count: a skill's points, a material and its quantity, an ammo and its capacity.</summary>
[JsonConverter(typeof(AmountConverter))]
public sealed record Amount(string Name, int Qty);

/// <summary>A material and its quantity, or, when <see cref="IsPoints"/>, a category and the material points it costs.</summary>
[JsonConverter(typeof(MaterialConverter))]
public sealed record Material(string Name, int Qty, bool IsPoints);

/// <summary>An item, its quantity, and its chance in percent (a gathering pull or a quest reward slot).</summary>
[JsonConverter(typeof(ChanceItemConverter))]
public sealed record ChanceItem(string Name, int Qty, int Percent);

/// <summary>One row of a monster's carve, capture or break list.</summary>
[JsonConverter(typeof(RewardRowConverter))]
public sealed record RewardRow(int ItemId, string Name, int Qty, int Percent);

/// <summary>A bowgun's internal ammo: the shots carried in total and per clip.</summary>
[JsonConverter(typeof(InternalAmmoConverter))]
public sealed record InternalAmmo(string Ammo, int Total, int Clip);

/// <summary>A Light Bowgun's rapid fire: shots per fire, power of each shot in percent, and the wait.</summary>
[JsonConverter(typeof(RapidFireConverter))]
public sealed record RapidFire(string Ammo, int Shots, int Power, string Wait);

/// <summary>A status's threshold on a monster in a quest: initial, the increase after each proc, and the maximum.</summary>
[JsonConverter(typeof(StatusStatConverter))]
public sealed record StatusStat(string Status, double Initial, double Step, double Max);

/// <summary>One way to get an item: the kind of source, where, the detail (a carve, a quest slot), and the chance.</summary>
[JsonConverter(typeof(ItemSourceConverter))]
public sealed record ItemSource(string Kind, string Where, string Detail, int? Percent);

/// <summary>A boss monster in a quest, with its stats there.</summary>
[JsonConverter(typeof(QuestMonsterConverter))]
public sealed record QuestMonster(string Id, string Name, QuestStats? Stats);

/// <summary>A reference to a quest, as an unlock condition gives it.</summary>
public sealed record QuestRef(int Quest, string Title, string Category, string Stars);

public static class QuestText
{
    /// <summary>
    /// How a quest is introduced wherever it is referenced: "Hub 3★: Title", and a Special Permit quest just
    /// "Special Permit: Title".
    /// </summary>
    public static string Prefix(string category, string stars) =>
        category == "Special Permit" ? category : $"{category} {stars}";
}
