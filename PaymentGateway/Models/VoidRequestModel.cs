using System;
namespace PaymentGateway.Models
{
    public class VoidRequestModel
    {
        public VoidRequestModel(TransactionID transactionID)
        {
            TransactionID = transactionID;
        }

        public TransactionID TransactionID { get; }

    }
}
