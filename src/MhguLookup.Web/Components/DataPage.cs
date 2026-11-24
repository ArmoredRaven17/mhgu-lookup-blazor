using System.Text.Json;
using MhguLookup.Web.Services;
using Microsoft.AspNetCore.Components;

namespace MhguLookup.Web.Components;

/// <summary>
/// Base for a tab that loads data files: tracks loading, and turns a failed download into a message on the page
/// instead of Blazor's error bar.
/// </summary>
public abstract class DataPage : ComponentBase
{
    [Inject] protected LookupData Data { get; set; } = null!;
    [Inject] protected Icons Icons { get; set; } = null!;

    protected bool Loading { get; private set; } = true;

    /// <summary>The message to show when the data could not be loaded.</summary>
    protected string? Error { get; private set; }

    /// <summary>Runs the page's loads, then marks the page loaded.</summary>
    protected async Task LoadAsync(Func<Task> load)
    {
        try
        {
            await load();
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or InvalidDataException)
        {
            Error = "The data for this tab could not be loaded. Try reloading the page.";
            Console.Error.WriteLine(ex);
        }
        Loading = false;
    }
}
