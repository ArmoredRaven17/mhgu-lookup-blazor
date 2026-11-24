using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using MhguLookup.Core;
using MhguLookup.Core.Models;

namespace MhguLookup.Web.Services;

/// <summary>
/// Loads the game data from <c>wwwroot/data/*.json</c>. Each file is fetched once per session and shared,
/// so switching tabs never downloads it again.
/// </summary>
public sealed class LookupData(HttpClient http)
{
    private readonly ConcurrentDictionary<string, Task<object>> _cache = new();

    public Task<SkillData> Skills() => Load("skills.json", LookupJsonContext.Default.SkillData);

    private async Task<T> Load<T>(string file, JsonTypeInfo<T> type) where T : class
    {
        var task = _cache.GetOrAdd(file, async f =>
            await http.GetFromJsonAsync($"data/{f}", type)
                ?? throw new InvalidDataException($"data/{f} is empty."));
        try
        {
            return (T)await task;
        }
        catch
        {
            _cache.TryRemove(file, out _);   // let a later visit retry a failed download
            throw;
        }
    }
}
