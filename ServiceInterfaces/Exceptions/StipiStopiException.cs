using System;

public abstract class StipiStopiException : Exception
{
    public StipiStopiException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public StipiStopiException(string message) : base(message)
    {
    }

    public StipiStopiException() : base()
    {
    }
}