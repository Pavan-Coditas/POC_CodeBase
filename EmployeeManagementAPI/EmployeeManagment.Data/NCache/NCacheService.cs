using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime;
using Alachisoft.NCache.Runtime.Caching;
namespace EmployeeManagment.Services.NCache
{
    public class NCacheService : INCacheService
    {
        private readonly ICache _cache;

        public NCacheService(ICache cache)
        {
            _cache = cache;
        }

        public T GetData<T>(string key)
        {
            var value = _cache.Get<T>(key);
            return value!;
        }

        public bool SetData<T>(string key, T value, ExpirationType expirationType, TimeSpan expireAfter = default(TimeSpan))
        {
            var cacheItem = new CacheItem(value)
            {
                Expiration = new Expiration(expirationType, expireAfter),
                Priority = CacheItemPriority.Default
            };
            _cache.Insert(key, cacheItem);
            return true;
        }

        public bool RemoveData(string key)
        {
            var isKeyExist = _cache.Contains(key);
            if (isKeyExist)
            {
                _cache.Remove(key);
                return true;
            }
            return false;
        }
    }
}
