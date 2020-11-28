using System;
using System.Collections.Generic;

namespace PaymentGateway
{
    public interface IErrorList <List>
    {
        public void SingleErrorThrown(string error);
        public void MultiErrorsThrown(List<String> errors);
        public List<String> RetrieveErrorList();        
    }
}
