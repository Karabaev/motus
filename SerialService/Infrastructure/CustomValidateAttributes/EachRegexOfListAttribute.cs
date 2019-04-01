namespace SerialService.Infrastructure.CustomValidateAttributes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class EachRegexOfListAttribute: ValidationAttribute
    {
        readonly string _regex;
        public EachRegexOfListAttribute(string regex)
        {
            _regex = regex;
        }
        public override bool IsValid(object value)
        {
            List<string> listOfValues = value as List<string>;
            if (listOfValues == null || listOfValues.Count == 0)
                return true;
            foreach (string str in listOfValues)
            {
                if (!Regex.IsMatch(str, _regex))
                {
                    return false;
                }
            }
            return true;
        }
    }
}