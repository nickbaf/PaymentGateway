using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway
{
    public class Transaction:ITransaction //IBANK? NOT NBG's internet banking platform
    {
        public TransactionID TransactionID { get; }
        public Money Money { get; }
        public Card Card { get; }
        public Money AlreadyCapturedMoney { get; } 

        public Transaction(TransactionID transactionID, Card card, Money money)
        {
            TransactionID = transactionID;
            Money = money;
            Card = card;
            AlreadyCapturedMoney = new Money(0, money.Currency);
        }


        public async Task<List<String>> CaptureTransaction(Money moneyToCapture)
        {
            List<String> errorList = new List<string>();
            try
            {             
                Money.Amount -= moneyToCapture.Amount;
                AlreadyCapturedMoney.Amount += moneyToCapture.Amount;
                //send something to bank no?

            }catch(TimeoutException ex)
            {
                errorList.Add("Error while capturing transaction");
            }
            return errorList;
        }

        public async Task<List<String>> RefundTransaction(Money moneyToCapture)
        {
            List<String> errorList = new List<string>();
            try
            {
                Money.Amount += moneyToCapture.Amount;
                AlreadyCapturedMoney.Amount -= moneyToCapture.Amount;
                //send something to bank no?

            }
            catch (TimeoutException ex)
            {
                errorList.Add("Error while refunding transaction");
            }
            return errorList;
        }
    }
}
