using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Events;

namespace PaymentGateway.Commands
{

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

        public async Task<IEvent> Execute()
        {
            List<String> ErrorChecks=new List<string>() ;
            bool? ValidCard = Card?.IsCreditCardValid(out ErrorChecks);
            Errors.MultiErrorsThrown(ErrorChecks);
            bool? ValidMoney = Money?.ValidateAmount(out ErrorChecks);         
            Errors.MultiErrorsThrown(ErrorChecks);

            if(ValidCard!=null && ValidCard!=false && ValidMoney!=null && ValidMoney!=false)
            {
                SentCommandToBank(this);
                return new AuthorizationSuccessEvent(TransactionID, Card.Number, Money);
            }
            return new AuthorizationFailedEvent(TransactionID, Card?.Number, Money,Errors);
        }

        public void SentCommandToBank(ICommand<IEvent> command)
        {

            Bank.SendCommand(command);
        }

     
    }



    public class CaptureCommand : ICommand<IEvent>
    {
        public TransactionID TransactionID; 
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
            bool? IsValidMoneyTransaction = Transaction?.Money?.ValidateMoneyToCapture(MoneyToCapture, out MoneyToBeCapturedChecks);
            Errors.MultiErrorsThrown(MoneyToBeCapturedChecks);
            if (IsValidMoneyTransaction != null && IsValidMoneyTransaction != false)
            {
                List<String>? ValidTransactionErrors = await Transaction?.CaptureTransaction(MoneyToCapture);
                Errors.MultiErrorsThrown(ValidTransactionErrors);
                if (IsValidMoneyTransaction is not null && !ValidTransactionErrors.Any())
                {
                    SentCommandToBank(this);
                    return new CaptureSuccessEvent(Transaction.Card.Number,Transaction.Money);
                }
                
            }

            return new CaptureFailedEvent(Transaction?.Card?.Number, Transaction?.Money, Errors);

        }

        public void SentCommandToBank(ICommand<IEvent> command)
        {

            Bank.SendCommand(command);
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
            if(Transaction is not null && Transaction.Strategy is not null)
            {
               ErrorList errorList= new ErrorList();
                errorList.SingleErrorThrown("Void failed as a capture/refund has been made.");
                return new VoidFailedEvent(Transaction?.Card?.Number, Transaction?.Money,errorList);
            }
            SentCommandToBank(this);
            return new VoidSuccessEvent(Transaction?.Card?.Number,Transaction?.Money);

        }

        public void SentCommandToBank(ICommand<IEvent> command)
        {

            Bank.SendCommand(command);
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
            List<string> MoneyToBeRefundedChecks = new List<string>();
            bool? IsValidMoneyTransaction = Transaction?.AlreadyCapturedMoney?.ValidateMoneyToRefund(MoneyToRefund, out  MoneyToBeRefundedChecks);
            Errors.MultiErrorsThrown(MoneyToBeRefundedChecks);
            if (IsValidMoneyTransaction is not null && IsValidMoneyTransaction == true)
            {
                List<String> ValidTransactionErrors = await Transaction.RefundTransaction(MoneyToRefund);
                Errors.MultiErrorsThrown(ValidTransactionErrors);
                if (!ValidTransactionErrors.Any())
                {
                    SentCommandToBank(this);
                    return new RefundSuccessEvent(Transaction.Card.Number, MoneyToRefund);
                }

            }
            return new RefundFailedEvent(Transaction?.Card?.Number, MoneyToRefund, Errors);
        }

        public void SentCommandToBank(ICommand<IEvent> command)
        {

            Bank.SendCommand(command);
        }

        
    }

}
