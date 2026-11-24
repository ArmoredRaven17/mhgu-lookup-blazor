using MhguLookup.Core.Lists;
using MhguLookup.Core.Models;
using MhguLookup.Core.Text;
using MhguLookup.Core.Theme;

namespace MhguLookup.Core.Tests;

public class GameColorsTests
{
    [Theory]
    [InlineData("Rapid Lv1", "rgb(200,200,255)")]
    [InlineData("Rapid Lv5", "rgb(88,88,255)")]
    [InlineData("Spread Lv3", "rgb(144,255,144)")]
    [InlineData("Heavy Lv2", "rgb(232,172,255)")]
    [InlineData("Unknown Lv2", null)]
    [InlineData("not a shot", null)]
    public void Bow_shots_get_their_type_hue_stronger_with_level(string shot, string? expected)
    {
        Assert.Equal(expected, GameColors.ShotColor(shot));
    }

    [Theory]
    [InlineData("P.Flaming S", "#FF4D2E")]
    [InlineData("Flaming S", "#FF4D2E")]
    [InlineData("Demon S", "#FF5252")]
    [InlineData("Demon Armor S", "#FFA040")]
    [InlineData("Mystery S", null)]
    public void Ammo_colours_match_the_first_pattern(string ammo, string? expected)
    {
        Assert.Equal(expected, GameColors.AmmoColor(ammo));
    }

    [Fact]
    public void A_sharpness_gauge_is_cut_at_its_length()
    {
        int[] row = [10, 20, 30, 40, 50, 60, 70];
        Assert.Equal([10, 10, 5, 0, 0, 0, 0], GameColors.SharpSegments(row, 25));
        Assert.Equal([10, 10, 10, 10, 10, 10, 10], GameColors.SharpSegments(row, 400));
    }

    [Theory]
    [InlineData(66, "cut", "#2e5e2e")]
    [InlineData(65, "cut", "#6b3d00")]
    [InlineData(25, "fire", "#2e5e2e")]
    [InlineData(10, "fire", "#5a5200")]
    [InlineData(9, "fire", "#383838")]
    [InlineData(20, "stun", "#383838")]
    public void Raw_and_element_hit_zones_use_their_own_tiers(int value, string type, string expected)
    {
        Assert.Equal(expected, GameColors.HitZoneBackground(value, type));
    }

    [Fact]
    public void Extracts_get_dark_text_on_light_colours()
    {
        Assert.Equal(("rgb(224,224,230)", "#101216"), GameColors.ExtractColors("white"));
        Assert.Equal(("rgb(199,41,41)", "#f2f4f8"), GameColors.ExtractColors("red"));
        Assert.Null(GameColors.ExtractColors(null));
    }
}

public class FormatTests
{
    [Theory]
    [InlineData(1500, "1,500")]
    [InlineData(28.5, "28.5")]
    [InlineData(14.25, "14.25")]
    [InlineData(0, "0")]
    public void N_groups_thousands_and_keeps_fractions(double n, string expected) => Assert.Equal(expected, Format.N(n));

    [Theory]
    [InlineData(0.7, "0.7")]
    [InlineData(1.0, "1")]
    public void Plain_drops_trailing_zeros(double n, string expected) => Assert.Equal(expected, Format.Plain(n));

    [Fact]
    public void Signs_and_articles()
    {
        Assert.Equal("+3", Format.Signed(3));
        Assert.Equal("-2", Format.Signed(-2));
        Assert.Equal("0", Format.Signed(0));
        Assert.Null(Format.SignClass(0));
        Assert.Equal("an", Format.Article("Arzuros"));
        Assert.Equal("a", Format.Article("Rathian"));
    }

    [Fact]
    public void Markup_is_stripped_and_lines_flattened()
    {
        Assert.Equal("Balanced", Format.PlainText("<COL FFFF50FF>Balanced</COL>"));
        Assert.Equal("a well-known item here", Format.Flat("a well-\nknown item\nhere"));
    }
}

public class GroupingTests
{
    private static Quest Q(int id, string category, string? target, params (string Id, string Name)[] monsters) => new(
        id, $"Quest {id}", "", "", "", "", target is null ? null : new QuestTarget(target, null), "", "", category, "", 1, "", false, 0,
        "", 50, 0, 0, 0, 0, 0, [], monsters.Select(m => new QuestMonster(m.Id, m.Name, null)).ToList());

    [Fact]
    public void A_permit_group_is_named_after_the_monster_most_of_its_quests_target()
    {
        Quest[] quests =
        [
            Q(400101, "Special Permit", "em_a", ("em_a", "Dreadqueen")),
            Q(400102, "Special Permit", "em_b", ("em_b", "Other"), ("em_a", "Dreadqueen")),
            Q(400103, "Special Permit", "em_a", ("em_a", "Dreadqueen")),
            Q(400201, "Special Permit", "em_c", ("em_c", "Hellblade")),
            Q(100001, "Village", "em_a", ("em_a", "Dreadqueen")),
        ];
        var groups = Grouping.PermitMonsters(quests);
        Assert.Equal(new MonsterRef("em_a", "Dreadqueen"), groups[1]);
        Assert.Equal(new MonsterRef("em_c", "Hellblade"), groups[2]);
        Assert.Equal(2, groups.Count);
    }

    [Fact]
    public void Art_tiers_group_under_their_name_generic_first_then_guild_card_order()
    {
        HunterArt Art(int id, string name, string? weapon, int category = 1) => new(id, name, "", "", category, weapon, []);
        var groups = Grouping.ArtGroups(
        [
            Art(10, "Bow Art I", "w10"), Art(11, "Bow Art II", "w10"),
            Art(20, "Great Art I", "w00"),
            Art(1, "Generic Art", null),
        ]);
        Assert.Equal(["Generic Art", "Great Art", "Bow Art"], groups.Select(g => g.Name));
        Assert.Equal(2, groups[2].Tiers.Count);
        Assert.Equal("10", groups[2].Key);
    }

    [Fact]
    public void A_monsters_quests_come_in_category_then_level_order()
    {
        Quest[] quests =
        [
            Q(3, "Hub", null, ("em_a", "A")) with { Level = 2 },
            Q(2, "Village", null, ("em_a", "A")) with { Level = 5 },
            Q(1, "Hub", null, ("em_a", "A")) with { Level = 1 },
            Q(4, "Village", null, ("em_b", "B")),
        ];
        Assert.Equal([2, 1, 3], Grouping.QuestsWith(quests, "em_a").Select(q => q.Id));
    }
}
