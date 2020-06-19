using System;

namespace ServiceInterfaces.Exceptions
{
    public class OptimisticLockingException : StipiStopiException
    {
        public string UserName { get; }

        public OptimisticLockingException(string userName) : this(userName, null)
        {
        }

        public OptimisticLockingException(string userName, Exception innerException) :
            base($"Invalid password for user {userName}", innerException)
        {
            UserName = userName;
        }

        public OptimisticLockingException() : this("<not specified>")
        {
        }
    }
}