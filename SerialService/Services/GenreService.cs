namespace SerialService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DAL.Entities;
    using DAL.Repository;
    using Interfaces;
    using Infrastructure.Core;
    using DAL.Context;

    public class GenreService : IGenreService
    {
        public GenreService(ApplicationDbContext context)
        {
            Repository = new GenreRepository(context);
        }
        public IRepository<Genre> Repository { get; set; }

        public bool Create(Genre entity)
        {
            if (entity == null)
                return false;

            if (this.GetAll().Contains(entity))
                return false;

            return this.Repository.AddEntity(entity);
        }

        public bool Create(IEnumerable<Genre> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            bool result = true;

            foreach (var item in entities)
                result = result && this.Create(item);

            return result;
        }

        public Genre Get(int? id)
        {
            throw new NotImplementedException();
        }

        public EntityList<Genre> GetAll()
        {
            return Repository.GetAllEntities();
        }

        public EntityList<Genre> GetWithCondition(Func<Genre, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Genre> GetWithConditions(params Func<VideoMaterial, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public EntityList<Genre> GetWithConditions(params Func<Genre, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public bool MarkArchive(Genre entity)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Genre entity)
        {
            throw new NotImplementedException();
        }

        public Genre SearchByStringPropValue(string nameOfPropertie, string value)
        {
            throw new NotImplementedException();
        }

        public EntityList<Genre> SerchByStringPropPart(string nameOfPropertie, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить один экземпляр объекта по заданному условию.
        /// </summary>
        /// <param name="predicate">Условие выборки.</param>
        public Genre GetScalarWithCondition(Func<Genre, bool> predicate)
        {
            return this.GetAll().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Получить один экземпляр объекта по главному строковому полю (Name).
        /// </summary>
        /// <param name="value">Значение для поиска.</param>
        public Genre GetByMainStringProperty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return this.GetScalarWithCondition(vm => vm.Name.ToLower() == value.ToLower());
        }

        public bool UnmarkArchive(Genre entity)
        {
            throw new NotImplementedException();
        }

        public EntityList<Genre> GetAllUnmarkedArchive()
        {
            throw new NotImplementedException();
        }

        public EntityList<Genre> GetUnmarkedArchiveWithCondition(Func<Genre, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Genre> GetUnmarkedArchiveWithConditions(params Func<Genre, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public Genre GetUnmarkedArchiveScalarWithCondition(Func<Genre, bool> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Создает и записывает объекты жанров с именами (Name) из контейнера имен. Возвращает контейнер жанров из базы.
        /// </summary>
        /// <param name="fullNameContainer">Контейнер с именами.</param>
        public List<Genre> AutoSave(IEnumerable<string> fullNameContainer)
        {
            List<Genre> result = new List<Genre>();
            if (fullNameContainer == null)
                return null;

            foreach (var item in fullNameContainer)
            {
                Genre genre = this.GetByMainStringProperty(item);

                if (genre == null)
                {
                    result.Add(new Genre { Name = item, IsArchived = false });
                }
                else
                {
                    result.Add(genre);
                }
            }
            return result;
        }
    }
}