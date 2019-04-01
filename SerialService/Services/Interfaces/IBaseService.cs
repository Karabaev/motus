namespace SerialService.Services.Interfaces
{
    using System;
    using DAL.Entities;
    using Infrastructure.Core;
    using System.Collections.Generic;

    public interface IBaseService<E> where E: IBaseEntity, new()
    {
        E Get(int? id);

        bool Create(E entity);

        bool Create(IEnumerable<E> entities);

        bool Remove(E entity);

        EntityList<E> GetAll();

        EntityList<E> GetWithCondition(Func<E, bool> predicate);

        E GetScalarWithCondition(Func<E, bool> predicate);

        E GetByMainStringProperty(string value);
    }
}
