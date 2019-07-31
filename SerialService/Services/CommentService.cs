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

    public class CommentService : ICommentService
    {
        public CommentService(ApplicationDbContext context)
        {
            Repository = new CommentRepository(context);
        }

        public IRepository<Comment> Repository { get; set; }

        public bool Create(Comment entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (this.GetAll().Contains(entity))
                throw new EntryAlreadyExistsException();

            bool result = this.Repository.AddEntity(entity);
            return result;
        }

        public bool Create(IEnumerable<Comment> entities)
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

        public Comment Get(int? id)
        {
            if (!id.HasValue)
                throw new ArgumentNullException(nameof(id));

            return this.Repository.GetEntity(id.Value);
        }

        public EntityList<Comment> GetAll()
        {
            return this.Repository.GetAllEntities();
        }

        /// <summary>
        /// Получить один экземпляр объекта по заданному условию.
        /// </summary>
        /// <param name="predicate">Условие выборки.</param>
        public Comment GetScalarWithCondition(Func<Comment, bool> predicate)
        {
            return this.GetAll().FirstOrDefault(predicate);
        }

        public bool Remove(Country entity)
        {
            throw new NotImplementedException();
        }

        public EntityList<Country> GetWithCondition(Func<Country, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Comment entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return this.Repository.RemoveEntity(entity.ID);
        }

        public EntityList<Comment> GetWithCondition(Func<Comment, bool> predicate)
        {
            if(predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return this.GetAll().Where(predicate).ToEntityList<Comment>();
        }

        public bool MarkArchive(Comment entity)
        {
            if (entity == null || !this.GetAll().Contains(entity))
                return false;

            entity.IsArchived = true;
            return this.Repository.SaveChanges();
        }

        public bool UnmarkArchive(Comment entity)
        {
            if (entity == null || !this.GetAll().Contains(entity))
                return false;

            entity.IsArchived = false;
            return this.Repository.SaveChanges();
        }

        public EntityList<Comment> GetAllUnmarkedArchive()
        {
            return this.GetAll().Where(e => e.IsArchived == false).ToEntityList();
        }

        public EntityList<Comment> GetUnmarkedArchiveWithCondition(Func<Comment, bool> predicate)
        {
            return this.GetAllUnmarkedArchive().Where(predicate).ToEntityList();
        }

        public Comment GetUnmarkedArchiveScalarWithCondition(Func<Comment, bool> predicate)
        {
            return this.GetAllUnmarkedArchive().FirstOrDefault(predicate);
        }

        public Comment GetByMainStringProperty(string value)
        {
            throw new NotImplementedException();
        }

        public bool AddVote(int? id, bool isPositiveMark)
        {
            if (!id.HasValue)
                throw new ArgumentNullException(nameof(id));

            Comment comment = this.Get(id);

            if (comment == null)
                throw new EntryNotFoundException();

            if (isPositiveMark)
                comment.PositiveVoteCount++;
            else
                comment.NegativeVoteCount++;

            return this.Repository.SaveChanges();
        }

        public bool RemoveVote(int? id, bool isPositiveMark)
        {
            if (!id.HasValue)
                throw new ArgumentNullException(nameof(id));

            Comment comment = this.Get(id);

            if (comment == null)
                throw new EntryNotFoundException();

            if (isPositiveMark)
                comment.PositiveVoteCount--;
            else
                comment.NegativeVoteCount--;

            return this.Repository.SaveChanges();
        }

        /// <summary>
        /// Переместить 1 отметку из в противоположную.
        /// </summary>
        /// <param name="id">Идентификатор видеоматериала.</param>
        /// <param name="fromPositive">Если true, то будет перемещение из положительных отметок в отрицательную, иначе наоборот.</param>
        /// <returns>true, если успешно, иначе false.</returns>
        public bool InvertVote(int? id, bool fromPositive)
        {
            if (!id.HasValue)
                throw new ArgumentNullException(nameof(id));

            Comment comment = this.Get(id);

            if (comment == null)
                return false;

            if (fromPositive)
            {
                comment.PositiveVoteCount--;
                comment.NegativeVoteCount++;
            }
            else
            {
                comment.PositiveVoteCount++;
                comment.NegativeVoteCount--;
            }

            return this.Repository.SaveChanges();
        }
    }
}