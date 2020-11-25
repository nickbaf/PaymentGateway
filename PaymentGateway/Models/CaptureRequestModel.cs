using System;
namespace PaymentGateway.Models
{
    public class CaptureRequestModel
    {
        public CaptureRequestModel(Guid transactionID, Money money)
        {
            TransactionID = transactionID;
            Money = money;
        }

        public Guid TransactionID { get; }
        public Money Money { get; }
        
    }
}
