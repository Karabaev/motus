namespace SerialService.DAL.Entities
{
    using Infrastructure;
    using System;
    using System.Collections.Generic;

    public class VideoMaterial : IVideoMaterialtem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Text { get; set; }
        public string Tagline { get; set; }
        public string KinopoiskID { get; set; }
        public float IDMB { get; set; }
        public float? KinopoiskRating { get; set; }
        public int? Duration { get; set; }
        public virtual List<Country> Countries { get; set; }
        public virtual List<Genre> Genres { get; set; }
        public string AuthorID { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public DateTime? AddDateTime { get; set; }
        public DateTime? MoonWalkAddDate { get; set;}
        public int? ReleaseDate { get; set; }
        public int PositiveMarkCount { get; set; }
        public int NegativeMarkCount { get; set; }
        public virtual List<Picture> Pictures { get; set; }
        public virtual List<Person> FilmMakers { get; set; }
        public virtual List<Person> Actors { get; set; }
        public virtual List<VideoMark> VideoMarks { get; set; }
        public virtual List<SerialSeason> SerialSeasons { get; set; }
        public virtual List<Theme> Themes { get; set; }
        public virtual List<ApplicationUser> SubscribedUsers { get; set; }
        public CheckStatus CheckStatus { get; set; }
		/// <summary>
		/// Флаг, отслеживать обновления?
		/// </summary>
		public bool WatchForUpdates { get; set; }
        /// <summary>
        /// Флаг "мягкого удаления" (с возможностью восстановления)
        /// </summary>
        public bool IsArchived { get; set; }

        public bool IsSerial { get; set; }

       // public virtual List<VideoMaterialViewsByUsers> ViewsByUsers { get; set; }

       // public string IframeUrl { get; set; }

        public override bool Equals(object obj) // todo: надо протестировать этот метод
        {
            VideoMaterial vm = obj as VideoMaterial;

            if (vm != null)
            {
                foreach (var item in this.GetType().GetProperties())
                {
                    if(item.GetValue(this) != item.GetValue(vm))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Проверяет на равенство объекты по главному свойству. (KinopoiskID)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Alike(IBaseEntity entity)
        {
            VideoMaterial videoMaterial = entity as VideoMaterial;

            if (videoMaterial == null)
                return false;

            return this.KinopoiskID == videoMaterial.KinopoiskID;
        }
    }
}
