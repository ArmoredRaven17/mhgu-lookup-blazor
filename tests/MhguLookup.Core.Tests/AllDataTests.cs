using MhguLookup.Core.Models;

namespace MhguLookup.Core.Tests;

/// <summary>Every data file the app ships reads into its model, including the positional rows and tagged unions.</summary>
public class AllDataTests
{
    private static LookupJsonContext Ctx => LookupJsonContext.Default;

    [Fact]
    public void Weapons_every_class_reads_and_its_tree_links_resolve()
    {
        var index = DataFiles.Load("weapons.json", Ctx.WeaponIndex);
        Assert.Equal(14, index.Classes.Count);
        Assert.All(index.Sharpness, row => Assert.Equal(7, row.Length));

        foreach (var file in DataFiles.WeaponClassFiles())
        {
            var cls = DataFiles.Load(file, Ctx.WeaponClass);
            var rows = cls.Weapons.Select(w => w.Row).ToHashSet();
            Assert.Equal(index.Classes.Single(c => c.Id == cls.Id).Count, cls.Weapons.Count);
            Assert.All(cls.Weapons, w =>
            {
                Assert.NotEmpty(w.Levels);
                Assert.All(w.Into, t => Assert.Contains(t.Row, rows));
                if (w.From is { } from) Assert.Contains(from.Row, rows);
                Assert.All(w.Levels.Where(l => l.Sharp is not null), l => Assert.InRange(l.SharpRow, 0, index.Sharpness.Count - 1));
            });
        }
    }

    [Fact]
    public void Gun_and_bow_rows_read_positionally()
    {
        var lbg = DataFiles.Load("weapons/w06.json", Ctx.WeaponClass);
        var gun = lbg.Weapons.SelectMany(w => w.Levels).First(l => l.Gun?.Rapid is { Count: > 0 }).Gun!;
        Assert.NotEmpty(gun.Ammo);
        Assert.All(gun.Rapid!, r => Assert.InRange(r.Shots, 1, 9));

        var bow = DataFiles.Load("weapons/w10.json", Ctx.WeaponClass);
        Assert.Contains(bow.Weapons.SelectMany(w => w.Levels), l => l.Bow is { Shots.Count: > 0 });
    }

    [Fact]
    public void Material_point_costs_are_told_apart_from_item_costs()
    {
        var armor = DataFiles.Load("armor.json", Ctx.ListArmorSet);
        var upgrades = armor.SelectMany(a => a.Upgrades).SelectMany(u => u).ToList();
        Assert.Contains(upgrades, m => m.IsPoints);
        Assert.Contains(upgrades, m => !m.IsPoints);
        Assert.Equal(1250, armor.Count);
    }

    [Fact]
    public void Hunter_art_unlocks_read_every_kind()
    {
        var arts = DataFiles.Load("arts.json", Ctx.ListHunterArt);
        var kinds = arts.SelectMany(a => a.Unlock).Select(u => u.GetType()).ToHashSet();
        Assert.Contains(typeof(StartUnlock), kinds);
        Assert.Contains(typeof(QuestUnlock), kinds);
        Assert.Contains(typeof(HuntUnlock), kinds);
        Assert.All(arts.SelectMany(a => a.Unlock).OfType<QuestUnlock>(), q => Assert.NotEmpty(q.Title));
    }

    [Fact]
    public void Canteen_unlocks_dishes_and_menus_read()
    {
        var c = DataFiles.Load("canteen.json", Ctx.Canteen);
        Assert.Equal(c.Ingredients.Count, c.Unlocks.Count);
        Assert.All(c.Dishes, d => Assert.True(c.Menus.ContainsKey(d.Menu.ToString())));
        Assert.All(c.Menus.Values.SelectMany(m => m), s => Assert.NotEmpty(s.Name));
        Assert.Contains(c.Unlocks.SelectMany(u => u), u => u is RequestIngredient { Items.Count: > 0 });
    }

    [Fact]
    public void Monsters_hit_zones_and_rewards_read()
    {
        var m = DataFiles.Load("monsters.json", Ctx.MonsterData);
        var rathian = m.Monsters.Single(x => x.Id == "em001_00");
        Assert.Equal("Rathian", rathian.Name);
        Assert.Equal(90, rathian.Hz!.Zones[0].Row["cut"]);
        Assert.NotEmpty(rathian.Rewards!["Low Rank"].Carve![0].Rows);
        Assert.Contains(rathian.Quests, q => q.Name == "G");
    }

    [Fact]
    public void Quests_read_with_each_bosss_stats()
    {
        var quests = DataFiles.Load("quests.json", Ctx.ListQuest);
        Assert.Equal(1468, quests.Count);
        var stats = quests.SelectMany(q => q.Monsters).First(m => m.Stats is not null).Stats!;
        Assert.True(stats.HpMin <= stats.Hp && stats.Hp <= stats.HpMax);
        Assert.Contains(stats.Status, s => s.Step % 1 != 0);   // steps such as 28.5 keep their fraction
        Assert.Contains(quests, q => q.Target?.Item is not null);
        Assert.Contains(quests, q => q.Target?.Monster is not null);
    }

    [Fact]
    public void Item_sources_read_with_and_without_a_chance()
    {
        var sources = DataFiles.Load("sources.json", Ctx.DictionaryStringListItemSource);
        var all = sources.Values.SelectMany(s => s).ToList();
        Assert.Contains(all, s => s.Percent is null);
        Assert.Contains(all, s => s.Percent is not null);
    }

    [Fact]
    public void The_remaining_files_read()
    {
        Assert.Equal(6, DataFiles.Load("styles.json", Ctx.ListStyle).Count);
        Assert.Equal(242, DataFiles.Load("decorations.json", Ctx.ListDecoration).Count);
        Assert.Equal(2624, DataFiles.Load("items.json", Ctx.ListItem).Count);
        Assert.Equal(183, DataFiles.Load("combos.json", Ctx.ListCombo).Count);
        Assert.Equal(26, DataFiles.Load("gathering.json", Ctx.ListGatherMap).Count);
        Assert.NotEmpty(DataFiles.Load("horn-songs.json", Ctx.HornSongs).Songs);
        Assert.NotEmpty(DataFiles.Load("item-icons.json", Ctx.ItemIconMap).ById);
        Assert.NotEmpty(DataFiles.Load("monster-icons.json", Ctx.DictionaryStringString));
        Assert.Equal(5, DataFiles.Load("equip-icons.json", Ctx.EquipIconMap).Armor.Count);

        var palico = DataFiles.Load("palico.json", Ctx.Palico);
        Assert.Equal("RARE 3", palico.Rare(3));
        Assert.Equal(5, palico.Labels.Res.Count);
        Assert.NotEmpty(palico.Weapons[0].Materials);
    }
}
