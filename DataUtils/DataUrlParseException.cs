using System;
using System.Runtime.Serialization;

namespace DataUtils
{
    [Serializable]
    internal class DataUrlParseException : Exception
    {
        public DataUrlParseException()
        {
        }

        public DataUrlParseException(string message) : base(message)
        {
        }

        public DataUrlParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataUrlParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}