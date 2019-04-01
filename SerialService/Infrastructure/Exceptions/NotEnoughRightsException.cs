namespace SerialService.Infrastructure.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NotEnoughRightsException : Exception
    {
        public NotEnoughRightsException() { }
        public NotEnoughRightsException(string message) : base(message) { }
        public NotEnoughRightsException(string message, Exception inner) : base(message, inner) { }
        protected NotEnoughRightsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}