using System;
namespace PaymentGateway.Models
{
    public class CaptureRequestModel
    {
        public CaptureRequestModel(TransactionID transactionID, Money money)
        {
            TransactionID = transactionID;
            Money = money;
        }

        public TransactionID TransactionID { get; }
        public Money Money { get; }
        
    }
}
