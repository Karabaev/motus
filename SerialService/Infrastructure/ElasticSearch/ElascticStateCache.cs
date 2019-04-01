namespace SerialService.Infrastructure.ElasticSearch
{
    public static class ElascticStateCache
    {
        private static bool indexNeeded = true;
        private static object locker = new object();

        public static bool IndexNeeded { get; set; }
    }
}