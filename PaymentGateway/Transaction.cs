using System;
namespace PaymentGateway
{
    public class Transaction:ITransaction
    {
        public TransactionID TransactionID { get; }
        public Money Money { get; }
        public Card Card { get; }

        public Transaction(TransactionID transactionID, Card card, Money money)
        {
            TransactionID = transactionID;
            Money = money;
            Card = card;
        }

       
    }
}
