using System.Runtime.Serialization;

namespace EntityFrameworkGenericRepository.Exceptions;

public class EntityWithSameIdExistsException : EntitySaveFailedException
{
    public EntityWithSameIdExistsException()
    {
    }

    protected EntityWithSameIdExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public EntityWithSameIdExistsException(string? message) : base(message)
    {
    }

    public EntityWithSameIdExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}