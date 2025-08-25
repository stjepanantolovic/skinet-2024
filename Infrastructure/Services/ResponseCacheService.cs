using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services
{
    public class ResponseCacheService(IConnectionMultiplexer redis) : IResponseCacheService
    {
        public Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetcahedResponseAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public Task removeCacheByPattern(string pattern)
        {
            throw new NotImplementedException();
        }
    }
}