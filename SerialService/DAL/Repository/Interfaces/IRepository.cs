namespace SerialService.DAL.Repository
{
    using System.Collections.Generic;
    using Entities;
    using Infrastructure.Core;

    public interface IRepository<T> where T : IBaseEntity
    {
        EntityList<T> GetAllEntities();
        T GetEntity(int id);
        bool AddEntity(T entity);
        bool UpdateEntity(T entity);
        bool RemoveEntity(int id);
        bool RemoveEntity(T entity);
        bool SaveChanges();
    }
}
