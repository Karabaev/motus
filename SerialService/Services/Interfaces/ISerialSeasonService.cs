namespace SerialService.Services.Interfaces
{
    using System;
    using Infrastructure.Core;
    using DAL.Entities;

    public interface ISerialSeasonService : IBaseService<SerialSeason>
    {
        SerialSeason Get(int seasonNumber, int videoMaterialId, int translationId);

        bool Update(int oldID, SerialSeason newSeason);

        bool MarkArchive(SerialSeason entity);

        bool UnmarkArchive(SerialSeason entity);

        EntityList<SerialSeason> GetAllUnmarkedArchive();

        EntityList<SerialSeason> GetUnmarkedArchiveWithCondition(Func<SerialSeason, bool> predicate);

        EntityList<SerialSeason> GetUnmarkedArchiveWithConditions(params Func<SerialSeason, bool>[] predicates);

        SerialSeason GetUnmarkedArchiveScalarWithCondition(Func<SerialSeason, bool> predicate);
    }
}