using System.Runtime.Serialization;

namespace EntityFrameworkGenericRepository.Exceptions;

public class EntitySaveFailedException : Exception
{
    public EntitySaveFailedException()
    {
    }

    protected EntitySaveFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public EntitySaveFailedException(string? message) : base(message)
    {
    }

    public EntitySaveFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}