namespace SerialService.Services.Interfaces
{
    using System;
    using Infrastructure.Core;
    using DAL.Entities;
    using System.Collections.Generic;

    public interface IPersonService: IBaseService<Person>
    {
        bool MarkArchive(Person entity);

        bool UnmarkArchive(Person entity);

        EntityList<Person> GetAllUnmarkedArchive();

        EntityList<Person> GetUnmarkedArchiveWithCondition(Func<Person, bool> predicate);

        EntityList<Person> GetUnmarkedArchiveWithConditions(params Func<Person, bool>[] predicates);

        Person GetUnmarkedArchiveScalarWithCondition(Func<Person, bool> predicate);

        List<Person> AutoSave(IEnumerable<string> fullNameContainer);
    }
}
