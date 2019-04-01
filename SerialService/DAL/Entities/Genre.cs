namespace SerialService.DAL.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// Жанр фильма.
    /// </summary>
    public class Genre : IBaseEntity
    {
        public int ID { get; set; }
        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Список видеоматериалов в этом жанре.
        /// </summary>
        public virtual List<VideoMaterial> VideoMaterials { get; set; }
        /// <summary>
        /// Флаг "мягкого удаления" (с возможностью восстановления)
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Проверяет на равенство объекты по главному свойству. (Name)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Alike(IBaseEntity entity)
        {
            Genre genre = entity as Genre;

            if (genre == null)
                return false;

            return this.Name == genre.Name;
        }
    }
}
