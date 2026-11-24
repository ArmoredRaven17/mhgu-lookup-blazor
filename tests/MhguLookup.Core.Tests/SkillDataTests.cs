using MhguLookup.Core.Models;

namespace MhguLookup.Core.Tests;

/// <summary>Checks the real <c>skills.json</c> the app ships against the models it is read into.</summary>
public class SkillDataTests
{
    private static readonly Lazy<SkillData> Data = new(() => DataFiles.Load("skills.json", LookupJsonContext.Default.SkillData));

    [Fact]
    public void Deserializes_every_tree_and_skill()
    {
        Assert.Equal(204, Data.Value.Trees.Count);
        Assert.Equal(326, Data.Value.Skills.Count);
    }

    [Fact]
    public void Fields_map_from_camelCase_json()
    {
        var poison = Data.Value.Trees.Single(t => t.Id == 1);
        Assert.Equal("Poison", poison.Name);
        Assert.Contains(poison.Ladder, l => l is { Points: 10, Skill: 1 });

        var negate = Data.Value.Skills.Single(s => s.Id == 1);
        Assert.Equal("Negate Poison", negate.Name);
        Assert.NotEmpty(negate.Brief);
        Assert.Contains("Wellness", negate.GrantedBy);
    }

    [Fact]
    public void Every_ladder_step_points_at_a_real_skill()
    {
        var ids = Data.Value.Skills.Select(s => s.Id).ToHashSet();
        var missing = Data.Value.Trees.SelectMany(t => t.Ladder.Select(l => (t.Name, l.Skill))).Where(x => !ids.Contains(x.Skill));
        Assert.Empty(missing);
    }

    [Fact]
    public void Tree_and_skill_ids_are_unique()
    {
        Assert.Equal(Data.Value.Trees.Count, Data.Value.Trees.Select(t => t.Id).Distinct().Count());
        Assert.Equal(Data.Value.Skills.Count, Data.Value.Skills.Select(s => s.Id).Distinct().Count());
    }
}
