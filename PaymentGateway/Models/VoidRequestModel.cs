using System;
namespace PaymentGateway.Models
{
    public class VoidRequestModel
    {
        public VoidRequestModel(Guid transactionID)
        {
            TransactionID = transactionID;
        }

        public Guid TransactionID { get; }

    }
}
