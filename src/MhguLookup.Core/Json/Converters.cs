using System.Text.Json;
using System.Text.Json.Serialization;
using MhguLookup.Core.Models;

namespace MhguLookup.Core.Json;

// Converters for the data files' positional rows. The files are only ever read, so writing is not supported.

/// <summary>Base for a read-only converter.</summary>
public abstract class ReadOnlyConverter<T> : JsonConverter<T>
{
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{typeof(T).Name} is read from the data files and never written.");
}

/// <summary><c>[name, quantity]</c></summary>
public sealed class AmountConverter : ReadOnlyConverter<Amount>
{
    public override Amount Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        Positional.Start(ref r);
        var a = new Amount(Positional.String(ref r), Positional.Int(ref r));
        Positional.End(ref r);
        return a;
    }
}

/// <summary><c>[item, quantity]</c>, or <c>[category, points, "pts"]</c> for a material-point cost.</summary>
public sealed class MaterialConverter : ReadOnlyConverter<Material>
{
    public override Material Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        Positional.Start(ref r);
        var name = Positional.String(ref r);
        var qty = Positional.Int(ref r);
        if (!Positional.TryNext(ref r)) return new Material(name, qty, false);
        var points = r.GetString() == "pts";
        Positional.End(ref r);
        return new Material(name, qty, points);
    }
}

/// <summary><c>[item, quantity, percent]</c></summary>
public sealed class ChanceItemConverter : ReadOnlyConverter<ChanceItem>
{
    public override ChanceItem Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        Positional.Start(ref r);
        var c = new ChanceItem(Positional.String(ref r), Positional.Int(ref r), Positional.Int(ref r));
        Positional.End(ref r);
        return c;
    }
}

/// <summary><c>[item id, item name, quantity, percent]</c></summary>
public sealed class RewardRowConverter : ReadOnlyConverter<RewardRow>
{
    public override RewardRow Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        Positional.Start(ref r);
        var row = new RewardRow(Positional.Int(ref r), Positional.String(ref r), Positional.Int(ref r), Positional.Int(ref r));
        Positional.End(ref r);
        return row;
    }
}

/// <summary><c>[ammo, total, clip]</c></summary>
public sealed class InternalAmmoConverter : ReadOnlyConverter<InternalAmmo>
{
    public override InternalAmmo Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        Positional.Start(ref r);
        var a = new InternalAmmo(Positional.String(ref r), Positional.Int(ref r), Positional.Int(ref r));
        Positional.End(ref r);
        return a;
    }
}

/// <summary><c>[ammo, shots, power %, wait]</c></summary>
public sealed class RapidFireConverter : ReadOnlyConverter<RapidFire>
{
    public override RapidFire Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        Positional.Start(ref r);
        var a = new RapidFire(Positional.String(ref r), Positional.Int(ref r), Positional.Int(ref r), Positional.String(ref r));
        Positional.End(ref r);
        return a;
    }
}

/// <summary><c>[status, initial, step, max]</c></summary>
public sealed class StatusStatConverter : ReadOnlyConverter<StatusStat>
{
    public override StatusStat Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        Positional.Start(ref r);
        var s = new StatusStat(Positional.String(ref r), Positional.Double(ref r), Positional.Double(ref r), Positional.Double(ref r));
        Positional.End(ref r);
        return s;
    }
}

/// <summary><c>[kind, where, detail, percent or null]</c></summary>
public sealed class ItemSourceConverter : ReadOnlyConverter<ItemSource>
{
    public override ItemSource Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        Positional.Start(ref r);
        var s = new ItemSource(Positional.String(ref r), Positional.String(ref r), Positional.String(ref r), Positional.NullableInt(ref r));
        Positional.End(ref r);
        return s;
    }
}

/// <summary><c>[monster id, name, stats in this quest]</c>; the stats may be missing or null.</summary>
public sealed class QuestMonsterConverter : ReadOnlyConverter<QuestMonster>
{
    public override QuestMonster Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        Positional.Start(ref r);
        var id = Positional.String(ref r);
        var name = Positional.String(ref r);
        if (!Positional.TryNext(ref r)) return new QuestMonster(id, name, null);
        var stats = r.TokenType == JsonTokenType.Null ? null : JsonSerializer.Deserialize(ref r, LookupJsonContext.Default.QuestStats);
        Positional.End(ref r);
        return new QuestMonster(id, name, stats);
    }
}
