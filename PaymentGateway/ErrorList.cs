using System;
using System.Collections.Generic;

namespace PaymentGateway
{ 
    /// <summary>
    /// Error list
    /// </summary>
    public class ErrorList:IErrorList<List<String>>
    {
        public List<String> Errors { get; } = new List<String>();
        public ErrorList()
        {
        }

        public void MultiErrorsThrown(List<string> errors)
        {
            Errors.AddRange(errors);
        }

        public List<string> RetrieveErrorList()
        {
            return Errors;
        }

        public void SingleErrorThrown(string error)
        {
            Errors.Add(error);
        }
    }
}
