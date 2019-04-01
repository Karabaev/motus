namespace SerialService.Services.Interfaces
{
    using System;
    using Infrastructure.Core;
    using DAL.Entities;
    using System.Collections.Generic;

    public interface ICountryService: IBaseService<Country>
    {
        bool MarkArchive(Country entity);

        bool UnmarkArchive(Country entity);

        EntityList<Country> GetAllUnmarkedArchive();

        EntityList<Country> GetUnmarkedArchiveWithCondition(Func<Country, bool> predicate);

        EntityList<Country> GetUnmarkedArchiveWithConditions(params Func<Country, bool>[] predicates);

        Country GetUnmarkedArchiveScalarWithCondition(Func<Country, bool> predicate);

        List<Country> AutoSave(IEnumerable<string> fullNameContainer);
    }
}
