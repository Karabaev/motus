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

    public class VideoMarkService : IVideoMarkService
    {
        public IRepository<VideoMark> Repository { get; set; }

        public VideoMarkService(ApplicationDbContext context)
        {
            this.Repository = new VideoMarkRepository(context);
        }

        public bool Create(VideoMark entity)
        {
            if (entity == null)
                return false;

            VideoMark mark = null; 

            if(!string.IsNullOrEmpty(entity.AuthorID))
                mark = this.GetScalarWithCondition((m => m.VideoMaterialID == entity.VideoMaterialID && m.AuthorID == entity.AuthorID));

            if(mark == null)
                mark = this.GetScalarWithCondition((m => m.VideoMaterialID == entity.VideoMaterialID && m.UserIP == entity.UserIP));
            else
                throw new EntryAlreadyExistsException("Метка пользователя для этого видеоматериала уже стоит");

            if (mark != null)
                throw new EntryAlreadyExistsException("Метка с этого ip адреса для этого видеоматериала уже стоит");

            return this.Repository.AddEntity(entity);
        }

        public bool Create(IEnumerable<VideoMark> entities)
        {
            throw new NotImplementedException();
        }

        public VideoMark Get(int? id)
        {
            throw new NotImplementedException();
        }

        public EntityList<VideoMark> GetAll()
        {
            return this.Repository.GetAllEntities();
        }


        public VideoMark GetByMainStringProperty(string value)
        {
            throw new NotImplementedException();
        }

        public VideoMark GetScalarWithCondition(Func<VideoMark, bool> predicate)
        {
            if (predicate == null)
                return null;

            return this.GetAll().FirstOrDefault(predicate);
        }

        public EntityList<VideoMark> GetWithCondition(Func<VideoMark, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<VideoMark> GetWithConditions(params Func<VideoMark, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public bool Remove(VideoMark entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Инвертировать значение.
        /// </summary>
        /// <param name="entity">Сущность для изменения.</param>
        /// <returns>true, если успешно, иначе false.</returns>
        public bool InverValue(VideoMark entity)
        {
            if (entity == null)
                return false;

            entity.Value = !entity.Value;
            return this.Repository.SaveChanges();
        }
    }
}