using System;
using System.Reflection;
using StackExchange.Redis;

namespace OAuth_Workspace
{
    public class RedisObjectStore<T> : IRedisObjectStore<T>
    {
        private ConnectionMultiplexer _connection;
        private readonly IDatabase _db;
        private int DatabaseIndex = 0;
        private Type TypeOfT => typeof(T);
        private PropertyInfo[] PropertiesOfT => TypeOfT.GetProperties();


        public RedisObjectStore(StoreType type)
        {
            _connection = RedisConnectionFactory.GetConnection();
            _db = _connection.GetDatabase((int) type);
        }

        public T Get(string key)
        {
            var hash = _db.HashGetAll(key);

            return GetObjectFromHash(hash);
        }

        public void Save(string key, T obj)
        {
            if (obj == null) return;
            var hash = CreateRedisHash(obj);

            _db.HashSet(key, hash);
        }

        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace((key)))
            {
                throw new ArgumentException("invalid key");
            }

            _db.KeyDelete(key);
        }

        public void Expire(string key, TimeSpan expiryTime)
        {
            _db.KeyExpire(key, expiryTime);
        }

        private HashEntry[] CreateRedisHash(T obj)
        {
            var properties = PropertiesOfT;
            var hash = new HashEntry[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                hash[i] = new HashEntry(properties[i].Name, properties[i].GetValue(obj).ToString());
            }

            return hash;
        }

        private T GetObjectFromHash(HashEntry[] hash)
        {
            var obj = (T) Activator.CreateInstance(TypeOfT);
            var props = PropertiesOfT;

            foreach (PropertyInfo propertyInfo in props)
            foreach (HashEntry hashEntry in hash)
                if (propertyInfo.Name == hashEntry.Name)
                {
                    var val = hashEntry.Value;
                    var type = propertyInfo.PropertyType;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        if (string.IsNullOrEmpty(val))
                            propertyInfo.SetValue(obj, null);

                    propertyInfo.SetValue(obj, Convert.ChangeType(val, type));
                }

            return obj;
        }
    }
}