namespace InfoAgent.Exceptions
{
    using System;

    public class NotFoundInFilmBaseException:ArgumentException
    {
        public string Value { get; }

        public NotFoundInFilmBaseException()
        {

        }

        public NotFoundInFilmBaseException(string message)
            : base(message)
        {

        }

        public NotFoundInFilmBaseException(string message, string value)
            : base(message)
        {
            this.Value = value;
        }
    }
}
