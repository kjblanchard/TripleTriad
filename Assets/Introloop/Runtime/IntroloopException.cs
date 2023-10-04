using System;
using System.Runtime.Serialization;

namespace E7.Introloop
{
    /// <summary>
    ///     When there is no specific C# exception type to use, Introloop throws an instance of this type.
    /// </summary>
    [Serializable]
    public class IntroloopException : Exception
    {
        public IntroloopException()
        {
        }

        public IntroloopException(string message) : base(message)
        {
        }

        public IntroloopException(string message, Exception inner) : base(message, inner)
        {
        }

        protected IntroloopException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}