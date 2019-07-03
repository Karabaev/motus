namespace SerialService.DAL.Entities
{
    using System;

    public class VideoMaterialViewsByUsers : IBaseEntity
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string UserIP { get; set; }
        public int VideoMaterialID { get; set; }
        public virtual VideoMaterial VideoMaterial { get; set; }
        public int EndTimeOfLastView { get; set; }
        public int? SerialSeasonID { get; set; }
        public SerialSeason SerialSeason { get; set; }
        public int? EpisodeNumber { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public bool Alike(IBaseEntity entity)
        {
            VideoMaterialViewsByUsers other = entity as VideoMaterialViewsByUsers;

            if (other == null)
                return false;

            return (this.UserID == other.UserID || this.UserIP == other.UserIP) &&
                this.VideoMaterialID == other.VideoMaterialID &&
                this.SerialSeasonID == other.SerialSeasonID;
        }

        public override string ToString()
        {
            return string.Format("ID: {0} UserID: {1} VideoMaterialID {2}: EndTimeOfLastView: {3} SerialSeasonID {4} EpisodeNumber: {5}",
                this.ID, this.UserID, this.VideoMaterialID, this.EndTimeOfLastView, this.SerialSeasonID, this.EpisodeNumber);
        }
    }
}