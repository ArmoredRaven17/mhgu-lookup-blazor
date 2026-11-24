using MhguLookup.Core.Models;

namespace MhguLookup.Web.Components;

/// <summary>Short descriptions used by the Canteen tab.</summary>
public static class CanteenText
{
    /// <summary>A one-line hint of how an ingredient is unlocked, for the list.</summary>
    public static string UnlockHint(IngredientUnlock u) => u switch
    {
        StartIngredient => "From the start",
        QuestIngredient q => q.Title,
        RequestIngredient r => $"{r.Npc}'s Villager Request",
        TalkIngredient t => $"Talk to the {t.Npc}",
        _ => "",
    };

    /// <summary>The chance each of a sauce's skills activates.</summary>
    public static string Odds(Canteen data) =>
        "Chance each skill activates, by its place in the sauce: "
        + string.Join(" · ", data.Odds.Select((o, i) => $"{new[] { "1st", "2nd", "3rd" }[i]} {o.Normal}% (chef fired up {o.Fired}%)"))
        + ". A Gourmet Voucher activates all three.";
}
