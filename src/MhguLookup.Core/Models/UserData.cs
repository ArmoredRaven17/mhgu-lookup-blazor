namespace MhguLookup.Core.Models;

/// <summary>A bookmarked page. Same shape as the React app's, so the two can share an export format.</summary>
/// <param name="Type">The entity type, e.g. <c>armorskill</c> or <c>weapon</c>.</param>
/// <param name="Path">The route of the entity, e.g. <c>armorskills/12</c>.</param>
public sealed record Bookmark(string Type, string Id, string Name, string Path, string? Icon = null);

/// <summary>The entity a note is attached to.</summary>
/// <param name="Category">Shown in the Notes tab, e.g. "Skill tree" or a weapon class.</param>
public sealed record NoteTarget(string Type, string Id, string Name, string Category, string Path, string? Icon = null);

/// <summary>A saved note: its target plus the text.</summary>
public sealed record NoteRecord(string Type, string Id, string Name, string Category, string Path, string Note, string? Icon = null)
{
    public static NoteRecord For(NoteTarget t, string note) => new(t.Type, t.Id, t.Name, t.Category, t.Path, note, t.Icon);
}
