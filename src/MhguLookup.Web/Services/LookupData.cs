using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using MhguLookup.Core;
using MhguLookup.Core.Models;

namespace MhguLookup.Web.Services;

/// <summary>
/// Loads the game data from <c>wwwroot/data/*.json</c>. Each file is fetched once per session and shared,
/// so switching tabs never downloads it again.
/// </summary>
public sealed class LookupData(HttpClient http)
{
    private static LookupJsonContext Ctx => LookupJsonContext.Default;

    private readonly ConcurrentDictionary<string, Task<object>> _cache = new();

    public Task<SkillData> Skills() => Load("skills.json", Ctx.SkillData);
    public Task<List<Style>> Styles() => Load("styles.json", Ctx.ListStyle);
    public Task<List<HunterArt>> Arts() => Load("arts.json", Ctx.ListHunterArt);
    public Task<WeaponIndex> WeaponIndex() => Load("weapons.json", Ctx.WeaponIndex);
    public Task<WeaponClass> WeaponClass(string id) => Load($"weapons/{id}.json", Ctx.WeaponClass);
    public Task<HornSongs> HornSongs() => Load("horn-songs.json", Ctx.HornSongs);
    public Task<List<ArmorSet>> Armor() => Load("armor.json", Ctx.ListArmorSet);
    public Task<List<Decoration>> Decorations() => Load("decorations.json", Ctx.ListDecoration);
    public Task<MonsterData> Monsters() => Load("monsters.json", Ctx.MonsterData);
    public Task<List<Quest>> Quests() => Load("quests.json", Ctx.ListQuest);
    public Task<List<GatherMap>> Gathering() => Load("gathering.json", Ctx.ListGatherMap);
    public Task<Dictionary<string, List<ItemSource>>> Sources() => Load("sources.json", Ctx.DictionaryStringListItemSource);
    public Task<List<Combo>> Combos() => Load("combos.json", Ctx.ListCombo);
    public Task<Canteen> Canteen() => Load("canteen.json", Ctx.Canteen);
    public Task<List<Item>> Items() => Load("items.json", Ctx.ListItem);
    public Task<Palico> Palico() => Load("palico.json", Ctx.Palico);
    public Task<ItemIconMap> ItemIcons() => Load("item-icons.json", Ctx.ItemIconMap);
    public Task<EquipIconMap> EquipIcons() => Load("equip-icons.json", Ctx.EquipIconMap);
    public Task<Dictionary<string, string>> MonsterIcons() => Load("monster-icons.json", Ctx.DictionaryStringString);

    private async Task<T> Load<T>(string file, JsonTypeInfo<T> type) where T : class
    {
        var task = _cache.GetOrAdd(file, async f =>
            await http.GetFromJsonAsync($"data/{f}", type)
                ?? throw new InvalidDataException($"data/{f} is empty."));
        try
        {
            return (T)await task;
        }
        catch
        {
            _cache.TryRemove(file, out _);   // let a later visit retry a failed download
            throw;
        }
    }
}
