using System;
using System.Collections.Generic;

namespace PaymentGateway.Events
{
    /// <summary>
    /// Events are created as a response to a command. A succesfull processed command creates a success event
    /// that we push back as a response to the API request. A command that produces errors when executing will result in
    /// producing a failiure event that will be retured as a response to the API request.
    /// </summary>
    public abstract class BaseEvent:IEvent
    {
        /// <summary>
        /// As per the exercise instructions for every failed output we return the cardNumber,
        /// amount and the error message, that is the credit card number and the message.
        /// </summary>
        /// <param name="errors">A string formated as "$CreditCardNumber $error" e.x 4000 0000 0000 0119: authorisation failure</param>
        protected BaseEvent(string cardNumber, Money amountAndCurrencyAvailable,ErrorList errors)
        {
            CardNumber = cardNumber;
            AmountAndCurrencyAvailable = amountAndCurrencyAvailable;
            Errors = errors;
        }      
        public string? CardNumber { get; }
        public Money? AmountAndCurrencyAvailable { get; }
        private ErrorList? Errors { get; }

        public string FormatError(string cardNumber, ErrorList errors)
        {
            return cardNumber + " " + errors.ToOneLinerString();
        }
    }


    public class AuthorizationFailedEvent : BaseEvent
    {
        public TransactionID? TransactionID { get; }
        public string? Error { get; }
        public AuthorizationFailedEvent(TransactionID transactionID,string cardNumber,Money money,ErrorList list):base(cardNumber,money,list)
        {
            TransactionID = transactionID;
            Error = FormatError(cardNumber, list);
        }
    }

    public class AuthorizationSuccessEvent: BaseEvent
    {
        public TransactionID TransactionID { get; }

        public AuthorizationSuccessEvent(TransactionID transactionID, string cardNumber, Money money) : base(cardNumber, money,null)
        {
            TransactionID = transactionID;
        }
    }



    public class CaptureFailedEvent : BaseEvent
    {
        public string? Error { get; }
        public CaptureFailedEvent(string cardNumber,Money money,ErrorList list) : base(cardNumber, money,list) {
            Error = FormatError(cardNumber, list);
        }
    }

    public class CaptureSuccessEvent : BaseEvent
    {      
        public CaptureSuccessEvent(string cardNumber, Money money) : base(cardNumber, money,null) {
        }
    }



    public class VoidFailedEvent : BaseEvent
    {
        public string? Error { get; }
        public VoidFailedEvent(string cardNumber, Money money,ErrorList list) : base(cardNumber, money,list) {
            Error = FormatError(cardNumber, list);
        }
    }
    public class VoidSuccessEvent : BaseEvent
    {
        public VoidSuccessEvent(string cardNumber, Money money) : base(cardNumber, money,null) { }
    }


    public class RefundFailedEvent : BaseEvent
    {
        public string? Error { get; }
        public RefundFailedEvent(string cardNumber, Money money,ErrorList list) : base(cardNumber, money,list) {
            Error = FormatError(cardNumber, list);
        }
    }
    public class RefundSuccessEvent : BaseEvent
    {
        public RefundSuccessEvent(string cardNumber, Money money) : base(cardNumber, money,null) { } 
    }

}
