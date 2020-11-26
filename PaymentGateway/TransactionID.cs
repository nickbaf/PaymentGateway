using System;
namespace PaymentGateway
{
    public class TransactionID
    {
        public Guid ID { get; }

        public TransactionID(Guid iD)
        {
            ID = iD;
        }
    }
}
