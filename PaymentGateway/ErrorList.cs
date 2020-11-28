using System;
using System.Collections.Generic;
using System.Text;

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
            if (errors != null)
            {
                Errors.AddRange(errors);
            }
        }

        public List<string> RetrieveErrorList()
        {
            return Errors;
        }

        public void SingleErrorThrown(string error)
        {
            Errors.Add(error);
        }

        public string ToOneLinerString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(String s in Errors)
            {
                sb.Append('|');
                sb.Append(s);
                
            }
            return sb.ToString();
        }
    }
}
