using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway
{
    public class Transaction : ITransaction //IBANK? NOT NBG's internet banking platform
    {
        public TransactionID TransactionID { get; }
        public Money Money { get; }
        public Card Card { get; }
        public Money AlreadyCapturedMoney { get; set; }
        public ITransactionStrategy Strategy { get; set; }

        public Transaction(TransactionID transactionID, Card card, Money money)
        {
            TransactionID = transactionID;
            Money = money;
            Card = card;
            AlreadyCapturedMoney = new Money(0, money.Currency);
            Strategy = null;
        }


        public async Task<List<String>> CaptureTransaction(Money moneyToCapture)
        {
            if (Strategy is null)
            {
                Strategy = new CaptureStrategy();
            }
            if(Strategy is RefundStrategy)
            {
                return new List<String>() { "Cannot capture money after a refund has been made." };
            }
           return await Strategy.DoTheTransaction(this, moneyToCapture);
            
        }

        public async Task<List<String>> RefundTransaction(Money moneyToRefund)
        {
            Strategy = new RefundStrategy();
            return await Strategy.DoTheTransaction(this,moneyToRefund);
        }


        public void SetAlreadyCapturedMoney(Money m)
        {
            AlreadyCapturedMoney = m;
        }

    }

    public interface ITransactionStrategy {

        public Task<List<String>> DoTheTransaction(Transaction transaction, Money transactionMoney);

    }


    public class CaptureStrategy : ITransactionStrategy
    {
        public async Task<List<string>> DoTheTransaction(Transaction transaction, Money transactionMoney)
        {
            List<String> errorList = new List<string>();
            try
            {
                if (transactionMoney.ValidateAmount(out _) && transaction.Money.ValidateMoneyToCapture(transactionMoney, out _))
                {
                    transaction.Money.Amount -= transactionMoney.Amount;
                    transaction.AlreadyCapturedMoney.Amount += transactionMoney.Amount;
                }
                else
                {
                    throw new Exception("Internal Checks Failed.");
                }
                //send something to bank no?

            }
            catch (Exception ex)
            {
                errorList.Add("Error while capturing transaction");

            }
            return errorList;
        }
    }

    public class RefundStrategy : ITransactionStrategy
    {
        public async Task<List<string>> DoTheTransaction(Transaction transaction, Money transactionMoney)
        {
            List<String> errorList = new List<string>();
            try
            {
                if (transactionMoney.ValidateAmount(out _) && transaction.AlreadyCapturedMoney.ValidateMoneyToRefund(transactionMoney, out _))
                {
                    transaction.Money.Amount += transactionMoney.Amount;
                    transaction.AlreadyCapturedMoney.Amount -= transactionMoney.Amount;
                    //send something to bank no?
                }
                else
                {
                    throw new Exception("Internal Checks Failed.");
                }
            }
            catch (Exception ex)
            {
                errorList.Add("Error while refunding transaction");
            }
            return errorList;
        }
    }


}
