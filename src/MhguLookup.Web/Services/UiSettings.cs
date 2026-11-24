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

    /// <summary>Apply the saved colours and text size. Called once, when the app starts.</summary>
    public void ApplySaved()
    {
        SetCssVariables(Appearance.SurfaceVariables(store.Get(SurfaceKey) ?? Appearance.DefaultSurface));
        SetCssVariables(Appearance.AccentVariables(store.Get(AccentKey) ?? Appearance.DefaultAccent));
        SetCssVariables(new Dictionary<string, string> { ["--text-scale"] = TextScale.ToString(CultureInfo.InvariantCulture) });
    }

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
