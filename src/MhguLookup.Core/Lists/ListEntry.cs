namespace MhguLookup.Core.Lists;

/// <summary>One row of a tab's list panel.</summary>
/// <param name="Key">The route segment of the entry, usually its id.</param>
/// <param name="Sub">A muted second line.</param>
/// <param name="Search">Extra text the search box matches, beyond the label.</param>
/// <param name="Indent">Depth in a tree (the weapons' upgrade tree); ignored while searching.</param>
/// <param name="Tree">
/// Draw <paramref name="Indent"/> as branch lines with a collapse toggle. The entries must then come in
/// tree order: each child right after its parent, one level deeper.
/// </param>
/// <param name="Group">A heading the entry is listed under (entries come already ordered by group).</param>
/// <param name="Icon">An icon shown before the label.</param>
/// <param name="GroupColor">The colour of this entry's group heading (default: the accent colour).</param>
public sealed record ListEntry(
    string Key,
    string Label,
    string? Sub = null,
    string? Search = null,
    int Indent = 0,
    bool Tree = false,
    string? Group = null,
    string? Icon = null,
    string? GroupColor = null);
