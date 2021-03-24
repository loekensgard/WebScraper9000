using System;
using System.Runtime.Serialization;

namespace WebScraper9000.Exceptions
{
    public class IdkException : Exception
    {
        public IdkException()
        {
        }

        public IdkException(string message) : base(message)
        {
        }

        public IdkException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IdkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
