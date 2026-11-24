using MhguLookup.Core.Lists;

namespace MhguLookup.Core.Tests;

public class ListViewTests
{
    // A small upgrade tree, in tree order:
    //   Root
    //   ├ A
    //   │ └ A1
    //   └ B
    //     ├ B1
    //     └ B2
    private static readonly ListEntry[] Tree =
    [
        new("root", "Root", Tree: true),
        new("a", "A", Tree: true, Indent: 1),
        new("a1", "A1", Tree: true, Indent: 2, Search: "alias"),
        new("b", "B", Tree: true, Indent: 1),
        new("b1", "B1", Tree: true, Indent: 2),
        new("b2", "B2", Tree: true, Indent: 2),
    ];

    private static readonly HashSet<string> None = [];

    [Fact]
    public void Without_search_or_collapse_every_entry_shows()
    {
        Assert.Same(Tree, ListView.Filter(Tree, "", None));
    }

    [Fact]
    public void Search_matches_label_and_extra_search_text_case_insensitively()
    {
        Assert.Equal(["a1"], ListView.Filter(Tree, "  ALIAS ", None).Select(e => e.Key));
        Assert.Equal(["b", "b1", "b2"], ListView.Filter(Tree, "b", None).Select(e => e.Key));
    }

    [Fact]
    public void Collapsing_a_branch_hides_everything_below_it_until_the_branch_ends()
    {
        var shown = ListView.Filter(Tree, null, new HashSet<string> { "a" });
        Assert.Equal(["root", "a", "b", "b1", "b2"], shown.Select(e => e.Key));

        shown = ListView.Filter(Tree, null, new HashSet<string> { "root" });
        Assert.Equal(["root"], shown.Select(e => e.Key));
    }

    [Fact]
    public void Search_ignores_collapsed_branches()
    {
        Assert.Equal(["a1"], ListView.Filter(Tree, "A1", new HashSet<string> { "a" }).Select(e => e.Key));
    }

    [Fact]
    public void Tree_lines_draw_tees_corners_and_continuing_branches()
    {
        var lines = ListView.BuildTreeLines(Tree);

        Assert.Empty(lines.For("root"));
        Assert.Equal([Guide.Tee], lines.For("a"));                    // ├ A
        Assert.Equal([Guide.Through, Guide.Corner], lines.For("a1")); // │ └ A1
        Assert.Equal([Guide.Corner], lines.For("b"));                 // └ B
        Assert.Equal([Guide.None, Guide.Tee], lines.For("b1"));       //   ├ B1
        Assert.Equal([Guide.None, Guide.Corner], lines.For("b2"));    //   └ B2
    }

    [Fact]
    public void Only_entries_with_children_get_a_collapse_toggle()
    {
        var lines = ListView.BuildTreeLines(Tree);
        Assert.Equal(new HashSet<string> { "root", "a", "b" }, lines.HasChildren);
    }

    [Fact]
    public void Entries_that_are_not_tree_entries_get_no_lines()
    {
        var lines = ListView.BuildTreeLines([new ListEntry("x", "X"), new ListEntry("y", "Y", Indent: 1)]);
        Assert.Empty(lines.Cells);
    }
}
