namespace InfoAgent.Exceptions
{
    using System;

    public class IsBlockedException: Exception
    {
        public string Value { get; }

        public IsBlockedException()
        {

        }

        public IsBlockedException(string message)
            : base(message)
        {

        }

        public IsBlockedException(string message, string value)
            : base(message)
        {
            this.Value = value;
        }
    }
}
