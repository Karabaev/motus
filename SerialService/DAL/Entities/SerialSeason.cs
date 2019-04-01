namespace SerialService.DAL.Entities
{
    using System;

    public class SerialSeason : IBaseEntity
    {
        public int ID { get; set; }
        public int SeasonNumber { get; set; }
        public int? EpisodesCount { get; set; }
        //public int SerialID { get; set; }
		public DateTime? LastEpisodeTime { get; set; }
		public int VideoMaterialID { get; set; }
		public virtual VideoMaterial VideoMaterial { get; set; }
		public int TranslationID { get; set; }
		public virtual Translation Translation { get; set; }

        /// <summary>
        /// Флаг "мягкого удаления" (с возможностью восстановления)
        /// </summary>
        public bool IsArchived { get; set; }

		/// <summary>
		/// Проверяет на равенство объекты по сочетанию главных свойств. (SerialID, SeasonNumber, TranslationID)
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool Alike(IBaseEntity entity)
        {
			SerialSeason other = entity as SerialSeason;

			if (other == null)
				return false;

			return this.VideoMaterialID == other.VideoMaterialID && 
				this.SeasonNumber == other.SeasonNumber &&
				this.TranslationID == other.TranslationID;
		}
    }
}
