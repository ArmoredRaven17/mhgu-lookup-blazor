using MhguLookup.Core;
using MhguLookup.Core.Models;

namespace MhguLookup.Web.Services;

/// <summary>The user's bookmarks, kept in localStorage.</summary>
public sealed class Bookmarks(LocalStore store)
{
    private const string Key = "bookmarks";

    public event Action? Changed;

    public IReadOnlyList<Bookmark> All => Read();

    public bool IsBookmarked(string path) => Read().Any(b => b.Path == path);

    public void Toggle(Bookmark bookmark)
    {
        var all = Read();
        var others = all.Where(b => b.Path != bookmark.Path).ToList();
        if (others.Count == all.Count) others.Add(bookmark);   // it was not bookmarked: add it
        Write(others);
    }

    public void Remove(string path) => Write(Read().Where(b => b.Path != path).ToList());

    private List<Bookmark> Read() => store.GetJson(Key, LookupJsonContext.Default.ListBookmark, []);

    private void Write(List<Bookmark> list)
    {
        store.SetJson(Key, list, LookupJsonContext.Default.ListBookmark);
        Changed?.Invoke();
    }
}

/// <summary>The user's own notes on game entities, kept in localStorage.</summary>
public sealed class Notes(LocalStore store)
{
    private const string Key = "notes";

    public event Action? Changed;

    public IReadOnlyList<NoteRecord> All => Read();

    /// <summary>The note for an entity, or an empty string when there is none.</summary>
    public string Get(string type, string id) => Read().FirstOrDefault(n => n.Type == type && n.Id == id)?.Note ?? "";

    /// <summary>Save a note. A blank note deletes it.</summary>
    public void Set(NoteTarget target, string note)
    {
        var others = Read().Where(n => !(n.Type == target.Type && n.Id == target.Id)).ToList();
        if (!string.IsNullOrWhiteSpace(note)) others.Add(NoteRecord.For(target, note));
        Write(others);
    }

    public void Remove(string type, string id) => Write(Read().Where(n => !(n.Type == type && n.Id == id)).ToList());

    private List<NoteRecord> Read() =>
        store.GetJson(Key, LookupJsonContext.Default.ListNoteRecord, [])
            .Where(n => n is { Type.Length: > 0, Id.Length: > 0 } && n.Note is not null)
            .ToList();

    private void Write(List<NoteRecord> list)
    {
        store.SetJson(Key, list, LookupJsonContext.Default.ListNoteRecord);
        Changed?.Invoke();
    }
}
