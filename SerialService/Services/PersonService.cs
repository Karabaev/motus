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
    using System.Collections;

    public class PersonService : IPersonService
    {
        public PersonService(ApplicationDbContext context)
        {
            Repository = new PersonRepository(context);
        }
        public IRepository<Person> Repository { get; set; }

        /// <summary>
        /// Добавить одну запись в базу. Перед записью идет проверка на дублирование главного строкового свойства(FullName).
        /// </summary>
        /// <param name="entity">Объект для записи.</param>
        public bool Create(Person entity)
        {
            if (entity == null)
                return false;

            if (this.GetAll().Contains(entity))
                return false;

            return this.Repository.AddEntity(entity);
        }

        /// <summary>
        /// Добавить контейнер записей в базу. Перед записью идет проверка на дублирование главного строкового свойства(FullName).
        /// </summary>
        /// <param name="entities">Контейнер объектов для записи.</param>
        public bool Create(IEnumerable<Person> entities)
        {
            if (entities == null)
                return false;

            foreach (var item in entities)
            {
                if(!this.Create(item))
                {
                    return false;
                }
            }

            return true;
        }

        public Person Get(int? id)
        {
            throw new NotImplementedException();
        }

        public EntityList<Person> GetAll()
        {
            return Repository.GetAllEntities();
        }

        public EntityList<Person> GetWithCondition(Func<Person, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Person> GetWithConditions(params Func<VideoMaterial, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public EntityList<Person> GetWithConditions(params Func<Person, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public bool MarkArchive(Person entity)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Person entity)
        {
            throw new NotImplementedException();
        }

        public Person SearchByStringPropValue(string nameOfPropertie, string value)
        {
            throw new NotImplementedException();
        }

        public EntityList<Person> SerchByStringPropPart(string nameOfPropertie, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить один экземпляр объекта по заданному условию.
        /// </summary>
        /// <param name="predicate">Условие выборки.</param>
        public Person GetScalarWithCondition(Func<Person, bool> predicate)
        {
            return this.GetAll().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Получить один экземпляр объекта по главному строковому полю (FullName).
        /// </summary>
        /// <param name="value">Значение для поиска.</param>
        public Person GetByMainStringProperty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return this.GetScalarWithCondition(vm => vm.FullName.ToLower() == value.ToLower());
        }

        public bool UnmarkArchive(Person entity)
        {
            throw new NotImplementedException();
        }

        public EntityList<Person> GetAllUnmarkedArchive()
        {
            throw new NotImplementedException();
        }

        public EntityList<Person> GetUnmarkedArchiveWithCondition(Func<Person, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public EntityList<Person> GetUnmarkedArchiveWithConditions(params Func<Person, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public Person GetUnmarkedArchiveScalarWithCondition(Func<Person, bool> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Создает и записывает объекты личностей с именами (FullName) из контейнера имен. Возвращает контейнер личностей из базы.
        /// </summary>
        /// <param name="fullNameContainer">Контейнер с именами.</param>
        public List<Person> AutoSave(IEnumerable<string> fullNameContainer)
        {
            List<Person> result = new List<Person>();
            if (fullNameContainer == null)
                return null;

            foreach (var item in fullNameContainer)
            {
                Person person = this.GetByMainStringProperty(item);

                if (person == null)
                {
                    result.Add(new Person { FullName = item, IsArchived = false });
                }
                else
                {
                    result.Add(person);
                }
            }
            return result;
        }
    }
}