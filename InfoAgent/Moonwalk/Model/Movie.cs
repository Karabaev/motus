namespace InfoAgent.Moonwalk.Model
{
    using System;

    public class Movie : IVideoMaterial
    {
        public string title_ru { get; set; }
        public string title_en { get; set; }
        public int? year { get; set; }
        public Duration duration { get; set; }
        public int? kinopoisk_id { get; set; }
        public int? world_art_id { get; set; }
        public int? pornolab_id { get; set; }
        public string token { get; set; }
        public string type { get; set; }
        public bool? camrip { get; set; }
        public string source_type { get; set; }
        public string source_quality_type { get; set; }
        public bool? instream_ads { get; set; }
        public bool? directors_version { get; set; }
        public string iframe_url { get; set; }
        public string trailer_token { get; set; }
        public string trailer_iframe_url { get; set; }
        public string translator { get; set; }
        public int? translator_id { get; set; }
        public DateTime added_at { get; set; }
        public string category { get; set; }
        public Block block { get; set; }
        public MaterialData material_data { get; set; }
    }
}
