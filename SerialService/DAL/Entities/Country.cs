namespace SerialService.DAL.Entities
{
    using System.Collections.Generic;

    public class Country : IBaseEntity
    {
        public int ID { get; set; }
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
            Country country = entity as Country;

            if (country == null)
                return false;

            return this.Name == country.Name;
        }
    }
}
