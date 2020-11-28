using System;
using System.Collections.Generic;

namespace PaymentGateway.Events
{
    public abstract class BaseEvent:IEvent
    {
        protected BaseEvent(string cardNumber, Money amountAndCurrencyAvailable,ErrorList errors)
        {
            CardNumber = cardNumber;
            AmountAndCurrencyAvailable = amountAndCurrencyAvailable;
            Errors = errors;
        }

        public string? CardNumber { get; }
        public Money? AmountAndCurrencyAvailable { get; }
        public ErrorList? Errors { get; }


    }


    public class AuthorizationFailedEvent : BaseEvent
    {
        public TransactionID TransactionID { get; }
        public ErrorList Errors { get; }
        public AuthorizationFailedEvent(TransactionID transactionID,string cardNumber,Money money,ErrorList list):base(cardNumber,money,list)
        {
            TransactionID = transactionID;
            Errors = list;
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
        public CaptureFailedEvent(string cardNumber,Money money,ErrorList list) : base(cardNumber, money,list) { }
    }

    public class CaptureSuccessEvent : BaseEvent
    {
       // public Transaction Transaction { get; }
        public CaptureSuccessEvent(string cardNumber, Money money) : base(cardNumber, money,null) {
        }
    }



    public class VoidFailedEvent : BaseEvent
    {
        public VoidFailedEvent(string cardNumber, Money money,ErrorList list) : base(cardNumber, money,list) { }
    }
    public class VoidSuccessEvent : BaseEvent
    {
        public VoidSuccessEvent(string cardNumber, Money money) : base(cardNumber, money,null) { }
    }


    public class RefundFailedEvent : BaseEvent
    {
        public RefundFailedEvent(string cardNumber, Money money,ErrorList list) : base(cardNumber, money,list) { }
    }
    public class RefundSuccessEvent : BaseEvent
    {
        public RefundSuccessEvent(string cardNumber, Money money) : base(cardNumber, money,null) { } 
    }

}
