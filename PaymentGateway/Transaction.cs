using System;
namespace PaymentGateway
{
    public class Transaction:ITransaction
    {
        private Guid TransactionID { get; }
        private Money Money { get; }
        private Card Card { get; }

        public Transaction(Guid transactionID, Money money, Card card)
        {
            TransactionID = transactionID;
            Money = money;
            Card = card;
        }

       
    }
}
