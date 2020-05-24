using System;

namespace ServiceInterfaces.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public string UserName { get; }

        public UserAlreadyExistsException(string userName) : this(userName, null)
        {
        }

        public UserAlreadyExistsException(string userName, Exception innerException) :
            base($"User {userName} already exists", innerException)
        {
            UserName = userName;
        }

        public UserAlreadyExistsException() : this("<not specified>")
        {
        }
    }
}