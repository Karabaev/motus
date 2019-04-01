namespace SerialService.Infrastructure.CustomValidateAttributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class CustomRegexValidationAttribute: ValidationAttribute
    {
        readonly string _regex;
        public CustomRegexValidationAttribute(string regex)
        {
            _regex = regex;
        }
        public override bool IsValid(object value)
        {
            string _value = value as string;
            if (value == null)
                return true;
            if (!Regex.IsMatch(_value, _regex))
            {
                return false;
            }
            return true;
        }
    }
}