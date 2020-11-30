using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Events;

/// <summary>
/// Represents a series of actions that need to be taken for the action that the command represents.
/// A sussesfull command will create an SuccessEvent, otherwise a failiure event will be created.
//  Example: The AuthorizeCommand takes all the nessecary steps for performing the authorization of a transaction.
//  This includes, amount, currency and creadit card checks.
//  Execute: Instructs the object to execute the nessesary steps. Returns a success or failed event.
/// </summary>
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

        /// <summary>
        /// Executing an authorization command. Here, we have all the data we need that are parsed from the API request.
        /// The function will check for credit card errors, and validate the amount and currency given.
        /// If all the checks pass the command will contact the Mock bank and send a AuthorizationSuccessEvent.
        /// </summary>
        /// <returns></returns>
        public async Task<IEvent> Execute()
        {
            List<String> ErrorChecks=new List<string>() ;
            bool? ValidCard = Card?.IsCreditCardValid(out ErrorChecks);
            Errors.MultiErrorsThrown(ErrorChecks);
            bool? ValidMoney = Money?.ValidateAmount(out ErrorChecks);         
            Errors.MultiErrorsThrown(ErrorChecks);

            if(ValidCard!=null && ValidCard!=false && ValidMoney!=null && ValidMoney!=false)
            {
                SentCommandToBank(this); //MockBank
                return new AuthorizationSuccessEvent(TransactionID, Card.Number, Money);
            }
            return new AuthorizationFailedEvent(TransactionID, Card?.Number, Money,Errors);
        }

        /// <summary>
        /// This was a last minute addition. A Mock Bank was created to simulate a direct link
        /// to and from the Authorization bank. For simplicity and lack of time only 1 bank is supported.
        /// </summary>
        /// <param name="command"></param>
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
