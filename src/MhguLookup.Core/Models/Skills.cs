namespace MhguLookup.Core.Models;

/// <summary>The contents of <c>data/skills.json</c>: every skill tree and every skill it can activate.</summary>
public sealed record SkillData(IReadOnlyList<SkillTree> Trees, IReadOnlyList<Skill> Skills)
{
    public static SkillData Empty { get; } = new([], []);
}

/// <summary>A skill tree (e.g. "Poison") and the skill each point total activates.</summary>
public sealed record SkillTree(int Id, string Name, string Desc, IReadOnlyList<LadderStep> Ladder);

/// <summary>One rung of a tree's ladder: at <see cref="Points"/> points, <see cref="Skill"/> activates.</summary>
public sealed record LadderStep(int Points, int Skill);

/// <summary>An activated skill (e.g. "Negate Poison").</summary>
/// <param name="Brief">A short explanation; the full one is in MHGU Armor Skills Explained.</param>
/// <param name="GrantedBy">Other sources of the skill, such as set bonuses or talismans.</param>
public sealed record Skill(
    int Id,
    string Name,
    string Desc,
    IReadOnlyList<SkillTreeRef> Trees,
    string Brief,
    IReadOnlyList<string> GrantedBy);

public sealed record SkillTreeRef(int Tree, int Points);
