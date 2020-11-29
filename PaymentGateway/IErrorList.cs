using System;
using System.Collections.Generic;

namespace PaymentGateway
{
    /// <summary>
    /// Interface for storing Errors produced
    /// </summary>
    /// <typeparam name="List"></typeparam>
    public interface IErrorList <List>
    {
        public void SingleErrorThrown(string error);
        public void MultiErrorsThrown(List<String> errors);
        public List<String> RetrieveErrorList();        
    }
}
