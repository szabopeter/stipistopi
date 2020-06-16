using System;

namespace ServiceInterfaces.Exceptions
{
    public class InsufficientRoleException : StipiStopiException
    {
        public string UserName { get; }

        public InsufficientRoleException(string userName) : this(userName, null)
        {
        }

        public InsufficientRoleException(string userName, Exception innerException) :
            base($"User {userName} does not have the necessary role", innerException)
        {
            UserName = userName;
        }

        public InsufficientRoleException() : this("<not specified>")
        {
        }
    }
}