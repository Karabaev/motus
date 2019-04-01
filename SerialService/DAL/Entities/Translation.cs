namespace SerialService.DAL.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// Перевод.
    /// </summary>
    public class Translation : IBaseEntity
    {
        public int ID { get; set; }
        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; set; }

        public virtual List<SerialSeason> SerialSeasons { get; set; }

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
            Translation translation = entity as Translation;

            if (translation == null)
                return false;

            return this.Name == translation.Name;
        }
    }
}
