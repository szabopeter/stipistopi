using System;

namespace ServiceInterfaces.Exceptions
{
    public class InvalidPasswordException : StipiStopiException
    {
        public string UserName { get; }

        public InvalidPasswordException(string userName) : this(userName, null)
        {
        }

        public InvalidPasswordException(string userName, Exception innerException) :
            base($"Invalid password for user {userName}", innerException)
        {
            UserName = userName;
        }

        public InvalidPasswordException() : this("<not specified>")
        {
        }
    }
}