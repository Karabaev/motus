namespace SerialService.Services.Interfaces
{
    using System;
    using Infrastructure.Core;
    using DAL.Entities;

    public interface ITranslationService : IBaseService<Translation>
    {
        bool MarkArchive(Translation entity);

        bool UnmarkArchive(Translation entity);

        EntityList<Translation> GetAllUnmarkedArchive();

        EntityList<Translation> GetUnmarkedArchiveWithCondition(Func<Translation, bool> predicate);

        EntityList<Translation> GetUnmarkedArchiveWithConditions(params Func<Translation, bool>[] predicates);

        Translation GetUnmarkedArchiveScalarWithCondition(Func<Translation, bool> predicate);
    }
}