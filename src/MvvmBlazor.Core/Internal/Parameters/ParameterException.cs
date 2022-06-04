namespace MvvmBlazor.Internal.Parameters;

[Serializable]
public class ParameterException : Exception
{
    public ParameterException() { }
    protected ParameterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public ParameterException(string? message) : base(message) { }
    public ParameterException(string? message, Exception? innerException) : base(message, innerException) { }
}