using System;
namespace PaymentGateway
{
    public class TransactionID : IGuid
    {
        private Guid ID { get; }

        public TransactionID(Guid iD)
        {
            ID = iD;
        }

        public Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}
