using System;
namespace PaymentGateway.Models
{
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
    public class AuthorizationSuccessModel
    {
       
        public Guid TransactionID { get; set; }
        public Money Money { get; set; }

        public AuthorizationSuccessModel(Guid transactionID, Money money)
        {
            TransactionID = transactionID;
            Money = money;
        }

    }


    public class AuthorizationFailedModel
    {
        public Guid TransactionID { get; set; }
        public Money Money { get; set; }

        public AuthorizationFailedModel(Guid transactionID, Money money)
        {
            TransactionID = transactionID;
            Money = money;
        }
    }
}
