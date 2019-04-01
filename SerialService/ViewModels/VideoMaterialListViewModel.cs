namespace SerialService.ViewModels
{
    using System.Collections.Generic;

    public class VideoMaterialListViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string PosterURL { get; set; }
        public List<string> GenreTitles { get; set; }
        public List<string> CountryNames { get; set; }
        public int PositiveMarkCount { get; set; }
        public int NegativeMarkCount { get; set; }
        public float? KinopoiskRating { get; set; }
        public float? Imdb { get; set; }
        public int? Duration { get; set; }
    }
}