using System.Text.Json;
using MhguLookup.Core.Models;

namespace MhguLookup.Core.Tests;

/// <summary>Bookmarks and notes are stored as JSON in localStorage, in the same shape as the React app's.</summary>
public class UserDataTests
{
    [Fact]
    public void A_bookmark_without_an_icon_leaves_the_field_out()
    {
        var json = JsonSerializer.Serialize([new Bookmark("armorskill", "1", "Poison", "armorskills/1")],
            LookupJsonContext.Default.ListBookmark);
        Assert.Equal("""[{"type":"armorskill","id":"1","name":"Poison","path":"armorskills/1"}]""", json);
    }

    [Fact]
    public void Notes_written_by_the_React_app_read_back()
    {
        const string json = """[{"type":"armorskill","id":"1","name":"Poison","category":"Skill tree","path":"/armorskills/1","note":"Bring antidotes"}]""";
        var note = JsonSerializer.Deserialize(json, LookupJsonContext.Default.ListNoteRecord)!.Single();
        Assert.Equal(("armorskill", "1", "Bring antidotes"), (note.Type, note.Id, note.Note));
        Assert.Null(note.Icon);
    }

    [Fact]
    public void A_note_record_copies_its_target()
    {
        var target = new NoteTarget("armorskill", "18", "Attack", "Skill tree", "armorskills/18");
        Assert.Equal(new NoteRecord("armorskill", "18", "Attack", "Skill tree", "armorskills/18", "text"), NoteRecord.For(target, "text"));
    }
}
