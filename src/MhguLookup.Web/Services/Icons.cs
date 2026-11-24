using MhguLookup.Core.Models;

namespace MhguLookup.Web.Services;

/// <summary>
/// The game's icons for items, monsters and equipment. The three small icon maps are loaded once, while the app
/// starts, so every lookup after that is synchronous and a component can show an icon on its first render.
/// </summary>
public sealed class Icons(LookupData data)
{
    private ItemIconMap _items = new(new Dictionary<string, string>(), new Dictionary<string, string>());
    private EquipIconMap _equip = new(new Dictionary<string, int>(), new Dictionary<string, int>(), new Dictionary<string, int>());
    private IReadOnlyDictionary<string, string> _monsters = new Dictionary<string, string>();

    /// <summary>Loads the icon maps. If one fails, its icons are just left out.</summary>
    public async Task LoadAsync()
    {
        var items = data.ItemIcons();
        var equip = data.EquipIcons();
        var monsters = data.MonsterIcons();
        try { _items = await items; } catch (Exception ex) { Console.Error.WriteLine(ex); }
        try { _equip = await equip; } catch (Exception ex) { Console.Error.WriteLine(ex); }
        try { _monsters = await monsters; } catch (Exception ex) { Console.Error.WriteLine(ex); }
    }

    public string? ItemById(int id) => File(_items.ById.GetValueOrDefault(id.ToString()));

    public string? ItemByName(string name) => File(_items.ByName.GetValueOrDefault(name));

    /// <summary>An item's icon, by id when known, otherwise by name.</summary>
    public string? Item(string name, int? id = null) => (id is { } i ? ItemById(i) : null) ?? ItemByName(name);

    public string? Monster(string? id) =>
        id is not null && _monsters.TryGetValue(id, out var f) ? $"icons/monster/{f}.png" : null;

    /// <param name="cls">The weapon class, w00 .. w14.</param>
    public string? Weapon(string cls, int rarity) => Equip(_equip.Weapon, cls, rarity);

    /// <param name="slot">Head, Chest, Arms, Waist or Legs.</param>
    public string? Armor(string slot, int rarity) => Equip(_equip.Armor, slot, rarity);

    /// <param name="kind">cutting, blunt, head or body.</param>
    public string? Palico(string kind, int rarity) => Equip(_equip.Palico, kind, rarity);

    private static string? File(string? key) => key is null ? null : $"icons/item/{key}.png";

    // The shape comes from the equipment kind's cell on the icon sheet, the colour from the rarity.
    private static string? Equip(IReadOnlyDictionary<string, int> cells, string kind, int rarity) =>
        cells.TryGetValue(kind, out var cell) && rarity is >= 1 and <= 11 ? $"icons/equip/{cell}-{rarity}.png" : null;
}
