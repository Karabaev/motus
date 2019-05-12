namespace SerialService.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    public class ReturnUrlViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string ReturnUrl { get; set; }
    }
}