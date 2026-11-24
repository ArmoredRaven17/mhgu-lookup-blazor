using System.Globalization;
using MhguLookup.Core;
using MhguLookup.Core.Theme;
using Microsoft.JSInterop;

namespace MhguLookup.Web.Services;

/// <summary>
/// The persisted UI settings: colours, text size, and the list panel's width and collapsed state.
/// The list panel state is global, not per tab, so collapsing it on one tab keeps it collapsed everywhere.
/// </summary>
public sealed class UiSettings(LocalStore store, IJSRuntime js)
{
    private const string SurfaceKey = "surface";
    private const string AccentKey = "accent";
    private const string TextScaleKey = "text-scale";
    private const string CollapsedKey = "section-collapsed";
    private const string WidthsKey = "section-widths";

    /// <summary>Raised when a setting that affects layout changes, so every open component can re-render.</summary>
    public event Action? Changed;

    private IJSInProcessRuntime Js => (IJSInProcessRuntime)js;

    public double TextScale => double.TryParse(store.Get(TextScaleKey), CultureInfo.InvariantCulture, out var s) && s > 0 ? s : 1;

    public bool ListCollapsed => store.Get(CollapsedKey) == "true";

    /// <summary>The surface theme's key (see <see cref="Appearance.Presets"/>).</summary>
    public string Surface => store.Get(SurfaceKey) ?? Appearance.DefaultSurface;

    /// <summary>The accent colour, as #RRGGBB.</summary>
    public string Accent => Appearance.ClampHex(store.Get(AccentKey) ?? Appearance.DefaultAccent);

    /// <summary>The Text Size choices: text only, not a page zoom, so the layout stays put.</summary>
    public static IReadOnlyList<(double Value, string Label)> TextScales { get; } =
        [(0.75, "75%"), (1, "100% (default)"), (1.25, "125%"), (1.5, "150%"), (1.75, "175%"), (2, "200%")];

    /// <summary>Apply the saved colours and text size. Called once, when the app starts.</summary>
    public void ApplySaved()
    {
        SetCssVariables(Appearance.SurfaceVariables(Surface));
        SetCssVariables(Appearance.AccentVariables(Accent));
        ApplyTextScale();
    }

    public void SetSurface(string key)
    {
        store.Set(SurfaceKey, key);
        SetCssVariables(Appearance.SurfaceVariables(key));
    }

    public void SetAccent(string hex)
    {
        var h = Appearance.ClampHex(hex);
        store.Set(AccentKey, h);
        SetCssVariables(Appearance.AccentVariables(h));
    }

    public void SetTextScale(double scale)
    {
        store.Set(TextScaleKey, scale.ToString(CultureInfo.InvariantCulture));
        ApplyTextScale();
        Changed?.Invoke();   // the list panel's width follows the text size
    }

    /// <summary>Back to the default colours and text size.</summary>
    public void Reset()
    {
        SetSurface(Appearance.DefaultSurface);
        SetAccent(Appearance.DefaultAccent);
        SetTextScale(1);
    }

    private void ApplyTextScale() =>
        SetCssVariables(new Dictionary<string, string> { ["--text-scale"] = TextScale.ToString(CultureInfo.InvariantCulture) });

    public void ToggleListCollapsed()
    {
        store.Set(CollapsedKey, ListCollapsed ? "false" : "true");
        Changed?.Invoke();
    }

    /// <summary>
    /// A width the user dragged the list panel to. Widths are keyed by each page's base width,
    /// so pages that share a base width share a remembered size.
    /// </summary>
    public int? ListWidth(int baseWidth) =>
        Widths().TryGetValue(baseWidth.ToString(CultureInfo.InvariantCulture), out var w) ? w : null;

    public void SetListWidth(int baseWidth, int px)
    {
        var map = Widths();
        map[baseWidth.ToString(CultureInfo.InvariantCulture)] = px;
        store.SetJson(WidthsKey, map, LookupJsonContext.Default.DictionaryStringInt32);
        Changed?.Invoke();
    }

    private Dictionary<string, int> Widths() =>
        store.GetJson(WidthsKey, LookupJsonContext.Default.DictionaryStringInt32, []);

    private void SetCssVariables(IReadOnlyDictionary<string, string> vars)
    {
        foreach (var (name, value) in vars) Js.InvokeVoid("mhgu.setCssVar", name, value);
    }
}
