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
        public Money AlreadyCapturedMoney { get; set; } 

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
                if (Money.ValidateAmount(out _) && Money.ValidateMoneyToCapture(moneyToCapture, out _))
                {
                    Money.Amount -= moneyToCapture.Amount;
                    AlreadyCapturedMoney.Amount += moneyToCapture.Amount;
                }
                else
                {
                    throw new Exception("Internal Checks Failed.");
                }
                //send something to bank no?

            }catch(Exception ex)
            {
                errorList.Add("Error while capturing transaction");
                
            }
            return errorList;
        }

        public async Task<List<String>> RefundTransaction(Money moneyToRefund)
        {
            List<String> errorList = new List<string>();
            try
            {
                if (moneyToRefund.ValidateAmount(out _) && AlreadyCapturedMoney.ValidateMoneyToRefund(moneyToRefund, out _))
                {
                    Money.Amount += moneyToRefund.Amount;
                    AlreadyCapturedMoney.Amount -= moneyToRefund.Amount;
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

        
        public void SetAlreadyCapturedMoney(Money m)
        {
            AlreadyCapturedMoney = m;
        }

    }



}
