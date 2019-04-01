using Nest;
using System;
using System.Threading;

namespace SerialService.Infrastructure.ElasticSearch
{
    public static class ElasticIndexControll
    {
        static private ElasticClient client;

        static ElasticIndexControll()
        {
            client = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")));
        }

        static public void CloseIndex(string indexName)
        {
            ElascticStateCache.IndexNeeded = false;
            Thread.Sleep(500);
            if (ElascticStateCache.IndexNeeded)
            {
                return;
            }
            client.CloseIndex(indexName);
        }

        static public void OpenIndex(string indexName)
        {
            ElascticStateCache.IndexNeeded = true;
            //TODO:Сделать логику проверки
            client.OpenIndex(indexName);
            Thread.Sleep(500);
        }

        static bool GetOpennessFlag(string indexName)
        {
            return client.ClusterState().Metadata.Indices[indexName].State == "open";
        }
    }
}