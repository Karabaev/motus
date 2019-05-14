namespace SerialService.ViewModels.User
{
    public class SaveViewTimeViewModel
    {
        public int TimeSec { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
        public string TranslatorName { get; set; }
        public int VideoMaterialID { get; set; }
        public string UserID { get; set; }
    }
}