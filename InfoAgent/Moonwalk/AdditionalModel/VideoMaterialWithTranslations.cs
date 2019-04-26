namespace InfoAgent.Moonwalk.AdditionalModel
{
    using System.Collections.Generic;
    using Model;

    public class VideoMaterialWithTranslations
    {
        public int? KinopoiskID { get; set; }
        public List<IVideoMaterial> Translations { get; set; }
    }
}
