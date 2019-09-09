namespace SerialService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using DAL.Entities;
    using DAL.Repository;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using DAL.Context;

    public class PictureService : IPictureService
    {
        public IRepository<Picture> Repository { get; set; }

        public PictureService(IDbContext context)
        {
            Repository = new PictureRepository(context);
        }

        public bool Create(Picture entity)
        {
            if (entity == null || this.GetByMainStringProperty(entity.URL) != null)
                return false;

            return this.Repository.AddEntity(entity);
        }

        public bool Create(IEnumerable<Picture> entities)
        {
            throw new NotImplementedException();
        }

        public Picture Get(int? id)
        {
            throw new NotImplementedException();
        }

        public EntityList<Picture> GetAll()
        {
            return Repository.GetAllEntities();
        }

        public EntityList<Picture> GetAllUnmarkedArchive()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить один экземпляр объекта по заданному условию.
        /// </summary>
        /// <param name="predicate">Условие выборки.</param>
        public Picture GetScalarWithCondition(Func<Picture, bool> predicate)
        {
            return this.GetAll().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Получить один экземпляр объекта по главному строковому полю (Url).
        /// </summary>
        /// <param name="value">Значение для поиска.</param>
        public Picture GetByMainStringProperty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return this.GetScalarWithCondition(vm => vm.URL.ToLower() == value.ToLower());
        }

        public Picture GetUnmarkedArchiveScalarWithCondition(Func<Picture, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Picture> GetUnmarkedArchiveWithCondition(Func<Picture, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Picture> GetUnmarkedArchiveWithConditions(params Func<Picture, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public EntityList<Picture> GetWithCondition(Func<Picture, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Picture> GetWithConditions(params Func<Picture, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public bool MarkArchive(Picture entity)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Picture entity)
        {
            throw new NotImplementedException();
        }

        public bool UnmarkArchive(Picture entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Создает и записывает объекты настроений с именами (Name) из контейнера имен. Возвращает контейнер настроений из базы.
        /// </summary>
        /// <param name="fullNameContainer">Контейнер с именами.</param>
        public List<Picture> AutoSave(IEnumerable<string> fullNameContainer)
        {
            List<Picture> result = new List<Picture>();

            if (fullNameContainer == null)
                return result;

            foreach (var item in fullNameContainer)
            {
                Picture picture = this.GetByMainStringProperty(item);
                if (picture == null)
                {
                    result.Add(new Picture { URL = item, IsArchived = false });
                }
                else
                {
                    result.Add(picture);
                }
            }
            return result;
        }
    }
}