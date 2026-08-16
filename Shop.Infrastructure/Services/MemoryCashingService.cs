
using Microsoft.Extensions.Caching.Memory;
using Shop.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Services;

public class MemoryCachingService : ICachingService
{
    private readonly IMemoryCache _memoryCache;
    public MemoryCachingService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public Task<T?> GetAsync<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T value))
        {
            return Task.FromResult<T?>(value);
        }

        return Task.FromResult<T?>(default);
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? exp)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = exp ?? TimeSpan.FromMinutes(15)
        };

        _memoryCache.Set(key, value, options);
        return Task.CompletedTask;
    }
}