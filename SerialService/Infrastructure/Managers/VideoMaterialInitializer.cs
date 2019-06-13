namespace SerialService.Infrastructure.Managers
{
    using System;
    using System.Collections.Generic;

    public struct VideoMaterialInitializer
    {
        public VideoMaterialInitializer(
            string title,
            string originalTitle,
            string text,
            string tagline,
            string kinopoiskID,
            float imdb,
            float? kinopoiskRating,
            int? duration,
            string authorMail,
            DateTime? moonWalkAddDate,
            int? releaseDate,
            IEnumerable<string> countries,
            IEnumerable<string> genres,
            string posterURL,
            IEnumerable<string> filmMakers,
            IEnumerable<string> actors,
            IEnumerable<SerialSeasonInitializer> serialSeasons,
            IEnumerable<string> themes,
            CheckStatus checkStatus,
            bool watchForUpdates,
            bool isArchived,
            bool isSerial,
            string iframeUrl)
        {
            this.Title = title;
            this.OriginalTitle = originalTitle;
            this.Text = text;
            this.Tagline = tagline;
            this.KinopoiskID = kinopoiskID;
            this.IDMB = imdb;
            this.KinopoiskRating = kinopoiskRating;
            this.Duration = duration;
            this.AuthorMail = authorMail;
            this.MoonWalkAddDate = moonWalkAddDate;
            this.ReleaseDate = releaseDate;
            this.CheckStatus = checkStatus;
            this.WatchForUpdates = watchForUpdates;
            this.IsArchived = isArchived;
            this.Countries = new List<string>(countries);
            this.Genres = new List<string>(genres);
            this.PosterURL = posterURL;
            this.FilmMakers = new List<string>(filmMakers);
            this.Actors = new List<string>(actors);
            this.SerialSeasonInitializers = new List<SerialSeasonInitializer>(serialSeasons);
            this.Themes = new List<string>(themes);
            this.IsSerial = isSerial;
            this.IframeUrl = iframeUrl;
        }

        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Text { get; set; }
        public string Tagline { get; set; }
        public string KinopoiskID { get; set; }
        public string IframeUrl { get; set; }
        public float IDMB { get; set; }
        public float? KinopoiskRating { get; set; }
        public int? Duration { get; set; }
        public string AuthorMail { get; set; }
        public DateTime? MoonWalkAddDate { get; set; }
        public int? ReleaseDate { get; set; }
        public CheckStatus CheckStatus { get; set; }
        public bool WatchForUpdates { get; set; }
        public bool IsArchived { get; set; }
        public bool IsSerial { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Genres { get; set; }
        public string PosterURL { get; set; }
        public List<string> FilmMakers { get; set; }
        public List<string> Actors { get; set; }
        public List<SerialSeasonInitializer> SerialSeasonInitializers { get; set; }
        public List<string> Themes { get; set; }

    }
}