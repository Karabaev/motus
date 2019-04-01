namespace SerialService.Infrastructure.Core
{
    using System.Runtime.Caching;

    public static class GlobalCache
    {
        private static MemoryCache _cache = new MemoryCache("ExampleCache");
        
        public static object GetItem(string key) {
            return _cache.Get(key);
        }
        
        public static void AddOrGetExisting(string key, object value)
        {
            var oldValue = _cache.AddOrGetExisting(key, value, new CacheItemPolicy());       
        }
    }
}