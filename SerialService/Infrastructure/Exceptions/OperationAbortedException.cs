namespace SerialService.Infrastructure.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class OperationAbortedException : Exception
    {
        public OperationAbortedException() { }
        public OperationAbortedException(string message) : base(message) { }
        public OperationAbortedException(string message, Exception inner) : base(message, inner) { }
        protected OperationAbortedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}