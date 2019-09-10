namespace SerialService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DAL.Entities;
    using DAL.Repository;
    using Interfaces;
    using Infrastructure.Exceptions;
    using Infrastructure;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using DAL.Context;
    using SerialService.Models;
    using AutoMapper;

    public class VideoMaterialViewsByUsersService : IVideoMaterialViewsByUsersService
    {
        public VideoMaterialViewsByUsersRepository Repository { get; set; }

        public VideoMaterialViewsByUsersService(IDbContext context)
        {
            Repository = new VideoMaterialViewsByUsersRepository(context);
        }

        public VideoMaterialViewsByUsers Get(int? id)
        {
            if (!id.HasValue)
                return null;

            return this.Repository.GetEntity(id.Value);
        }

        public bool Create(VideoMaterialViewsByUsers entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return this.Repository.AddEntity(entity);
        }

        public bool Create(IEnumerable<VideoMaterialViewsByUsers> entities)
        {
            throw new NotImplementedException();
        }

        public bool Remove(VideoMaterialViewsByUsers entity)
        {
            if (entity == null)
                return false;

            return this.Repository.RemoveEntity(entity);
        }

        public EntityList<VideoMaterialViewsByUsers> GetAll()
        {
            return this.Repository.GetAllEntities();
        }

        public EntityList<VideoMaterialViewsByUsers> GetWithCondition(Func<VideoMaterialViewsByUsers, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public VideoMaterialViewsByUsers GetScalarWithCondition(Func<VideoMaterialViewsByUsers, bool> predicate)
        {
            return this.Repository.GetAllEntities().FirstOrDefault(predicate);
        }

        public VideoMaterialViewsByUsers GetByMainStringProperty(string value)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntity(VideoMaterialViewsByUsers entity)
        {
            return this.Repository.UpdateEntity(entity);
        }
    }
}