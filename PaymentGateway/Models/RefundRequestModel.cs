using System;
namespace PaymentGateway.Models
{
    public class RefundRequestModel
    {
        public RefundRequestModel(TransactionID transactionID, Money money)
        {
            TransactionID = transactionID;
            Money = money;
        }

        public TransactionID TransactionID { get; }
        public Money Money { get; }
    }
}
