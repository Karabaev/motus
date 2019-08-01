namespace SerialService.Infrastructure.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException() { }
        public AccessDeniedException(string message) : base(message) { }
        public AccessDeniedException(string message, Exception inner) : base(message, inner) { }
        protected AccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}