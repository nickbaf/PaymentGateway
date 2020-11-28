using System;
namespace PaymentGateway
{
    public class LogConvention
    {
        Guid? ID;
        string Message;

        public LogConvention( Guid? iD, string message)
        {
            ID = iD;
            Message = message;
        }


        public String ToString()
        {
            return $"|Called with ID({ID.ToString().ToUpper()})||{Message}";
       }
    }
}
