namespace SerialService.Services.Interfaces
{
    using System;
    using Infrastructure.Core;
    using DAL.Entities;
    using System.Collections.Generic;

    public interface IGenreService: IBaseService<Genre>
    {
        bool MarkArchive(Genre entity);

        bool UnmarkArchive(Genre entity);

        EntityList<Genre> GetAllUnmarkedArchive();

        EntityList<Genre> GetUnmarkedArchiveWithCondition(Func<Genre, bool> predicate);

        EntityList<Genre> GetUnmarkedArchiveWithConditions(params Func<Genre, bool>[] predicates);

        Genre GetUnmarkedArchiveScalarWithCondition(Func<Genre, bool> predicate);

        List<Genre> AutoSave(IEnumerable<string> fullNameContainer);
    }
}
