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

    public class CountryService : ICountryService
    {
        public CountryService(ApplicationDbContext context)
        {
            Repository = new CountryRepository(context);
        }

        public IRepository<Country> Repository { get; set; }

        public bool Create(Country entity)
        {
            if (entity == null)
                return false;

            if (this.GetAll().Contains(entity))
                return false;

            return this.Repository.AddEntity(entity);
        }

        public bool Create(IEnumerable<Country> entities)
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

        public Country Get(int? id)
        {
            throw new NotImplementedException();
        }

        public EntityList<Country> GetAll()
        {
            return this.Repository.GetAllEntities();
        }

        /// <summary>
        /// Получить один экземпляр объекта по заданному условию.
        /// </summary>
        /// <param name="predicate">Условие выборки.</param>
        public Country GetScalarWithCondition(Func<Country, bool> predicate)
        {
            return this.GetAll().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Получить один экземпляр объекта по главному строковому полю (Name).
        /// </summary>
        /// <param name="value">Значение для поиска.</param>
        public Country GetByMainStringProperty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return this.GetScalarWithCondition(vm => vm.Name.ToLower() == value.ToLower());
        }

        /// <summary>
        /// Создает и записывает объекты стран с именами (Name) из контейнера имен. Возвращает контейнер стран из базы.
        /// </summary>
        /// <param name="fullNameContainer">Контейнер с именами.</param>
        public List<Country> AutoSave(IEnumerable<string> fullNameContainer)
        {
            List<Country> result = new List<Country>();
            if (fullNameContainer == null)
            {
                return null;
            }
            foreach (var item in fullNameContainer)
            {
                Country country = this.GetByMainStringProperty(item);

                if (country == null)
                {
                    result.Add(new Country { Name = item, IsArchived = false });
                }
                else
                {
                    result.Add(country);
                }
            }
            return result;
        }

        public bool MarkArchive(Country entity)
        {
            throw new NotImplementedException();
        }

        public bool UnmarkArchive(Country entity)
        {
            throw new NotImplementedException();
        }

        public EntityList<Country> GetAllUnmarkedArchive()
        {
            throw new NotImplementedException();
        }

        public EntityList<Country> GetUnmarkedArchiveWithCondition(Func<Country, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Country> GetUnmarkedArchiveWithConditions(params Func<Country, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public Country GetUnmarkedArchiveScalarWithCondition(Func<Country, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Country entity)
        {
            throw new NotImplementedException();
        }

        public EntityList<Country> GetWithCondition(Func<Country, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}