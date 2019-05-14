namespace SerialService.DAL.Entities
{
    public class VideoMaterialViewsByUsers : IBaseEntity
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int VideoMaterialID { get; set; }
        public virtual VideoMaterial VideoMaterial { get; set; }
        public int EndTimeOfLastView { get; set; }
        public int? SerialSeasonID { get; set; }
        public SerialSeason SerialSeason { get; set; }
        public int? EpisodeNumber { get; set; }

        public bool Alike(IBaseEntity entity)
        {
            VideoMaterialViewsByUsers other = entity as VideoMaterialViewsByUsers;

            if (other == null)
                return false;

            return this.UserID == other.UserID &&
                this.VideoMaterialID == other.VideoMaterialID &&
                this.SerialSeasonID == other.SerialSeasonID;
        }
    }
}