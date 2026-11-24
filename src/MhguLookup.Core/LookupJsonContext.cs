using System.Text.Json;
using System.Text.Json.Serialization;
using MhguLookup.Core.Models;

namespace MhguLookup.Core;

/// <summary>
/// Source-generated System.Text.Json metadata for every type read from <c>data/*.json</c> or localStorage.
/// Source generation keeps deserialization working after the WebAssembly publish trims unused code,
/// and skips runtime reflection, which is slow under WASM.
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    AllowOutOfOrderMetadataProperties = true)]
[JsonSerializable(typeof(SkillData))]
[JsonSerializable(typeof(List<Style>))]
[JsonSerializable(typeof(List<HunterArt>))]
[JsonSerializable(typeof(WeaponIndex))]
[JsonSerializable(typeof(WeaponClass))]
[JsonSerializable(typeof(HornSongs))]
[JsonSerializable(typeof(List<ArmorSet>))]
[JsonSerializable(typeof(List<Decoration>))]
[JsonSerializable(typeof(MonsterData))]
[JsonSerializable(typeof(List<Quest>))]
[JsonSerializable(typeof(QuestStats))]
[JsonSerializable(typeof(List<GatherMap>))]
[JsonSerializable(typeof(Dictionary<string, List<ItemSource>>))]
[JsonSerializable(typeof(List<Combo>))]
[JsonSerializable(typeof(Canteen))]
[JsonSerializable(typeof(List<Item>))]
[JsonSerializable(typeof(Palico))]
[JsonSerializable(typeof(ItemIconMap))]
[JsonSerializable(typeof(EquipIconMap))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(List<Bookmark>))]
[JsonSerializable(typeof(List<NoteRecord>))]
[JsonSerializable(typeof(Dictionary<string, int>))]
public sealed partial class LookupJsonContext : JsonSerializerContext;
