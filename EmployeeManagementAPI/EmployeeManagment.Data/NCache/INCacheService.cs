using Alachisoft.NCache.Runtime.Caching;

namespace EmployeeManagment.Services.NCache
{
    public interface INCacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, ExpirationType expirationType, TimeSpan expireAfter = default(TimeSpan));
        bool RemoveData(string key);
    }
}
