using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.JSInterop;

namespace MhguLookup.Web.Services;

/// <summary>
/// The browser's localStorage. Under WebAssembly, .NET and JavaScript share a thread, so these calls are
/// synchronous (<see cref="IJSInProcessRuntime"/>) and state can be read straight into a component's first render.
/// </summary>
/// <remarks>
/// Keys carry their own prefix, separate from the React app's <c>mhgu-lookup-*</c>. Both sites are served from
/// the same github.io origin, so a shared prefix would let this port read and overwrite the live app's notes.
/// </remarks>
public sealed class LocalStore(IJSRuntime js)
{
    public const string Prefix = "mhgu-lookup-blazor-";

    private IJSInProcessRuntime Js => (IJSInProcessRuntime)js;

    public string? Get(string key) => Js.Invoke<string?>("localStorage.getItem", Prefix + key);

    public void Set(string key, string value) => Js.InvokeVoid("localStorage.setItem", Prefix + key, value);

    /// <summary>Reads a JSON value, or <paramref name="fallback"/> when it is missing or unreadable.</summary>
    public T GetJson<T>(string key, JsonTypeInfo<T> type, T fallback)
    {
        var raw = Get(key);
        if (raw is null) return fallback;
        try { return JsonSerializer.Deserialize(raw, type) ?? fallback; }
        catch (JsonException) { return fallback; }
    }

    public void SetJson<T>(string key, T value, JsonTypeInfo<T> type) => Set(key, JsonSerializer.Serialize(value, type));
}
