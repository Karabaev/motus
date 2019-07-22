﻿namespace SerialService.Services
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

            if (this.GetAll().Contains(entity))
                return false;

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

            return this.Repository.GetEntity(id);
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
            throw new NotImplementedException();
        }
    }