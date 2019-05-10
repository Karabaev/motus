namespace Tools.Exceptions
{
    using System;

    public class NotMatchedSeasonsException : Exception
    {
        public string Value { get; }

        public NotMatchedSeasonsException()
        {

        }

        public NotMatchedSeasonsException(string message)
            : base(message)
        {

        }

        public NotMatchedSeasonsException(string message, string value)
            : base(message)
        {
            this.Value = value;
        }
    }
}
