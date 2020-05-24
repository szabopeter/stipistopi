using System;

namespace ServiceInterfaces.Exceptions
{
    public class UserDoesNotExistException : Exception
    {
        public string UserName { get; }

        public UserDoesNotExistException(string userName) : this(userName, null)
        {
        }

        public UserDoesNotExistException(string userName, Exception innerException) :
            base($"User {userName} does not exist", innerException)
        {
            UserName = userName;
        }

        public UserDoesNotExistException() : this("<not specified>")
        {
        }
    }
}