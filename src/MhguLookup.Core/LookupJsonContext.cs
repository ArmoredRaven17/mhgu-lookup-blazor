using System.Text.Json;
using System.Text.Json.Serialization;
using MhguLookup.Core.Models;

namespace MhguLookup.Core;

/// <summary>
/// Source-generated System.Text.Json metadata for every type read from <c>data/*.json</c> or localStorage.
/// Source generation keeps deserialization working after the WebAssembly publish trims unused code,
/// and skips runtime reflection, which is slow under WASM.
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(SkillData))]
[JsonSerializable(typeof(List<Bookmark>))]
[JsonSerializable(typeof(List<NoteRecord>))]
[JsonSerializable(typeof(Dictionary<string, int>))]
public sealed partial class LookupJsonContext : JsonSerializerContext;
