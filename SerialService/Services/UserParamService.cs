namespace SerialService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using DAL.Context;
    using DAL.Entities;
    using DAL.Repository;
    using Infrastructure.Exceptions;
    using Infrastructure.Core;

    public class UserParamService : IUserParamService
    {
        public IRepository<UserParam> Repository { get; set; }

        public UserParamService(IDbContext context)
        {
            this.Repository = new UserParamRepository(context);
        }

        public UserParam Get(string name, string userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return this.Repository.GetAllEntities().FirstOrDefault(p => p.Name == name && p.UserID == userId);
        }

        public UserParam Get(int? id)
        {
            if (!id.HasValue)
                throw new ArgumentNullException(nameof(id));

            return this.Repository.GetEntity(id.Value);
        }

        public bool Create(UserParam entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            bool result = this.Repository.AddEntity(entity);
            return result;
        }

        public bool Create(IEnumerable<UserParam> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            bool result = true;

            foreach (var item in entities)
                result = result && this.Create(item);

            return result;
        }

        public bool Remove(UserParam entity)
        {
            throw new NotImplementedException();
        }

        public EntityList<UserParam> GetAll()
        {
            return this.Repository.GetAllEntities();
        }

        public EntityList<UserParam> GetWithCondition(Func<UserParam, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public UserParam GetScalarWithCondition(Func<UserParam, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public UserParam GetByMainStringProperty(string value)
        {
            throw new NotImplementedException();
        }
    }
}