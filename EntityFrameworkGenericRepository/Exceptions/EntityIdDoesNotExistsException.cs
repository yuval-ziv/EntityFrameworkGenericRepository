using System.Runtime.Serialization;

namespace EntityFrameworkGenericRepository.Exceptions;

public class EntityIdDoesNotExistsException : EntityUpdateFailedException
{
    public EntityIdDoesNotExistsException()
    {
    }

    protected EntityIdDoesNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public EntityIdDoesNotExistsException(string? message) : base(message)
    {
    }

    public EntityIdDoesNotExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}