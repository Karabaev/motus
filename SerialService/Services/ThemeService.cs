namespace SerialService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DAL.Entities;
    using DAL.Repository;
    using Interfaces;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using DAL.Context;

    public class ThemeService : IThemeService
    {
        public IRepository<Theme> Repository { get; set; }

        public ThemeService(ApplicationDbContext context)
        {
            Repository = new ThemeRepository(context);
        }

        public bool Create(Theme entity)
        {
            if (entity == null)
                return false;

            if (this.GetAll().Contains(entity))
                return false;

            return this.Repository.AddEntity(entity);
        }

        public bool Create(IEnumerable<Theme> entities)
        {
            throw new NotImplementedException();
        }

        public Theme Get(int? id)
        {
            throw new NotImplementedException();
        }

        public EntityList<Theme> GetAll()
        {
            return Repository.GetAllEntities();
        }

        public EntityList<Theme> GetWithCondition(Func<Theme, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Theme> GetWithConditions(params Func<VideoMaterial, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public EntityList<Theme> GetWithConditions(params Func<Theme, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public bool MarkArchive(Theme entity)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Theme entity)
        {
            throw new NotImplementedException();
        }

        public Theme SearchByStringPropValue(string nameOfPropertie, string value)
        {
            throw new NotImplementedException();
        }

        public EntityList<Theme> SerchByStringPropPart(string nameOfPropertie, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить один экземпляр объекта по заданному условию.
        /// </summary>
        /// <param name="predicate">Условие выборки.</param>
        public Theme GetScalarWithCondition(Func<Theme, bool> predicate)
        {
            return this.GetAll().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Получить один экземпляр объекта по главному строковому полю (Name).
        /// </summary>
        /// <param name="value">Значение для поиска.</param>
        public Theme GetByMainStringProperty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return this.GetScalarWithCondition(vm => vm.Name.ToLower() == value.ToLower());
        }

        public bool UnmarkArchive(Theme entity)
        {
            throw new NotImplementedException();
        }

        public EntityList<Theme> GetAllUnmarkedArchive()
        {
            throw new NotImplementedException();
        }

        public EntityList<Theme> GetUnmarkedArchiveWithCondition(Func<Theme, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Theme> GetUnmarkedArchiveWithConditions(params Func<Theme, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public Theme GetUnmarkedArchiveScalarWithCondition(Func<Theme, bool> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Создает и записывает объекты настроений с именами (Name) из контейнера имен. Возвращает контейнер настроений из базы.
        /// </summary>
        /// <param name="fullNameContainer">Контейнер с именами.</param>
        public List<Theme> AutoSave(IEnumerable<string> fullNameContainer)
        {
            List<Theme> result = new List<Theme>();
            if (fullNameContainer == null)
                return null;

            foreach (var item in fullNameContainer)
            {
                Theme theme = this.GetByMainStringProperty(item);

                if (theme == null)
                {
                    result.Add(new Theme { Name = item, IsArchived = false });
                }
                else
                {
                    result.Add(theme);
                }
            }
            return result;
        }
    }
}