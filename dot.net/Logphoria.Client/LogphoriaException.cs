using System;

namespace Logphoria.Driver
{
    public class LogphoriaException : Exception
    {
        public LogphoriaException() { }
        public LogphoriaException(string message) : base(message) { }
        public LogphoriaException(string message, Exception innerException) : base(message, innerException) { }
    }
}