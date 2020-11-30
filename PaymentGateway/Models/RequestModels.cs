using System;
namespace PaymentGateway.Models
{

    /// <summary>
    /// Classes that represent the API Requests
    /// </summary>


    public class AuthorizationRequestModel
    {
        public Card Card { get; }
        public Money Money { get; }
        

        public AuthorizationRequestModel(Card card, Money money)
        {
            Card = card;
            Money = money;
        }
    }


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

    public class VoidRequestModel
    {
        public VoidRequestModel(TransactionID transactionID)
        {
            TransactionID = transactionID;
        }

        public TransactionID TransactionID { get; }

    }
}
