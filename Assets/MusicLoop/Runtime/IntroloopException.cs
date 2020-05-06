using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace E7.Introloop
{
    [System.Serializable]
    internal class IntroloopException : System.Exception
    {
        public IntroloopException() { }
        public IntroloopException(string message) : base(message) { }
        public IntroloopException(string message, System.Exception inner) : base(message, inner) { }
        protected IntroloopException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}