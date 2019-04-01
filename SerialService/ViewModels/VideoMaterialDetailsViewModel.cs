namespace SerialService.ViewModels
{
    using SerialService.Models;
    using System;
    using System.Collections.Generic;

    public class VideoMaterialDetailsViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Text { get; set; }
        public string PosterURL { get; set; }
        public string KinopoiskID { get; set; }
        public float? KinopoiskRating { get; set; }
        public float? Imdb { get; set; }
        public int Duration { get; set; }
        public bool IsUserSubscribed { get; set; }
        public List<string> GenreTitles { get; set; }
        public List<string> CountryNames { get; set; }
        public string AuthorName { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public int? ReleaseDate { get; set; }
        public List<string> TranslationTitles { get; set; }
        public List<string> PictureURLs { get; set; }
        public List<string> FilmMakerNames { get; set; }
        public List<string> ActorNames { get; set; }
        public int? SerialSeasonsCount { get; set; } // null, если Фильм.
        public DateTime? LastEpisodeTime { get; set; }
        public string LastEpisodeTranslator { get; set; }
        public List<string> ThemeNames { get; set; }
        public List<ElasticVideoMaterial> Similar { get; set; }
    }
}