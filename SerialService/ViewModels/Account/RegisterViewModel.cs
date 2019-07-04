namespace SerialService.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        [Required]
        [RegularExpression(@"^[A-Za-zА-Яа-яЁё0-9_]*$", ErrorMessage = "Имя пользователя должно содержать только буквы латинского алфавита и цифры")]
        [StringLength(30, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 1)]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression("^[а-яА-ЯёЁa-zA-Z0-9]+$")]
        [Display(Name = "Секретное слово")]
        public string Parole { get; set; }

        [Required(ErrorMessage = "Вы волжны согласиться с политикой конвиденциальности")]
        [Display(Name = "Согласен")]
        public bool PrivacyPolicyConfirmed { get; set; }
    }
}