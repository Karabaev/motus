namespace SerialService.DAL.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// Настроение.
    /// </summary>
    public class Theme : IBaseEntity
    {
        public int ID { get; set; }
        /// <summary>
        /// Название настроения.
        /// </summary>
        public string Name { get; set; }

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
            Theme theme = entity as Theme;

            if (theme == null)
                return false;

            return this.Name == theme.Name;
        }
    }
}
