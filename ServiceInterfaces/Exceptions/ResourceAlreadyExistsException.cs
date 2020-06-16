using System;

namespace ServiceInterfaces.Exceptions
{
    public class ResourceAlreadyExistsException : StipiStopiException
    {
        public string ResourceShortName { get; }

        public ResourceAlreadyExistsException(string resourceShortName) : this(resourceShortName, null)
        {
        }

        public ResourceAlreadyExistsException() : this("<not specified>")
        {
        }

        public ResourceAlreadyExistsException(string resourceShortName, Exception innerException) :
            base($"Resource {resourceShortName} already exists", innerException)
        {
            ResourceShortName = resourceShortName;
        }
    }
}