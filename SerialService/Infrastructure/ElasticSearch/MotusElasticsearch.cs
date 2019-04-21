namespace SerialService.Infrastructure.ElasticSearch
{
    using Nest;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SerialService.Models;

    /// <summary>
    /// Клиент для обращения к ElasticSearch
    /// </summary>
    public static class MotusElasticsearch
    {
        const string index = "video-materials";
        static ElasticClient client;
        static int StoreSize { get; set; }//Максимальное количество результатов поиска
        static MotusElasticsearch()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            client = new ElasticClient(settings);
            //Во избежаение ошибок предварительно удаляем существующий индекс
            //TODO: В последствии необходимо разработать менее затратный способ обновления
            if (client.IndexExists(index).Exists)
            {
                client.DeleteIndex(index);
            }
            var nGramFilters = new List<string> { "lowercase","russian_morphology","english_morphology" };
            //Создаем индекс с учетом анализа n-gram для поиска по части слова и нечеткого поиска
            var resp = client.CreateIndex(index, c => c
                 .Mappings(m => m
                    .Map<ElasticVideoMaterial>(mm => mm
                        .AutoMap()
                        .Properties(p => p
                            .Text(t => t
                                .Name(n => n.Title)
                                .Fields(f => f
                                    .Keyword(k => k
                                        .Name("keyword")
                                        .IgnoreAbove(256)
                                    )
                                    .Text(tt => tt
                                        .Name("ngram")
                                        .Analyzer("ngram_analyzer")
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(n => n.OriginalTitle)
                                .Fields(f => f
                                    .Keyword(k => k
                                        .Name("keyword")
                                        .IgnoreAbove(256)
                                    )
                                    .Text(tt => tt
                                        .Name("ngram")
                                        .Analyzer("ngram_analyzer")
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(n => n.ActorNames)
                                .Fields(f => f
                                    .Keyword(k => k
                                        .Name("keyword")
                                        .IgnoreAbove(256)
                                    )
                                    .Text(tt => tt
                                        .Name("ngram")
                                        .Analyzer("ngram_analyzer")
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(n => n.FilmMakerNames)
                                .Fields(f => f
                                    .Keyword(k => k
                                        .Name("keyword")
                                        .IgnoreAbove(256)
                                    )
                                    .Text(tt => tt
                                        .Name("ngram")
                                        .Analyzer("ngram_analyzer")
                                    )
                                )
                            )
                        )
                    )
                )
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(anz => anz
                            .Custom("ngram_analyzer", cc => cc
                                .Filters(nGramFilters)
                                .Tokenizer("ngram_tokenizer")))
                        .Tokenizers(tz => tz
                            .NGram("ngram_tokenizer", td => td
                                .MinGram(3)
                                .MaxGram(3)
                                .TokenChars(TokenChar.Letter, TokenChar.Digit)
                            )
                        )
                    )
                )
            );
        }

        /// <summary>
        /// Индексировать элементы
        /// </summary>
        /// <param name="models">Набор элементов ElasticVideoMaterial</param>
        public static void Index(IEnumerable<ElasticVideoMaterial> models)
        {
            if (models == null || !models.Any())
            {
                return;
            }
            //TODO: В данный момент индексация производится циклом, однако в последствии рекомендуется заменить с использованеим IndexMany()
            int count = 1;
            foreach (var model in models)
            {
                List<string> suggestArr = new List<string> { model.OriginalTitle, model.Title};
                suggestArr = suggestArr.Concat(model.ActorNames)
                          .Concat(model.FilmMakerNames)
                          .Concat(model.ThemeNames)
                          .ToList();
                model.Suggest = new CompletionField
                {
                    Input = suggestArr
                };
                client.Index(model,i=>i.Index(index));
                count++;
            };
            StoreSize = count;
        }

        /// <summary>
        /// Основной поиск
        /// </summary>
        /// <param name="query">Параметр поиска. Может быть частью названия, оригинального названия, имени актреа или режессера</param>
        public static IEnumerable<ElasticVideoMaterial> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }
            var result = client.SearchAsync< ElasticVideoMaterial > (i => i
                                                              .Query(q => q
                                                                  .MultiMatch(m => m
                                                                      .Fields(f => f
                                                                          .Field(ff => ff.OriginalTitle.Suffix("ngram"))
                                                                          .Field(ff => ff.Title.Suffix("ngram"))
                                                                          .Field(ff => ff.ActorNames.Suffix("ngram"))
                                                                          .Field(ff => ff.FilmMakerNames.Suffix("ngram")))
                                                                      .Query(query)
                                                                  )
                                                              )
                                                              .Index(index)
                                                        ).Result.Documents.ToList();
            return result;
        }

        /// <summary>
        /// Строгий поиск по имени актера
        /// </summary>
        public static IEnumerable<ElasticVideoMaterial> SearchByActorName(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Size(StoreSize)
                .Index(index)
                .Query(q => q
                    .MatchPhrasePrefix(m => m.Field(f => f.ActorNames)
                    .Query(query)))).Result.Documents.ToList();
            return result;
        }

        /// <summary>
        /// Строгий поиск по названию жанра
        /// </summary>
        public static IEnumerable<ElasticVideoMaterial> SearchByGenreTitle(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Size(StoreSize)
                .Index(index)
                .Query(q => q
                    .Match(m => m.Field(f => f.GenreTitles)
                    .Query(query)))).Result.Documents.ToList();
            return result;
        }

        /// <summary>
        /// Строгий поиск по названию студии перевода
        /// </summary>
        public static IEnumerable<ElasticVideoMaterial> SearchByTranslationTitle(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Size(StoreSize)
                .Index(index)
                .Query(q => q
                    .Match(m => m.Field(f => f.TranslationTitles)
                    .Query(query)))).Result.Documents.ToList();
            return result;
        }

        /// <summary>
        /// Строгий поиск по названию страны
        /// </summary>
        public static IEnumerable<ElasticVideoMaterial> SearchByCountrieName(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Size(StoreSize)
                .Index(index)
                .Query(q => q
                    .Match(m => m.Field(f => f.CountryNames)
                    .Query(query)))).Result.Documents.ToList();
            return result;
        }

        /// <summary>
        /// Поиск элементов с рейтингом IMDB выше заданного значения
        /// </summary>
        public static IEnumerable<ElasticVideoMaterial> SearchByIMDB(float imdb)
        {
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Size(StoreSize)
                .Index(index)
                .Query(q => q
                    .Range(r => r
                        .Field(f => f.Imdb)
                        .GreaterThanOrEquals(imdb)))).Result.Documents.ToList();
            return result;
        }

        /// <summary>
        /// Поиск элементов с рейтингом Кинопоиска выше заданного значения
        /// </summary>
        public static IEnumerable<ElasticVideoMaterial> SearchByKinopoiskRating(float imdb)
        {
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Size(StoreSize)
                .Index(index)
                .Query(q => q
                    .Range(r => r
                        .Field(f => f.KinopoiskRating)
                        .GreaterThanOrEquals(imdb)))).Result.Documents.ToList();
            return result;
        }

        /// <summary>
        /// Поиск элементов с датой выхода в заданном диапазоне
        /// </summary>
        public static IEnumerable<ElasticVideoMaterial> SearchByReliseDate(int yearFrom, int yearTo)
        {
            if (yearTo == 0)
            {
                yearTo = int.MaxValue;
            }
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Size(StoreSize)
                .Index(index)
                .Query(q => q
                    .Range(r => r
                        .Field(f => f.ReleaseDate)
                        .GreaterThanOrEquals(yearFrom)
                        .LessThanOrEquals(yearTo)))).Result.Documents.ToList();
            return result;
        }

        /// <summary>
        /// Получить подсказки
        /// </summary>
        /// <param name="query">Название, оргинальное название, имя актера или режиссера, ключевое слово</param>
        /// <returns></returns>
        public static List<string> GetSuggest(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Index(index)
                .Suggest(su => su
                    .Completion("suggest", c => c
                         .Field(f => f.Suggest)
                         .Prefix(query)
                         .Fuzzy(f => f.Fuzziness(Fuzziness.Auto)
                         )
                    .Size(5)//Возвращает 5 подсказок
                    )
                )
            ).Result.Suggest["suggest"].First().Options.Select(s=>s.Text).Distinct().ToList();
            return result;
        }

        /// <summary>
        /// Получить набор для страницы
        /// </summary>
        /// <param name="begin">Начало отсчета</param>
        /// <param name="count">Количество записей</param>
        /// <returns></returns>
        public static List<ElasticVideoMaterial> GetPage(int begin, int count)
        {
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Index(index)
                .From(begin)
                .Size(count)
                .Sort(q=>q.Descending(a=>a.ReleaseDate))).Result.Documents.ToList();
            return result;
        }

        /// <summary>
        /// Получить все записи
        /// </summary>
        /// <returns></returns>
        public static List<ElasticVideoMaterial> GetAll()
        {
            return SearchWrapper.OrderMaterials(
                client.SearchAsync<ElasticVideoMaterial>(s => s
                    .Index(index)
                    .Size(StoreSize)
                ).Result
                .Documents);
        }

        /// <summary>
        /// Получить похожие документы
        /// </summary>
        /// <returns></returns>
        public static List<ElasticVideoMaterial> GetSimilar(ElasticVideoMaterial material)
        {
            if (material == null)
            {
                throw new NullReferenceException();
            }
            var result = client.SearchAsync<ElasticVideoMaterial>(s => s
                .Index(index)
                .Query(q => q
                    .MoreLikeThis(mlt => mlt
                        .Like(l => l
                            .Document(d => d.Id(material.ID).Index(index))
                        )
                        .Analyzer("ngram_analyzer")
                        .MinTermFrequency(1)
                        .MinDocumentFrequency(1)
                        .Fields(f => f
                            .Field(p => p.Title)
                            .Field(p => p.GenreTitles)
                            .Field(p => p.FilmMakerNames)
                            .Field(p => p.TranslationTitles)
                            )
                        .Boost(1.1)
                        .BoostTerms(1.1)
                    )
                )
                .Size(10)
            ).Result.Documents.ToList();                
            return result;
        }
    }
}
