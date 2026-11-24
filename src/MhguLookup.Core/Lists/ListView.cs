namespace MhguLookup.Core.Lists;

/// <summary>A branch-line cell drawn before a tree entry.</summary>
public enum Guide
{
    /// <summary>Blank.</summary>
    None = 0,
    /// <summary>│ — a branch above continues past this row.</summary>
    Through = 1,
    /// <summary>├ — this entry, with a later sibling below.</summary>
    Tee = 2,
    /// <summary>└ — this entry, the last of its siblings.</summary>
    Corner = 3,
}

/// <summary>The branch lines of a tree list, and which entries have children (and so get a collapse toggle).</summary>
public sealed record TreeLines(IReadOnlyDictionary<string, Guide[]> Cells, IReadOnlySet<string> HasChildren)
{
    public Guide[] For(string key) => Cells.TryGetValue(key, out var c) ? c : [];
}

/// <summary>The list panel's logic, kept free of UI so it can be unit tested.</summary>
public static class ListView
{
    /// <summary>
    /// The entries to show. A search matches the label plus <see cref="ListEntry.Search"/>, case-insensitively,
    /// and ignores collapsing. Without a search, the children of collapsed tree entries are hidden.
    /// </summary>
    public static IReadOnlyList<ListEntry> Filter(IReadOnlyList<ListEntry> entries, string? search, IReadOnlySet<string> collapsed)
    {
        var q = search?.Trim();
        if (!string.IsNullOrEmpty(q))
            return entries.Where(e => (e.Label + " " + e.Search).Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        if (collapsed.Count == 0) return entries;

        var shown = new List<ListEntry>();
        var hideBelow = -1;                      // hide entries deeper than this until the branch ends
        foreach (var e in entries)
        {
            if (hideBelow >= 0 && e.Indent > hideBelow) continue;
            hideBelow = -1;
            shown.Add(e);
            if (e.Tree && collapsed.Contains(e.Key)) hideBelow = e.Indent;
        }
        return shown;
    }

    /// <summary>
    /// For entries in tree order: one cell per level of each tree entry (see <see cref="Guide"/>),
    /// so the lines join across rows of any height.
    /// </summary>
    public static TreeLines BuildTreeLines(IReadOnlyList<ListEntry> entries)
    {
        var cells = new Dictionary<string, Guide[]>();
        var hasChildren = new HashSet<string>();
        var n = entries.Count;

        // laterSibling[i]: a later entry sits at i's depth before any shallower one.
        var laterSibling = new bool[n];
        var seen = new List<bool>();             // seen[d]: a later entry at depth d since the last shallower one
        for (var i = n - 1; i >= 0; i--)
        {
            var d = entries[i].Indent;
            laterSibling[i] = d < seen.Count && seen[d];
            if (seen.Count > d + 1) seen.RemoveRange(d + 1, seen.Count - d - 1);
            while (seen.Count <= d) seen.Add(false);
            seen[d] = true;
        }

        var open = new List<bool>();             // open[d]: the branch at level d continues below (draw │)
        bool IsOpen(int d) => d < open.Count && open[d];
        for (var i = 0; i < n; i++)
        {
            var e = entries[i];
            if (!e.Tree) continue;
            var d = e.Indent;
            var row = new List<Guide>();
            for (var k = 0; k < d - 1; k++) row.Add(IsOpen(k + 1) ? Guide.Through : Guide.None);
            if (d > 0) row.Add(laterSibling[i] ? Guide.Tee : Guide.Corner);
            while (open.Count <= d) open.Add(false);
            open[d] = laterSibling[i];
            cells[e.Key] = [.. row];
            if (i + 1 < n && entries[i + 1].Indent > d) hasChildren.Add(e.Key);
        }
        return new TreeLines(cells, hasChildren);
    }
}
