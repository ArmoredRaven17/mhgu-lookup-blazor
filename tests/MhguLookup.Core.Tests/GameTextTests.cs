using MhguLookup.Core.Text;

namespace MhguLookup.Core.Tests;

public class GameTextTests
{
    private static string Plain(GameTextResult r) =>
        string.Concat(r.Parts.Select(p => p.IsBreak ? "\n" : p.Text));

    [Fact]
    public void Empty_text_has_no_parts()
    {
        Assert.Empty(GameText.Parse(null).Parts);
        Assert.Empty(GameText.Parse("").Parts);
    }

    [Fact]
    public void The_games_line_breaks_are_joined_into_running_text()
    {
        var r = GameText.Parse("Skills that affect your\nresistance to Poison.");
        Assert.Equal("Skills that affect your resistance to Poison.", Plain(r));
        Assert.False(r.Centred);
    }

    [Fact]
    public void A_word_hyphenated_at_a_break_keeps_its_hyphen_without_a_space()
    {
        Assert.Equal("a well-known item", Plain(GameText.Parse("a well-\nknown item")));
    }

    [Fact]
    public void A_blank_line_stays_a_paragraph_break()
    {
        var r = GameText.Parse("First line\nwraps.\n\nSecond paragraph.");
        Assert.Equal("First line wraps.\n\nSecond paragraph.", Plain(r));
    }

    [Fact]
    public void Centred_text_keeps_every_line_break()
    {
        var r = GameText.Parse("<CNTR>Line one\nLine two");
        Assert.True(r.Centred);
        Assert.Equal("Line one\nLine two", Plain(r));
    }

    [Fact]
    public void Colour_tags_colour_their_run_and_drop_the_alpha()
    {
        var r = GameText.Parse("Use <COL 00ff00FF>this</COL> now");
        Assert.Collection(r.Parts,
            p => Assert.Equal(("Use ", (string?)null), (p.Text, p.Color)),
            p => Assert.Equal(("this", (string?)"#00FF00"), (p.Text, p.Color)),
            p => Assert.Equal((" now", (string?)null), (p.Text, p.Color)));
    }

    [Fact]
    public void The_games_plain_red_is_flagged_for_the_readable_red()
    {
        var part = GameText.Parse("<COL FF0000FF>Warning</COL>").Parts.Single();
        Assert.True(part.IsDefaultHighlight);
    }

    [Fact]
    public void Unknown_tags_are_dropped()
    {
        Assert.Equal("Hello there", Plain(GameText.Parse("Hello <ICON 12>there")));
    }

    [Theory]
    [InlineData("Supply")]
    [InlineData("Account")]
    public void A_supply_or_account_tag_gets_a_line_of_its_own(string kind)
    {
        var r = GameText.Parse($"A potion for\nthe hunt. <COL FF0000FF>({kind} Item)</COL>");
        Assert.Equal($"A potion for the hunt.\n({kind} Item)", Plain(r));
    }
}
