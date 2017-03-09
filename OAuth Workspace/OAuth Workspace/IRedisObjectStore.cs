using System;

namespace OAuth_Workspace
{
    public interface IRedisObjectStore<T>
    {
        T Get(string key);

        void Save(string key, T obj);

        void Delete(string key);

        void Expire(string key, TimeSpan expiryTime);
    }
}