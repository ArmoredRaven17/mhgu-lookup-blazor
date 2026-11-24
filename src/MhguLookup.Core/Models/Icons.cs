namespace MhguLookup.Core.Models;

/// <summary>
/// item-icons.json: an item's icon file, by item id and by item name. Each icon is the game's icon sheet shape
/// tinted by the item's colour, as <c>icons/item/&lt;file&gt;.png</c>.
/// </summary>
public sealed record ItemIconMap(IReadOnlyDictionary<string, string> ById, IReadOnlyDictionary<string, string> ByName);

/// <summary>equip-icons.json: the icon sheet cell for each kind of equipment; the colour comes from the rarity.</summary>
public sealed record EquipIconMap(
    IReadOnlyDictionary<string, int> Armor,
    IReadOnlyDictionary<string, int> Weapon,
    IReadOnlyDictionary<string, int> Palico);
