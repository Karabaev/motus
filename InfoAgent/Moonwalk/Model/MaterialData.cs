namespace InfoAgent.Moonwalk.Model
{
    using System;

    public class MaterialData
    {
        public DateTime? updated_at { get; set; }
        public string poster { get; set; }
        public int? year { get; set; }
        public string tagline { get; set; }
        public string description { get; set; }
        public int? age { get; set; }
        public string[] countries { get; set; }
        public string[] genres { get; set; }
        public string[] actors { get; set; }
        public string[] directors { get; set; }
        public string[] studios { get; set; }
        public float? kinopoisk_rating { get; set; }
        public int? kinopoisk_votes { get; set; }
        public float? imdb_rating { get; set; }
        public int? imdb_votes { get; set; }
        public float? mpaa_rating { get; set; }
        public int? mpaa_votes { get; set; }
    }
}
