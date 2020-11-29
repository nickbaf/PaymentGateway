using System;
namespace PaymentGateway
{
    //A class that creates Guids
    public class TransactionIDGenerator:IGuid
    {
        

        public Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}
