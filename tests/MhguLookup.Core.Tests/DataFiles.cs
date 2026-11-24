using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MhguLookup.Core.Tests;

/// <summary>Reads the data files the app ships, from src/MhguLookup.Web/wwwroot/data.</summary>
internal static class DataFiles
{
    private static readonly Lazy<string> Dir = new(() =>
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            var path = Path.Combine(dir.FullName, "src", "MhguLookup.Web", "wwwroot", "data");
            if (Directory.Exists(path)) return path;
        }
        throw new DirectoryNotFoundException($"No wwwroot/data above {AppContext.BaseDirectory}.");
    });

    public static T Load<T>(string file, JsonTypeInfo<T> type) =>
        JsonSerializer.Deserialize(File.ReadAllText(Path.Combine(Dir.Value, file)), type)
            ?? throw new InvalidDataException($"{file} is empty.");

    public static IEnumerable<string> WeaponClassFiles() =>
        Directory.GetFiles(Path.Combine(Dir.Value, "weapons"), "*.json").Select(f => "weapons/" + Path.GetFileName(f));
}
