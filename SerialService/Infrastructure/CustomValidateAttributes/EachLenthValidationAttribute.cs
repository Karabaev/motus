namespace SerialService.Infrastructure.CustomValidateAttributes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EachLenthValidationAttribute: ValidationAttribute
    {
        readonly int _minLimit;
        readonly int _maxLimit;
        public EachLenthValidationAttribute(int maxLimit, int minLimit = 1)
        {
            _minLimit = minLimit;
            _maxLimit = maxLimit;
        }
        public override bool IsValid(object value)
        {
            List<string> listOfValues = value as List<string>;
            if (listOfValues == null || listOfValues.Count == 0)
                return true;
            foreach (string str in listOfValues)
            {
                if (!(_minLimit<=str.Length&&str.Length<=_maxLimit))
                {
                    return false;
                }
            }
            return true;
        }
    }
}