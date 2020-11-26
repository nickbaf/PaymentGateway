using System;
namespace PaymentGateway
{
    public class TransactionIDGenerator:IGuid
    {
        

        public Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}
