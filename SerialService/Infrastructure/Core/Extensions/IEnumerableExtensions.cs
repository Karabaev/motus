namespace SerialService.Infrastructure.Core.Extensions
{
    using DAL.Entities;
    using System.Collections.Generic;
    public static class IEnumerableExtensions 
    {
        public static EntityList<T> ToEntityList<T>(this IEnumerable<T> collection) where T : class, IBaseEntity
        {
            return new EntityList<T>(collection);
        }
    }
}