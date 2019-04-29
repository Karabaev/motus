namespace SerialService.Models
{
    using Nest;
    using System;
    using System.Collections.Generic;

    public class ElasticVideoMaterial:IEquatable<ElasticVideoMaterial>
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public float? KinopoiskRating { get; set; }
        public string PosterURL { get; set; }
        public float? Imdb { get; set; }
        public int Duration { get; set; }
        public DateTime? AddDate { get; set; }
        public List<string> GenreTitles { get; set; }
        public List<string> CountryNames { get; set; }
        public int? ReleaseDate { get; set; }
        public List<string> TranslationTitles { get; set; }
        public List<string> FilmMakerNames { get; set; }
        public List<string> ActorNames { get; set; }
        public List<string> ThemeNames { get; set; }
        public string Description { get; set; }
        public CompletionField Suggest { get; set; }

        public bool Equals(ElasticVideoMaterial other)
        {
            return other != null &&
                   other.ID == this.ID;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}