namespace SerialService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DAL.Entities;
    using DAL.Repository;
    using Interfaces;
    using Infrastructure.Exceptions;
    using Infrastructure.Core;
    using DAL.Context;
    using Infrastructure.Core.Extensions;

    public class CommentMarkService : ICommentMarkService
    {
        public CommentMarkService(ApplicationDbContext context)
        {
            Repository = new CommentMarkRepository(context);
        }

        public IRepository<CommentMark> Repository { get; set; }

        public bool Create(CommentMark entity)
        {
            if (entity == null)
                return false;

            CommentMark mark = null;

            if (!string.IsNullOrEmpty(entity.AuthorID))
                mark = this.GetScalarWithCondition((m => m.CommentID == entity.CommentID && m.AuthorID == entity.AuthorID));

            if (mark == null)
                mark = this.GetScalarWithCondition((m => m.CommentID == entity.CommentID && m.UserIP == entity.UserIP));
            else
                throw new EntryAlreadyExistsException("Метка пользователя для этого видеоматериала уже стоит");

            if (mark != null)
                throw new EntryAlreadyExistsException("Метка с этого ip адреса для этого видеоматериала уже стоит");

            return this.Repository.AddEntity(entity);
        }

        public bool Create(IEnumerable<CommentMark> entities)
        {
            if (entities == null)
                return false;

            foreach (var item in entities)
            {
                if (!this.Create(item))
                {
                    throw new OperationAbortedException(string.Format("Произошла ошибка при сохранении объекта сущности {0} в базу", item.GetType().Name));
                }
            }

            return true;
        }

        public CommentMark Get(int? id)
        {
            if (!id.HasValue)
                throw new ArgumentNullException(nameof(id));

            return this.Repository.GetEntity(id.Value);
        }

        public EntityList<CommentMark> GetAll()
        {
            return this.Repository.GetAllEntities();
        }

        public CommentMark GetByMainStringProperty(string value)
        {
            throw new NotImplementedException();
        }

        public CommentMark GetScalarWithCondition(Func<CommentMark, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return this.GetAll().FirstOrDefault(predicate);
        }

        public EntityList<CommentMark> GetWithCondition(Func<CommentMark, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public bool Remove(CommentMark entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return this.Repository.RemoveEntity(entity);
        }

        /// <summary>
        /// Инвертировать значение.
        /// </summary>
        /// <param name="entity">Сущность для изменения.</param>
        /// <returns>true, если успешно, иначе false.</returns>
        public bool InvertValue(CommentMark entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Value = !entity.Value;
            return this.Repository.SaveChanges();
        }
    }
}