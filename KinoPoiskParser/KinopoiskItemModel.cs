namespace KinoPoiskParser
{
    using System.Collections.Generic;
    /// <summary>
    /// Модель страницы КиноПоиска
    /// </summary>
    public class KinopoiskItemModel
    {
        public string PoserHref { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string KinopoiskID { get; set; }
        public string Duration { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Genres { get; set; }
        public string ReleaseDate { get; set; }
        public List<string> FilmMakers { get; set; }
        public List<string> Actors { get; set; }
        public string IDMB { get; set; }
    }
}
