using System.Text.Json.Serialization;

namespace MhguLookup.Core.Models;

// canteen.json and palico.json

/// <summary>canteen.json</summary>
/// <param name="Unlocks">Per ingredient (same order as <see cref="Ingredients"/>), the ways to unlock it.</param>
/// <param name="Menus">Per menu number, the sauces and the three skills each gives.</param>
/// <param name="Random">The Food Skills draw list.</param>
/// <param name="Alcohol">The Food Skills draw list when the sauce has alcohol.</param>
/// <param name="Odds">The chance each of a sauce's three skills activates.</param>
public sealed record Canteen(
    IReadOnlyList<string> Ingredients,
    IReadOnlyList<IReadOnlyList<IngredientUnlock>> Unlocks,
    IReadOnlyList<Dish> Dishes,
    IReadOnlyDictionary<string, IReadOnlyList<Sauce>> Menus,
    IReadOnlyList<FoodSkill> Random,
    IReadOnlyList<FoodSkill> Alcohol,
    IReadOnlyList<SkillOdds> Odds,
    IReadOnlyList<string> Sauces)
{
    /// <summary>Every skill the two Food Skills draw lists hold, once each, in table order.</summary>
    public IEnumerable<FoodSkill> FoodSkills => Random.Concat(Alcohol).DistinctBy(s => s.Id);
}

/// <param name="Ingredients">The two ingredients, or null for a dish on the menu from the start.</param>
/// <param name="Hunter">The meal's effects on the hunter, by stat, in the game's order.</param>
/// <param name="Palico">The meal's effects on the Palico.</param>
public sealed record Dish(
    string Name,
    IReadOnlyList<string>? Ingredients,
    int Level,
    IReadOnlyDictionary<string, int> Hunter,
    IReadOnlyDictionary<string, int> Palico,
    int Menu);

public sealed record Sauce([property: JsonPropertyName("sauce")] string Name, IReadOnlyList<string> Skills);

public sealed record FoodSkill(int Id, string Skill, string Desc, string Brief);

/// <param name="Normal">Percent.</param>
/// <param name="Fired">The chance when the chef is fired up, as text.</param>
public sealed record SkillOdds(int Normal, string Fired, int Gourmet);

/// <summary>One way to unlock a Canteen ingredient. The JSON's <c>kind</c> picks the type.</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(StartIngredient), "start")]
[JsonDerivedType(typeof(QuestIngredient), "quest")]
[JsonDerivedType(typeof(RequestIngredient), "request")]
[JsonDerivedType(typeof(TalkIngredient), "talk")]
public abstract record IngredientUnlock;

public sealed record StartIngredient : IngredientUnlock;

/// <summary>Clear a quest, which opens it either by progress or by talking to someone after.</summary>
public sealed record QuestIngredient : IngredientUnlock
{
    public string Via { get; init; } = "";
    public int Quest { get; init; }
    public string Title { get; init; } = "";
    public string Category { get; init; } = "";
    public string Stars { get; init; } = "";
}

/// <summary>Fulfill a villager's request.</summary>
public sealed record RequestIngredient : IngredientUnlock
{
    public string Npc { get; init; } = "";
    public IReadOnlyList<RequestItem> Items { get; init; } = [];
}

public sealed record RequestItem(int Id, string Name, int Qty);

public sealed record TalkIngredient : IngredientUnlock
{
    public string Npc { get; init; } = "";
}

/// <summary>palico.json. Every label is the game's own text.</summary>
public sealed record Palico(
    IReadOnlyList<PalicoWeapon> Weapons,
    IReadOnlyList<PalicoArmor> Head,
    IReadOnlyList<PalicoArmor> Body,
    IReadOnlyList<PalicoSkill> Skills,
    IReadOnlyList<PalicoMove> Moves,
    IReadOnlyList<PalicoBias> Biases,
    PalicoLabels Labels)
{
    /// <summary>"RARE n", in the game's words.</summary>
    public string Rare(int n) => Labels.Rare.Replace("%d", n.ToString());
}

public sealed record PalicoAttack(int Attack, int Affinity, int Element);

/// <param name="AtkTree">0 for a cutting weapon, otherwise blunt.</param>
/// <param name="Style">The weapon type in the game's markup (Balanced, Close Range, Boomerang).</param>
/// <param name="SpType">0 Balanced, 1 Close Range, 2 Boomerang.</param>
/// <param name="Sharpness">An index into the sharpness colours.</param>
public sealed record PalicoWeapon(
    int Id, string Name, string Desc, int Rarity,
    string AtkType, int AtkTree, string Style, int SpType, int Sharpness,
    PalicoAttack Melee, PalicoAttack Ranged, string? ElementType, int Defense, IReadOnlyList<Amount> Materials);

/// <param name="Res">Fire, water, thunder, ice and dragon resistance.</param>
public sealed record PalicoArmor(int Id, string Name, string Desc, int Rarity, int Defense, IReadOnlyList<int> Res, IReadOnlyList<Amount> Materials);

public sealed record PalicoSkill(int Id, string Name, string Desc, int Memory, string Condition);

/// <param name="Kind">The move's type, as a number in the game's order of the types.</param>
/// <param name="Type">The move's type, in the game's words.</param>
public sealed record PalicoMove(int Id, int Sort, string Name, string Desc, int Gauge, int Kind, string Type, string Condition);

public sealed record PalicoBias(int Id, string Name, string Desc);

/// <summary>The game's own words for the Palico screens' labels.</summary>
public sealed record PalicoLabels(
    string AtkType, string Melee, string Ranged, string Memory, string Conditions, string Gauge, string Type,
    string Head, string Body, string Skills, string Moves, string Equipment, string Attack, string Element,
    string Defense, string Affinity, string Rare, IReadOnlyList<string> Res, string Materials, string Weapon,
    string Bias, string Sharpness, string Handicraft);
