namespace SerialService.Infrastructure.ElasticSearch
{
    using SerialService.Models;
    using System.Collections.Generic;
    using System.Linq;

    public static class SearchWrapper
    {
        public static IEnumerable<ElasticVideoMaterial> FilerResults(FilterData data)
        {
            List<ElasticVideoMaterial> resultByCountries = null;
            List<ElasticVideoMaterial> resultByGenres = null;
            List<ElasticVideoMaterial> resultByTranslations = null;
            List<ElasticVideoMaterial> resultByImdb = null;
            List<ElasticVideoMaterial> resultByKinopoisk = null;
            List<ElasticVideoMaterial> resultByReliseDate = null;

            if (data.Countries != null && data.Countries.Any())
            {                
                string countries = string.Join(",", data.Countries);
                resultByCountries = MotusElasticsearch.SearchByCountrieName(countries).ToList();                
            }

            if (data.Genres != null && data.Genres.Any())
            {
                string genres = string.Join(",", data.Genres);
                resultByGenres = MotusElasticsearch.SearchByGenreTitle(genres).ToList();                
            }

            if (data.Translations != null && data.Translations.Any())
            {
                string translations = string.Join(",", data.Translations);
                resultByTranslations = MotusElasticsearch.SearchByTranslationTitle(translations).ToList();
            }

            if (data.MinImdb != 0)
            {
                resultByImdb = MotusElasticsearch.SearchByIMDB(data.MinImdb).ToList();                
            }

            if (data.MinKinopoisk != 0)
            {
                resultByKinopoisk = MotusElasticsearch.SearchByKinopoiskRating(data.MinKinopoisk).ToList();               
            }

            if (data.MaxReliseDateValue.HasValue)
            {
                int min = data.MinReliseDateValue ?? int.MinValue;
                resultByReliseDate = MotusElasticsearch.SearchByReliseDate(min, data.MaxReliseDateValue.Value).ToList();                
            }

            var listOfList = new List<List<ElasticVideoMaterial>>();
            if (resultByCountries != null)
            {
                listOfList.Add(resultByCountries);
            }
            if (resultByGenres != null)
            {
                listOfList.Add(resultByGenres);
            }
            if(resultByTranslations != null)
            {
                listOfList.Add(resultByTranslations);
            }
            if(resultByImdb != null)
            {
                listOfList.Add(resultByImdb);
            }
            if (resultByKinopoisk != null)
            {
                listOfList.Add(resultByKinopoisk);
            }
            if (resultByReliseDate != null)
            {
                listOfList.Add(resultByReliseDate);
            }

            var hashSet = new HashSet<ElasticVideoMaterial>(listOfList.First());
            foreach (var list in listOfList.Skip(1))
            {
                hashSet.IntersectWith(list);
            }

            return OrderMaterials(hashSet.ToList());
        }

        /// <summary>
        /// Сортировка результатов elasticsearch
        /// </summary>
        /// <returns></returns>
        public static List<ElasticVideoMaterial> OrderMaterials(IEnumerable<ElasticVideoMaterial> data)
        {
            if(data == null)
            {
                return new List<ElasticVideoMaterial>();
            }
            return data.OrderBy(d => d.Imdb < 6.5)
                       .ThenByDescending(d => d.ReleaseDate)
                       .ThenByDescending(d => d.Imdb)
                       .ToList();
        }
    }
}