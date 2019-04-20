namespace InfoAgent.Moonwalk.Model
{
    interface IVideoMaterial
    {
        string title_ru { get; set; }
        string title_en { get; set; }
        int? year { get; set; }
        int? kinopoisk_id { get; set; }
        int? world_art_id { get; set; }
        string translator { get; set; }
        int? translator_id { get; set; }
        string iframe_url { get; set; }
        string trailer_token { get; set; }
        string trailer_iframe_url { get; set; }
        string category { get; set; }
        Block block { get; set; }
        MaterialData material_data { get; set; }
        string token { get; set; }
        string type { get; set; }
    }
}
