namespace SerialService.Infrastructure.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class EntryNotFoundException : Exception
    {
        public EntryNotFoundException() { }
        public EntryNotFoundException(string message) : base(message) { }
        public EntryNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected EntryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}