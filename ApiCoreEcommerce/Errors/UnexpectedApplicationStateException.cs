using System;

namespace ApiCoreEcommerce.Errors
{
    public class UnexpectedApplicationStateException : Exception
    {
        public UnexpectedApplicationStateException(string message): base(message)
        {
            
        }

        public UnexpectedApplicationStateException()
        {
            
        }
    }
}