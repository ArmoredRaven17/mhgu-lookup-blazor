using System.Text.Json.Serialization;

namespace MhguLookup.Core.Models;

// weapons.json, weapons/wNN.json, armor.json, decorations.json, horn-songs.json

/// <summary>weapons.json: the weapon classes, and the sharpness table every weapon's gauge is a row of.</summary>
/// <param name="Sharpness">Per row, where each of the 7 colours (red .. purple) ends.</param>
public sealed record WeaponIndex(IReadOnlyList<WeaponClassInfo> Classes, IReadOnlyList<int[]> Sharpness);

public sealed record WeaponClassInfo(string Id, string Name, int Count);

/// <summary>weapons/wNN.json: one weapon class, its weapons in upgrade-tree order.</summary>
public sealed record WeaponClass(string Id, string Name, IReadOnlyList<Weapon> Weapons);

public sealed record Weapon(
    int Row,
    string Name,
    IReadOnlyList<WeaponForm> Forms,
    int Rarity,
    int Depth,
    string? Element,
    string? Element2,
    string? Phial,
    string? Shelling,
    IReadOnlyList<ForgeRecipe>? Create,
    TreeLink? From,
    IReadOnlyList<TreeLink> Into,
    IReadOnlyList<WeaponLevel> Levels);

/// <summary>A name the weapon takes from level <see cref="From"/> on, with the game's text for it.</summary>
public sealed record WeaponForm(string Name, string Text, int From);

public sealed record ForgeRecipe(int Level, IReadOnlyList<Material> Mats);

/// <summary>A link in the upgrade tree: a weapon's row and the level the upgrade happens at.</summary>
public sealed record TreeLink(int Row, int Level);

public sealed record WeaponLevel(
    int Lv,
    int Raw,
    int Aff,
    int Defense,
    int Slots,
    int? Ele,
    int? Ele2,
    int[]? Sharp,
    int? PhialValue,
    int? ShellLv,
    IReadOnlyList<string>? Notes,
    BowStats? Bow,
    GunStats? Gun,
    IReadOnlyList<Material>? Cost)
{
    /// <summary>The row of the sharpness table this level's gauge comes from.</summary>
    public int SharpRow => Sharp![0];

    /// <summary>The gauge's length at Handicraft +0.</summary>
    public int SharpLength => Sharp![1];
}

/// <param name="Open">How many charge levels are open without the Load Up skill.</param>
/// <param name="Boost">The coatings the bow boosts.</param>
public sealed record BowStats(string Arc, int Open, IReadOnlyList<string> Shots, IReadOnlyList<string> Coatings, IReadOnlyList<string>? Boost);

public sealed record GunStats(
    string Reload,
    string Recoil,
    string Deviation,
    IReadOnlyList<Amount> Ammo,
    IReadOnlyList<InternalAmmo> Internal,
    IReadOnlyList<Amount>? Crouch,
    IReadOnlyList<RapidFire>? Rapid);

/// <summary>horn-songs.json</summary>
public sealed record HornSongs(IReadOnlyDictionary<string, string> Colours, IReadOnlyList<HornSong> Songs);

public sealed record HornSong(IReadOnlyList<string> Notes, string Name, string Effect, string Encore);

/// <summary>An armor set from armor.json, which comes in the game's own listing order.</summary>
/// <param name="Event">Listed after the last RARE 11 set: the event sets.</param>
/// <param name="Class">Blademaster, Gunner or Both.</param>
/// <param name="Gender">Male, Female or Both.</param>
/// <param name="Res">Fire, water, thunder, ice and dragon resistance of each piece.</param>
/// <param name="Defense">Each piece's defense per upgrade level.</param>
/// <param name="Upgrades">The cost of level 2, 3, …</param>
public sealed record ArmorSet(
    int Id,
    string Name,
    int Rarity,
    int Sort,
    bool Event,
    string Class,
    string Gender,
    int Zenny,
    IReadOnlyList<int> Res,
    IReadOnlyList<int> Defense,
    IReadOnlyList<ArmorPiece> Pieces,
    IReadOnlyList<IReadOnlyList<Material>> Upgrades);

public sealed record ArmorPiece(string Slot, string Name, string Desc, int Slots, IReadOnlyList<Amount> Skills, IReadOnlyList<Material>? Create);

/// <param name="Id">Its item id.</param>
public sealed record Decoration(int Id, string Name, string Desc, int Slots, IReadOnlyList<Amount> Skills, IReadOnlyList<IReadOnlyList<Material>> Recipes);
