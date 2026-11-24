using System.Text.Json.Serialization;

namespace MhguLookup.Core.Models;

// arts.json

/// <summary>A tier of a Hunter Art. An Art's tiers are separate rows, named "&lt;Art&gt; I", "II" and "III".</summary>
/// <param name="Category">Groups an Art's tiers; 1..10 are the generic Arts.</param>
/// <param name="Weapon">The weapon class (w00..w14), or null for a generic Art.</param>
/// <param name="Unlock">The ways the game teaches it; any one of them does.</param>
public sealed record HunterArt(int Id, string Name, string Short, string Full, int Category, string? Weapon, IReadOnlyList<ArtUnlock> Unlock);

/// <summary>One way the game teaches a Hunter Art. The JSON's <c>kind</c> picks the type.</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(StartUnlock), "start")]
[JsonDerivedType(typeof(QuestUnlock), "quest")]
[JsonDerivedType(typeof(QuestsUnlock), "quests")]
[JsonDerivedType(typeof(ProgressUnlock), "progress")]
[JsonDerivedType(typeof(RequestsUnlock), "requests")]
[JsonDerivedType(typeof(ContributionUnlock), "contribution")]
[JsonDerivedType(typeof(HuntUnlock), "hunt")]
[JsonDerivedType(typeof(PalicoUnlock), "palico")]
public abstract record ArtUnlock;

/// <summary>Known from the start.</summary>
public sealed record StartUnlock : ArtUnlock;

/// <summary>Clear one quest.</summary>
public sealed record QuestUnlock : ArtUnlock
{
    public int Quest { get; init; }
    public string Title { get; init; } = "";
    public string Category { get; init; } = "";
    public string Stars { get; init; } = "";

    public QuestRef Ref => new(Quest, Title, Category, Stars);
}

/// <summary>Clear every one of these quests.</summary>
public sealed record QuestsUnlock : ArtUnlock
{
    public IReadOnlyList<QuestRef> Quests { get; init; } = [];
}

/// <summary>Village N★ quests open, or reach HR N, or G1★ opens.</summary>
public sealed record ProgressUnlock : ArtUnlock
{
    public string? Village { get; init; }
    public int? Hr { get; init; }
    public string? Hub { get; init; }
}

public sealed record RequestsUnlock : ArtUnlock
{
    public string Village { get; init; } = "";
    public int Count { get; init; }
}

public sealed record ContributionUnlock : ArtUnlock
{
    public string Village { get; init; } = "";
    public int Points { get; init; }
}

/// <summary>Hunt any one of these monsters.</summary>
public sealed record HuntUnlock : ArtUnlock
{
    public IReadOnlyList<MonsterRef> Monsters { get; init; } = [];
}

public sealed record PalicoUnlock : ArtUnlock
{
    public int Level { get; init; }
}

public sealed record MonsterRef(string Id, string Name);
