using System.Text.Json;

namespace NorthwindStore.Helpers;

public static class SessionExtensions
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static T? GetObject<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value, Options);
    }

    public static void SetObject<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value, Options));
    }
}
