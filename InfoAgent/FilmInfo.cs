namespace InfoAgent
{
    using System;
    using System.Collections.Generic;

    public class FilmInfo
    {
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Tagline { get; set; }
        public string PosterHref { get; set; }
        public string KinopoiskID { get; set; }
        public string Description { get; set; }
        public string IframeUrl { get; set; }
        public float? IDMB { get; set; }
        public float? KinopoiskRating { get; set; }
        public int? Duration { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Genres { get; set; }
        public int? ReleaseDate { get; set; }
        public DateTime? MoonWalkAddDate { get; set; }
        public List<Translation> Translations { get; set; }
        public List<string> FilmMakers { get; set; }
        public List<string> Actors { get; set; }
        public bool? IsSerial { get; set; }
        public bool? IsBlocked { get; set; }

        public override bool Equals(object obj)
		{
			FilmInfo other = obj as FilmInfo;

			if (other != null)
			{
				foreach (var item in this.GetType().GetProperties())
				{
					if (item.GetValue(this) != item.GetValue(other))
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
	}
}
