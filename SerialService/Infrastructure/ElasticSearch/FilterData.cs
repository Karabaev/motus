namespace SerialService.Infrastructure.ElasticSearch
{
    /// <summary>
    /// Датакласс для использования в фильтрах
    /// </summary>
    public class FilterData
    {
        public string[] Genres { get; set; }
        
        public string[] Countries { get; set; }

        public string[] Translations { get; set; }

        public float MinImdb { get; set; }

        public float MinKinopoisk { get; set; }

        public int? MinReliseDateValue { get; set; }

        public int? MaxReliseDateValue { get; set; }
    }
}