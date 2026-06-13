namespace Obstor;

/// <summary>
/// The base exception type for all errors raised by the Obstor SDK.
/// Derive from this class to create SDK-specific exception types.
/// </summary>
public abstract class ObstorException : Exception
{
    internal ObstorException()
    {
    }

    internal ObstorException(string message) : base(message)
    {
    }

    internal ObstorException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
