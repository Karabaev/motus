using Newtonsoft.Json;
using SerialService.DAL.Entities;

namespace SerialService.Infrastructure
{
    public class Cloner
    {
        public static IBaseEntity Clone<T>(IBaseEntity source) where T : IBaseEntity
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}