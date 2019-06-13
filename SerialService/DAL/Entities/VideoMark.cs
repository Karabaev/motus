namespace SerialService.DAL.Entities
{
    public class VideoMark : IMark
    {
        public int ID { get; set; }
        public string UserIP { get; set; }
        public bool Value { get; set; }
        public int VideoMaterialID { get; set; }
        public virtual VideoMaterial VideoMaterial { get; set; }
        public string AuthorID { get; set; }
        public ApplicationUser Author { get; set; }

        /// <summary>
        /// Проверяет на равенство объекты по главным свойствам. (VideoMaterialID и UserIP)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Alike(IBaseEntity entity)
        {
            VideoMark videoMark = entity as VideoMark;

            if (videoMark == null)
                return false;

            return this.VideoMaterialID == videoMark.VideoMaterialID && (this.AuthorID == videoMark.AuthorID || this.UserIP == videoMark.UserIP);
        }
    }
}
