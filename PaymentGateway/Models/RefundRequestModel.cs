using System;
namespace PaymentGateway.Models
{
    public class RefundRequestModel
    {
        public RefundRequestModel(Guid transactionID, Money money)
        {
            TransactionID = transactionID;
            Money = money;
        }

        public Guid TransactionID { get; }
        public Money Money { get; }
    }
}
