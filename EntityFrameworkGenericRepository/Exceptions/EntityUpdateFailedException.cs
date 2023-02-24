using System.Runtime.Serialization;

namespace EntityFrameworkGenericRepository.Exceptions;

public class EntityUpdateFailedException : Exception
{
    public EntityUpdateFailedException()
    {
    }

    protected EntityUpdateFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public EntityUpdateFailedException(string? message) : base(message)
    {
    }

    public EntityUpdateFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}