namespace SerialService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Interfaces;
	using DAL.Context;
    using DAL.Entities;
    using DAL.Repository;
    using Infrastructure.Core;
    using Infrastructure.Exceptions;
	

    public class SerialSeasonService : ISerialSeasonService
    {
        public IRepository<SerialSeason> Repository { get; set; }

		public SerialSeasonService(ApplicationDbContext context)
		{
			this.Repository = new SerialSeasonRepository(context);
		}

        public bool Create(SerialSeason entity)
        {
			if (entity == null)
				return false;

			if (this.GetAll().Contains(entity))
				throw new EntryAlreadyExistsException("Такой сезон уже существует в базе");

			return this.Repository.AddEntity(entity);
		}

        public bool Create(IEnumerable<SerialSeason> entities)
        {
            throw new NotImplementedException();
        }

		public bool Update(int oldID, SerialSeason newSeason)
		{
			if(newSeason == null)
				throw new ArgumentNullException("Передан пустой newSeason");

			newSeason.ID = oldID;
			return this.Repository.UpdateEntity(newSeason);
		}

        public SerialSeason Get(int? id)
        {
			if (!id.HasValue)
				throw new ArgumentNullException("Передан пустой id");

			return this.Repository.GetEntity((int)id);
        }

        public SerialSeason Get(int seasonNumber, int videoMaterialId, int translationId)
        {
            return this.Repository.GetAllEntities().FirstOrDefault(ss => ss.SeasonNumber == seasonNumber &&
                                                                        ss.VideoMaterialID == videoMaterialId && 
                                                                        ss.TranslationID == translationId);
        }

        public EntityList<SerialSeason> GetAll()
        {
            return this.Repository.GetAllEntities();
        }

        public EntityList<SerialSeason> GetAllUnmarkedArchive()
        {
            throw new NotImplementedException();
        }

        public SerialSeason GetByMainStringProperty(string value)
        {
            throw new NotImplementedException();
        }

        public SerialSeason GetScalarWithCondition(Func<SerialSeason, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public SerialSeason GetUnmarkedArchiveScalarWithCondition(Func<SerialSeason, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<SerialSeason> GetUnmarkedArchiveWithCondition(Func<SerialSeason, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<SerialSeason> GetUnmarkedArchiveWithConditions(params Func<SerialSeason, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public EntityList<SerialSeason> GetWithCondition(Func<SerialSeason, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<SerialSeason> GetWithConditions(params Func<SerialSeason, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public bool MarkArchive(SerialSeason entity)
        {
            throw new NotImplementedException();
        }

        public bool Remove(SerialSeason entity)
        {
            throw new NotImplementedException();
        }

        public bool UnmarkArchive(SerialSeason entity)
        {
            throw new NotImplementedException();
        }
    }
}