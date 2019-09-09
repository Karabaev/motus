namespace SerialService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using DAL.Entities;
    using DAL.Repository;
    using Infrastructure.Core;
    using DAL.Context;

    public class TranslationService : ITranslationService
    {
        public IRepository<Translation> Repository { get; set; }

        public TranslationService(IDbContext context)
        {
            Repository = new TranslationRepository(context);
        }

        public bool Create(Translation entity)
        {
            if (entity == null)
                return false;

            if (this.GetAll().Contains(entity))
                return false;

            return this.Repository.AddEntity(entity);
        }

        public bool Create(IEnumerable<Translation> entities)
        {
            throw new NotImplementedException();
        }

        public Translation Get(int? id)
        {
            throw new NotImplementedException();
        }

        public EntityList<Translation> GetAll()
        {
            return this.Repository.GetAllEntities();
        }

        public EntityList<Translation> GetAllUnmarkedArchive()
        {
            throw new NotImplementedException();
        }

        public Translation GetByMainStringProperty(string value)
        {
            return this.GetScalarWithCondition(vm => vm.Name.ToLower() == value.ToLower());
        }

        public Translation GetScalarWithCondition(Func<Translation, bool> predicate)
        {
			var result = this.Repository.GetAllEntities().FirstOrDefault(predicate);
			return result;
		}

        public Translation GetUnmarkedArchiveScalarWithCondition(Func<Translation, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Translation> GetUnmarkedArchiveWithCondition(Func<Translation, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Translation> GetUnmarkedArchiveWithConditions(params Func<Translation, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public EntityList<Translation> GetWithCondition(Func<Translation, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Translation> GetWithConditions(params Func<Translation, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public bool MarkArchive(Translation entity)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Translation entity)
        {
            throw new NotImplementedException();
        }

        public bool UnmarkArchive(Translation entity)
        {
            throw new NotImplementedException();
        }
    }
}