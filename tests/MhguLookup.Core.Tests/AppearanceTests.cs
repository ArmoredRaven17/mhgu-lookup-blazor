using MhguLookup.Core.Theme;

namespace MhguLookup.Core.Tests;

public class AppearanceTests
{
    [Theory]
    [InlineData("#c8a84b", "#c8a84b")]
    [InlineData("  #C8A84B ", "#C8A84B")]
    [InlineData("c8a84b", Appearance.DefaultAccent)]
    [InlineData("#fff", Appearance.DefaultAccent)]
    [InlineData(null, Appearance.DefaultAccent)]
    public void ClampHex_accepts_only_six_digit_colours(string? input, string expected)
    {
        Assert.Equal(expected, Appearance.ClampHex(input));
    }

    [Fact]
    public void Darken_scales_each_channel_and_rounds()
    {
        Assert.Equal("#903030", Appearance.Darken("#C04040", 0.75));
    }

    [Fact]
    public void Accent_variables_include_the_dimmed_and_translucent_versions()
    {
        var vars = Appearance.AccentVariables("#C04040");
        Assert.Equal("#C04040", vars["--accent"]);
        Assert.Equal("#903030", vars["--accent-dim"]);
        Assert.Equal("rgba(192, 64, 64, 0.12)", vars["--header-bg"]);
    }

    [Fact]
    public void An_unknown_surface_falls_back_to_the_default_theme()
    {
        Assert.Equal(Appearance.SurfaceVariables(Appearance.DefaultSurface), Appearance.SurfaceVariables("no-such-theme"));
    }

    [Fact]
    public void Surface_variables_carry_rgb_lists_for_translucent_overlays()
    {
        var vars = Appearance.SurfaceVariables("charcoal");
        Assert.Equal("#161616", vars["--bg"]);
        Assert.Equal("22, 22, 22", vars["--bg-rgb"]);
    }

    [Fact]
    public void Preset_keys_are_unique()
    {
        Assert.Equal(Appearance.Presets.Count, Appearance.Presets.Select(p => p.Key).Distinct().Count());
    }
}
