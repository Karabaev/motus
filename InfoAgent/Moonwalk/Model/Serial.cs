namespace InfoAgent.Moonwalk.Model
{
    public class Serial : IVideoMaterial
    {
        public string title_ru { get; set; }
        public string title_en { get; set; }
        public int? year { get; set; }
        public string token { get; set; }
        public string type { get; set; }
        public int? kinopoisk_id { get; set; }
        public int? world_art_id { get; set; }
        public string translator { get; set; }
        public int? translator_id { get; set; }
        public string iframe_url { get; set; }
        public string trailer_token { get; set; }
        public string trailer_iframe_url { get; set; }
        public int? seasons_count { get; set; }
        public int? episodes_count { get; set; }
        public string category { get; set; }
        public Block block { get; set; }
        public SeasonEpisodesCount[] season_episodes_count { get; set; }
        public MaterialData material_data { get; set; }
    }
}
