using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Loch.Shared.Caching.Extensions;

public static class DistributedCacheExtension
{
    public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recodeId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpirationTime = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(15),
            SlidingExpiration = slidingExpirationTime
        };
        var jsonData = JsonSerializer.Serialize(data);
        await cache.SetStringAsync(recodeId, jsonData, options);
    }
    public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
    {
        var jsonData = await cache.GetStringAsync(recordId);
        return jsonData is null ? default(T) : JsonSerializer.Deserialize<T>(jsonData);
    }
}