using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Events;

namespace PaymentGateway.Commands
{
    //public abstract class BaseCommand : ICommand
    //{
    //    public abstract BaseEvent Execute();

    //    IEvent ICommand.Execute();

    //}

    public class AuthorizeCommand : ICommand<IEvent>
    {
        public  Card Card;
        public  Money Money;
        public  TransactionID TransactionID;
        public  ErrorList Errors = new ErrorList();

        public AuthorizeCommand(TransactionID tID,Card card, Money money)
        {
            TransactionID = tID;
            Card = card;
            Money = money;
        }

        //TODO make bank Interface to simulate lag
        public async Task<IEvent> Execute()
        {
            bool ValidCard = Card.IsCreditCardValid(out List<String> CardChecks);
            bool ValidMoney = Money.ValidateAmount(out List<String> MoneyChecks);
            Errors.MultiErrorsThrown(CardChecks);
            Errors.MultiErrorsThrown(MoneyChecks);

            if(ValidCard && ValidMoney)
            {
                return new AuthorizationSuccessEvent(TransactionID, Card.Number, Money);
            }
            return new AuthorizationFailedEvent(TransactionID, Card.Number, Money,Errors);
        }
    }



    public class CaptureCommand : ICommand<IEvent>
    {
        public TransactionID TransactionID; //obsolete
        public Money MoneyToCapture;
        public Transaction Transaction;
        public ErrorList Errors = new ErrorList();

        public CaptureCommand(TransactionID transactionID, Money money,Transaction transaction)
        {
            TransactionID = transactionID;
            MoneyToCapture = money;
            Transaction = transaction;
        }

        public async Task<IEvent> Execute()
        {
            List<String> MoneyToBeCapturedChecks = new List<string>();
            bool? ValidMoneyTransaction = Transaction?.Money?.ValidateMoneyToCapture(MoneyToCapture, out MoneyToBeCapturedChecks);
            Errors.MultiErrorsThrown(MoneyToBeCapturedChecks);
            if (ValidMoneyTransaction!=null&& ValidMoneyTransaction!=false)
            {
                List<String>? ValidTransactionErrors = await Transaction?.CaptureTransaction(MoneyToCapture);
                Errors.MultiErrorsThrown(ValidTransactionErrors);
                if (ValidMoneyTransaction!=null && !ValidTransactionErrors.Any())
                {
                    return new CaptureSuccessEvent(Transaction.Card.Number,Transaction.Money);
                }
                
            }

            return new CaptureFailedEvent(Transaction?.Card?.Number, Transaction?.Money, Errors);

        }
    }




    public class VoidCommand : ICommand<IEvent>
    {
        public Transaction Transaction;
        public ErrorList Errors = new ErrorList();

        public VoidCommand(Transaction transaction)
        {
            Transaction = transaction;
        }

        public async Task<IEvent> Execute()
        {
            //contact bank
            if(Transaction.Strategy is not null)
            {
               ErrorList errorList= new ErrorList();
                errorList.SingleErrorThrown("Void failed as a capture/refund has been made.");
                return new VoidFailedEvent(Transaction.Card.Number, Transaction.Money,errorList);
            }
            return new VoidSuccessEvent(Transaction.Card.Number,Transaction.Money);

        }
    }




    public class RefundCommand : ICommand<IEvent>
    {
        public Money MoneyToRefund;
        public Transaction Transaction;
        public ErrorList Errors = new ErrorList();

        public RefundCommand(Money moneyToRefund, Transaction transaction)
        {
            MoneyToRefund = moneyToRefund;
            Transaction = transaction;
        }

        public async Task<IEvent> Execute()
        {
            bool ValidMoneyTransaction = Transaction.AlreadyCapturedMoney.ValidateMoneyToCapture(MoneyToRefund, out List<string> MoneyToBeRefundedChecks);
            Errors.MultiErrorsThrown(MoneyToBeRefundedChecks);
            if (ValidMoneyTransaction)
            {
                List<String> ValidTransactionErrors = await Transaction.RefundTransaction(MoneyToRefund);
                Errors.MultiErrorsThrown(ValidTransactionErrors);
                if (!ValidTransactionErrors.Any())
                {
                    return new RefundSuccessEvent(Transaction.Card.Number, MoneyToRefund);
                }

            }
            return new RefundFailedEvent(Transaction.Card.Number, MoneyToRefund, Errors);
        }
    }

}
