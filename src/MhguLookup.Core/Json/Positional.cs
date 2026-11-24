using System.Text.Json;

namespace MhguLookup.Core.Json;

/// <summary>
/// Reads a JSON array as a fixed sequence of values, for the data files' positional rows such as
/// <c>["Iron Ore", 3]</c>. Each read moves to the next element.
/// </summary>
internal static class Positional
{
    public static void Start(ref Utf8JsonReader r)
    {
        if (r.TokenType != JsonTokenType.StartArray) throw new JsonException($"Expected an array, found {r.TokenType}.");
    }

    public static string String(ref Utf8JsonReader r)
    {
        Next(ref r);
        return r.GetString() ?? throw new JsonException("Expected a string, found null.");
    }

    public static int Int(ref Utf8JsonReader r)
    {
        Next(ref r);
        return r.GetInt32();
    }

    public static double Double(ref Utf8JsonReader r)
    {
        Next(ref r);
        return r.GetDouble();
    }

    public static int? NullableInt(ref Utf8JsonReader r)
    {
        Next(ref r);
        return r.TokenType == JsonTokenType.Null ? null : r.GetInt32();
    }

    /// <summary>Moves to the next element; false when the array has ended instead.</summary>
    public static bool TryNext(ref Utf8JsonReader r)
    {
        if (!r.Read()) throw new JsonException("Unexpected end of JSON.");
        return r.TokenType != JsonTokenType.EndArray;
    }

    /// <summary>Moves past the array's end, after its last expected element.</summary>
    public static void End(ref Utf8JsonReader r)
    {
        if (!r.Read() || r.TokenType != JsonTokenType.EndArray) throw new JsonException("The array has more elements than expected.");
    }

    private static void Next(ref Utf8JsonReader r)
    {
        if (!r.Read() || r.TokenType == JsonTokenType.EndArray) throw new JsonException("The array has fewer elements than expected.");
    }
}
