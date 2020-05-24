using System;

namespace ServiceInterfaces.Exceptions
{
    public class ResourceDoesNotExistException : Exception
    {
        public string ResourceShortName { get; }

        public ResourceDoesNotExistException(string resourceShortName) : this(resourceShortName, null)
        {
        }

        public ResourceDoesNotExistException() : this("<not specified>")
        {
        }

        public ResourceDoesNotExistException(string resourceShortName, Exception innerException) :
            base($"Resource {resourceShortName} does not exist", innerException)
        {
            ResourceShortName = resourceShortName;
        }
    }
}