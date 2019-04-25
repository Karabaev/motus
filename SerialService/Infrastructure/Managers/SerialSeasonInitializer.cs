namespace SerialService.Infrastructure.Managers
{
    using System;

    public struct SerialSeasonInitializer
    {
        public SerialSeasonInitializer(
            int seasonNumber, 
            int? episodesCount,
            DateTime? lastEpisodeTime,
            string translationName)
        {
            this.SeasonNumber = seasonNumber;
            this.EpisodesCount = episodesCount;
            this.LastEpisodeTime = lastEpisodeTime;
            this.TranslationName = translationName;
        }

        public int SeasonNumber { get; set; }
        public int? EpisodesCount { get; set; }
        public DateTime? LastEpisodeTime { get; set; }
        public string TranslationName { get; set; }
    }
}