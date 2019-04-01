namespace SerialService.Services.Interfaces
{
    using System;
    using Infrastructure.Core;
    using DAL.Entities;
    using System.Collections.Generic;

    public interface IThemeService: IBaseService<Theme>
    {
        bool MarkArchive(Theme entity);

        bool UnmarkArchive(Theme entity);

        EntityList<Theme> GetAllUnmarkedArchive();

        EntityList<Theme> GetUnmarkedArchiveWithCondition(Func<Theme, bool> predicate);

        EntityList<Theme> GetUnmarkedArchiveWithConditions(params Func<Theme, bool>[] predicates);

        Theme GetUnmarkedArchiveScalarWithCondition(Func<Theme, bool> predicate);

        List<Theme> AutoSave(IEnumerable<string> fullNameContainer);
    }
}
