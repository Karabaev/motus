namespace SerialService.Infrastructure.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class EntryAlreadyExistsException : Exception
    {
        public EntryAlreadyExistsException() { }
        public EntryAlreadyExistsException(string message) : base(message) { }
        public EntryAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
        protected EntryAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}