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

    public class VideoMaterialService : IVideoMaterialService
    {
        public IVideoMaterialRepository Repository { get; set; }

        public VideoMaterialService(IDbContext context)
        {
            Repository = new VideoMaterialRepository(context);
        }

		public bool Update(int oldID, VideoMaterial newVideoMaterial)
		{
			if (newVideoMaterial == null)
				throw new ArgumentNullException("Передан пустой newSeason");

			newVideoMaterial.ID = oldID;
			newVideoMaterial.UpdateDateTime = DateTime.Now;
			return this.Repository.UpdateEntity(newVideoMaterial);
		}

		/// <summary>
		/// Получить видеоматериал по ID.
		/// </summary>
		/// <param name="id">Идентификатор.</param>
		public VideoMaterial Get(int? id)
        {
            if (!id.HasValue)
                return null;

            return this.Repository.GetEntity(id.Value);
        }
        /// <summary>
        /// Получить видеоматериал, видимый для юзеров, по ID.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        public VideoMaterial GetVisibleToUser(int? id)
        {
            if (!id.HasValue)
                return null;

            VideoMaterial result = this.Repository.GetEntity(id.Value);

            if (result == null || result.IsArchived || result.CheckStatus != CheckStatus.Confirmed)
                return null;

            return result;
        }

        /// <summary>
        /// Добавить новый видеоматериал в базу.
        /// </summary>
        /// <param name="entity">Объект для добавления.</param>
        public bool Create(VideoMaterial entity)
        {
            if (entity == null)
                return false;

            if (string.IsNullOrWhiteSpace(entity.KinopoiskID))
                throw new ArgumentOutOfRangeException("entity", "Свойство KinopoiskID не инициализировано");

            if (this.GetAll().Contains(entity))
                throw new EntryAlreadyExistsException("Видеоматериал с таким Kinopoisk ID уже существует");

            entity.AddDateTime = DateTime.Now;
            entity.UpdateDateTime = entity.AddDateTime;
            return this.Repository.AddEntity(entity);
        }

        /// <summary>
        /// Добавить контейнер видеоматериалов в базу.
        /// </summary>
        /// <param name="entities">Контейнер объектов для добавления.</param>
        public bool Create(IEnumerable<VideoMaterial> entities)
        {
            if (entities == null)
                return false;

            foreach (var item in entities)
            {
                if(!this.Create(item))
                    throw new OperationAbortedException(string.Format("Произошла ошибка при сохранении объекта сущности {0} в базу", item.GetType().Name));
            }

            return true;
        }

        /// <summary>
        /// Удалить видеоматериал из базы.
        /// </summary>
        /// <param name="entity">Объект для удаления.</param>
        public bool Remove(VideoMaterial entity)
        {
            if (entity == null)
                return false;

            return this.Repository.RemoveEntity(entity);
        }

        /// <summary>
        /// Пометить видеоматериал на удаление.
        /// </summary>
        /// <param name="entity">Объект для пометки.</param>
        public bool MarkArchive(VideoMaterial entity)
        {
            if (entity == null || !this.GetAll().Contains(entity))
                return false;

            entity.IsArchived = true;
            return this.Repository.SaveChanges();
        }

        /// <summary>
        /// Убрать пометку удаления у видеоматериала.
        /// </summary>
        /// <param name="entity">Объект для снятия пометки.</param>
        public bool UnmarkArchive(VideoMaterial entity)
        {
            if (entity == null || !this.GetAll().Contains(entity))
                return false;

            entity.IsArchived = false;
            return this.Repository.SaveChanges();
        }

        public bool AddSubscribedUser(int? videoMaterialID, ApplicationUser user)
        {
            if (!videoMaterialID.HasValue || user == null)
                return false;

            VideoMaterial videoMaterial = this.Get(videoMaterialID);

            if (videoMaterial == null)
                return false;

            videoMaterial.SubscribedUsers.Add(user);
            return this.Repository.SaveChanges();
        }

		public bool RemoveSubscribedUser(int? videoMaterialID, ApplicationUser user)
		{
			if (!videoMaterialID.HasValue || user == null)
				return false;

			VideoMaterial videoMaterial = this.Get(videoMaterialID);

			if (videoMaterial == null)
				return false;

		//	var userRes = videoMaterial.SubscribedUsers.FirstOrDefault(u => u.Id == user.Id);
			videoMaterial.SubscribedUsers.Remove(user);
		//	videoMaterial.SubscribedUsers.RemoveAt(0);
			return this.Repository.SaveChanges();
		}

		/// <summary>
		/// Получить все видеоматериалы из базы.
		/// </summary>
		public EntityList<VideoMaterial> GetAll()
        {
            return this.Repository.GetAllEntities();
        }

        /// <summary>
        /// Получить видеоматериалы по условию.
        /// </summary>
        /// <param name="predicate">Условие выборки.</param>
        public EntityList<VideoMaterial> GetWithCondition(Func<VideoMaterial, bool> predicate)
        {
            if (predicate == null)
                return null;

            return this.GetAll().Where(predicate).ToEntityList();
        }

        /// <summary>
        /// Получить один видеоматериал, подходящий под условие.
        /// </summary>
        /// <param name="predicate">Усовие выборки.</param>
        public VideoMaterial GetScalarWithCondition(Func<VideoMaterial, bool> predicate)
        {
            return this.GetAll().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Получить один видеоматериал по его главному строковому свойству (Title). Поиск не зависит от регистра.
        /// </summary>
        /// <param name="value">Значение для поиска.</param>
        public VideoMaterial GetByMainStringProperty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return this.GetScalarWithCondition(vm => vm.Title.ToLower() == value.ToLower()); // todo: метод задумывается для поиска по точному соответствию, мб надо убрать независимость от регистра
        }

        public EntityList<VideoMaterial> GetAllVisibleToUser()
        {
            return this.Repository.GetAllEntities().Where(vm => !vm.IsArchived 
                                                            && vm.CheckStatus == CheckStatus.Confirmed).ToEntityList();
        }

        public EntityList<VideoMaterial> GetVisibleToUserWithCondition(Func<VideoMaterial, bool> predicate)
        {
            if (predicate == null)
                return null;

            return this.GetAllVisibleToUser().Where(predicate).ToEntityList();
        }

        public bool AddMark(int? id, bool isPositiveMark)
        {
            if (!id.HasValue)
                return false;

            VideoMaterial videoMaterial = this.Get(id);

            if (videoMaterial == null)
                return false; // todo: добавить вид исключений: запись не найдена

            if (isPositiveMark)
                videoMaterial.PositiveMarkCount++;
            else
                videoMaterial.NegativeMarkCount++;

            return this.Repository.SaveChanges();
        }

        /// <summary>
        /// Переместить 1 отметку из в противоположную.
        /// </summary>
        /// <param name="id">Идентификатор видеоматериала.</param>
        /// <param name="fromPositive">Если true, то будет перемещение из положительных отметок в отрицательную, иначе наоборот.</param>
        /// <returns>true, если успешно, иначе false.</returns>
        public bool InvertMark(int? id, bool fromPositive)
        {
            if (!id.HasValue)
                return false;

            VideoMaterial videoMaterial = this.Get(id);

            if (videoMaterial == null)
                return false;

            if (fromPositive)
            {
                videoMaterial.PositiveMarkCount--;
                videoMaterial.NegativeMarkCount++;
            }
            else
            {
                videoMaterial.PositiveMarkCount++;
                videoMaterial.NegativeMarkCount--;
            }

            return this.Repository.SaveChanges();
        }

        public bool IsUserSubscribed(int? id, string userID)
        {
            if (!id.HasValue || string.IsNullOrWhiteSpace(userID))
                return false;

            VideoMaterial videoMaterial = this.Get(id);

            if (videoMaterial == null)
                return false;

            return videoMaterial.SubscribedUsers.FirstOrDefault(u => u.Id == userID) != null;
        }
        
        public bool EditMaterial(VideoMaterial entity)
        {
            if (entity == null)
            {
                return false;
            }
            return Repository.UpdateEntity(entity);
        }
        /// <summary>
        /// Получить нелениво загруженный материал
        /// </summary>
        public VideoMaterial GetLoaded(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }
            return this.Repository.GetEntityNotLazyLoad((int)id);
        }

        public IEnumerable<VideoMaterial> GetByPartOfTitle(string partOfTitle)
        {
            if (string.IsNullOrEmpty(partOfTitle))
            {
                return null;
            }
            return this.GetWithCondition(vm => vm.Title.ToLower().Contains(partOfTitle.ToLower()));
        }

        public IEnumerable<ElasticVideoMaterial> ElasticGetAll()
        {
            return Mapper.Map<List<VideoMaterial>, List<ElasticVideoMaterial>>(this.GetAllVisibleToUser().ToList());
        }
    }
}