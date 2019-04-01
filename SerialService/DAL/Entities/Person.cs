namespace SerialService.DAL.Entities
{
    using System.Collections.Generic;

    public class Person : IBaseEntity
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public virtual List<VideoMaterial> ActorVideoMaterials { get; set; }
        public virtual List<VideoMaterial> FilmMakerVideoMaterials { get; set; }
        /// <summary>
        /// Флаг "мягкого удаления" (с возможностью восстановления)
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Проверяет на равенство объекты по главному свойству. (FullName)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Alike(IBaseEntity entity)
        {
            Person person = entity as Person;

            if (person == null)
                return false;

            return this.FullName == person.FullName;
        }
    }
}
