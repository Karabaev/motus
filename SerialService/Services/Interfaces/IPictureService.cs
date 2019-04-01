namespace SerialService.Services.Interfaces
{
    using System;
    using Infrastructure.Core;
    using DAL.Entities;
    using System.Collections.Generic;

    public interface IPictureService : IBaseService<Picture>
    {
        bool MarkArchive(Picture entity);

        bool UnmarkArchive(Picture entity);

        EntityList<Picture> GetAllUnmarkedArchive();

        EntityList<Picture> GetUnmarkedArchiveWithCondition(Func<Picture, bool> predicate);

        EntityList<Picture> GetUnmarkedArchiveWithConditions(params Func<Picture, bool>[] predicates);

        Picture GetUnmarkedArchiveScalarWithCondition(Func<Picture, bool> predicate);

        List<Picture> AutoSave(IEnumerable<string> fullNameContainer);
    }
}