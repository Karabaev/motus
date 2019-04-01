namespace SerialService.Infrastructure.ElasticSearch
{
    using SerialService.Models;
    using System.Collections.Generic;
    using System.Linq;

    public static class SearchWrapper
    {
        public static IEnumerable<ElasticVideoMaterial> FilerResults(FilterData data)
        {
            List<ElasticVideoMaterial> result = new List<ElasticVideoMaterial>();

            if (data.Countries != null && data.Countries.Any())
            {
                string countries = string.Join(",", data.Countries);
                var resultByCountries = MotusElasticsearch.SearchByCountrieName(countries);

                if(resultByCountries.Any())
                {
                    result.AddRange(resultByCountries);
                }
            }

            if (data.Genres != null && data.Genres.Any())
            {
                string genres = string.Join(",", data.Genres);
                var resultByGenres = MotusElasticsearch.SearchByGenreTitle(genres);

                if (result.Any() && resultByGenres.Any())
                {
                    result.Intersect(resultByGenres);
                }
                else if (resultByGenres.Any())
                {
                    result.AddRange(resultByGenres);
                }
            }

            if (data.Translations != null && data.Translations.Any())
            {
                string translations = string.Join(",", data.Translations);
                var resultByTranslations = MotusElasticsearch.SearchByTranslationTitle(translations);

                if (result.Any() && resultByTranslations.Any())
                {
                    result.Intersect(resultByTranslations);
                }
                else if (resultByTranslations.Any())
                {
                    result.AddRange(resultByTranslations);
                }
            }

            if (data.MinImdb != 0)
            {
                var resultByImdb = MotusElasticsearch.SearchByIMDB(data.MinImdb);

                if (result.Any() && resultByImdb.Any())
                {
                    result.Intersect(resultByImdb);
                }
                else if (resultByImdb.Any())
                {
                    result.AddRange(resultByImdb);
                }
            }

            if (data.MinKinopoisk != 0)
            {
                var resultByKinopoisk = MotusElasticsearch.SearchByKinopoiskRating(data.MinKinopoisk);

                if (result.Any() && resultByKinopoisk.Any())
                {
                    result.Intersect(resultByKinopoisk);
                }
                else if (resultByKinopoisk.Any())
                {
                    result.AddRange(resultByKinopoisk);
                }
            }

            if (data.MaxReliseDateValue != 0)
            {
                int min = data.MinReliseDateValue ?? int.MinValue;
                int max = data.MaxReliseDateValue ?? int.MaxValue;
                var resultByReliseDate = MotusElasticsearch.SearchByReliseDate(min,max);

                if (result.Any() && resultByReliseDate.Any())
                {
                    result.Intersect(resultByReliseDate);
                }
                else if (resultByReliseDate.Any())
                {
                    result.AddRange(resultByReliseDate);
                }
            }
            return result;
        }
    }
}