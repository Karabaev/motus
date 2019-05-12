namespace SerialService.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ExternalLoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ошибка провайдера OAuth")]
        public string Provider { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ReturnUrl { get; set; }
    }
}