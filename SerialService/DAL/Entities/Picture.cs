namespace SerialService.DAL.Entities
{
    public class Picture : IBaseEntity
    {
        public int ID { get; set; }
        public string URL { get; set; }
        public int? VideoMaterialID { get; set; }
        public virtual VideoMaterial VideoMaterial { get; set; }
        public bool IsPoster { get; set; }
        /// <summary>
        /// Флаг "мягкого удаления" (с возможностью восстановления)
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Проверяет на равенство объекты по главным свойствам. (URL и VideoMaterialID)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Alike(IBaseEntity entity)
        {
            Picture picture = entity as Picture;

            if (picture == null)
                return false;

            return this.URL == picture.URL && this.VideoMaterialID == picture.VideoMaterialID;
        }
    }
}
