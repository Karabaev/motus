namespace Elasticsearch
{
    using Nest;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SerialService.Models;
    /// <summary>
    /// Клиент для обращения к ElasticSearch
    /// </summary>
    public class Elasticsearch
    {
        string index = "video-materials";
        ElasticClient client;
        public Elasticsearch()
        {
            //Дефолтный индекс
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex(index);
            //Клинет(позволяет нам ебашить лекго и просто ебашить запросики к нашему эластику)
            client = new ElasticClient(settings);
            //Если целевой существует - сносим его(это необходимо поправить в дельнейшем, оставляя)
            if (client.IndexExists(index).Exists)
            {
                client.DeleteIndex(index);
            }
            //Пересоздадим индекс
            client.CreateIndex(index, c => c
                .Mappings(m => m
                    .Map<ElasticVideoMaterial>(mm => mm
                        .AutoMap()
                    )
                )
            );
        }

        /// <summary>
        /// Проиндексиорвать массив сущнойстей
        /// </summary>
        /// <param name="models"></param>
        public void IndexMany(ElasticVideoMaterial[] models)
        {
            #region Идексация
            foreach (var model in models)
            {
                //Здесь добавляем подсказки для автодополнения
                model.Suggest = new CompletionField { Input = new[] { model.Title, model.OriginalTitle}
                                                                    .Concat(model.ActorNames)
                                                                    .Concat(model.CountryNames)
                                                                    .Concat(model.FilmMakerNames)
                                                                    .Concat(model.GenreTitles)
                                                                    .Concat(model.ThemeNames)
                                                              , Weight = 80 };
            }
            #endregion
            //Индексируем
            client.IndexMany(models);
            //Обновляем
            client.Refresh(index);
        }

        /// <summary>
        /// Получить результаты полнотекстового поиска
        /// </summary>
        /// <param name="query">Запрос для полнотекстового поиска</param>
        /// <returns></returns>
        public List<ElasticVideoMaterial> GetResult(string query)
        {
            #region Запрос
            if (client.IndexExists("vm").Exists)
            {
                return client.Search<ElasticVideoMaterial>(s => s.Query(q => q
                                                                 .MultiMatch(m => m.Fields(f => f.Field(ff => ff.ActorNames)
                                                                                                .Field(ff => ff.Title))
                                                                                  .Query(query)))).Documents.ToList();
            }
            #endregion
            return null;
        }
        /// <summary>
        /// Получить подсказки автодополнения
        /// </summary>
        /// <param name="query">запрос/часть запроса</param>
        /// <returns></returns>
        public List<string> GetSuggests(string query)
        {
            #region Запрос
            var response = client.Search<ElasticVideoMaterial>(s => s.Index<ElasticVideoMaterial>()
                                                                     .Source(so => so.Includes(f => f
                                                                     .Field(ff => ff.ID)))
                                                                     .Suggest(su => su
                                                                         .Completion("suggest", cs => cs
                                                                             .Field(f => f.Suggest)
                                                                             .Prefix(query)
                                                                             .Fuzzy(f => f
                                                                                 .Fuzziness(Fuzziness.Auto))
                                                                             .Size(10))));
            #endregion
            return response.Suggest["suggest"].First().Options.Select(o=>o.Text).ToList();
        }
    }
}
