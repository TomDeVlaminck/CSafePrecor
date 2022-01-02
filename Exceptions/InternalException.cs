using System;

namespace CSafe.Exceptions
{
    [Serializable]
    public class CommandSetException : Exception
    {
        public CommandSetException(string message) :
            base(message) {}
    }

    [Serializable]
    public class BufferExceededException : Exception
    {
        public BufferExceededException(string message) :
            base(message)
        {
        }
    }
}
