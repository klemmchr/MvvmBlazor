using System;
using System.Runtime.Serialization;

namespace MvvmBlazor.Internal.Bindings
{
    public class BindingException : Exception
    {
        public BindingException() { }

        protected BindingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public BindingException(string message) : base(message) { }

        public BindingException(string message, Exception innerException) : base(message, innerException) { }
    }
}