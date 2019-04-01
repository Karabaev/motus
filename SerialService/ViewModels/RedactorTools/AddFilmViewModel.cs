namespace SerialService.ViewModels.RedactorTools
{
    using Infrastructure.CustomValidateAttributes;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddFilmViewModel
    {
        const string nameRegex = @"^[А-я,A-z.'-]+\s?[А-я,A-z.'-]+";
        const string urlRegex = @"^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}";
        const string floatRegex = @"^\d+([\,]\d+)*([\.]\d+)?$";
        private string _IDMB;
        private string _KinopoiskRating;
        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        [MaxLength(300)]
        public string OriginalTitle { get; set; }

        public string Tagline { get; set; }

        [MaxLength(250)]
        [CustomRegexValidation(urlRegex)]
        public string PosterHref { get; set; }

        [Required]
        public int? KinopoiskID { get; set; }

        [Required]
        public int? ReleaseDate { get; set; }

        public int? Duration { get; set; }

        [Required]
        [CustomRegexValidation(floatRegex)]
        public string IDMB
        {
            get
            {
                return this._IDMB;
            }
            set
            {
                this._IDMB = value.Replace('.', ',');
            }
        }

        [Required]
        [CustomRegexValidation(floatRegex)]
        public string KinopoiskRating
        {
            get
            {
                return this._KinopoiskRating;
            }
            set
            {
                this._KinopoiskRating = value.Replace('.', ',');
            }
        }

        [EachRegexOfList(nameRegex)]
        [EachLenthValidation(50, 1)]
        public List<string> Actors { get; set; }

        [EachRegexOfList(nameRegex)]
        [EachLenthValidation(50, 1)]
        public List<string> Genres { get; set; }

        [EachRegexOfList(nameRegex)]
        [EachLenthValidation(50, 1)]
        public List<string> Countries { get; set; }

        [EachRegexOfList(urlRegex)]
        [EachLenthValidation(250, 1)]
        public List<string> Pictures { get; set; }

        [EachRegexOfList(nameRegex)]
        [EachLenthValidation(50, 1)]
        public List<string> FilmMakers { get; set; }

        [EachRegexOfList(nameRegex)]
        [EachLenthValidation(50, 1)]
        public List<string> Themes { get; set; }

        [MaxLength(550)]
        public string Text { get; set; }
    }
}