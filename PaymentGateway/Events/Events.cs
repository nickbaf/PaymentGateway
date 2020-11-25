using System;
namespace PaymentGateway.Events
{
    public abstract class BaseEvent:IEvent
    {
        protected BaseEvent(string cardNumber, Money amountAndCurrencyAvailable)
        {
            CardNumber = cardNumber;
            AmountAndCurrencyAvailable = amountAndCurrencyAvailable;
        }

        public string CardNumber { get; }
        public Money AmountAndCurrencyAvailable { get; }


    }


    public class AuthorizeFailedEvent : BaseEvent
    {
        Guid TransactionID;
        //maybe we need reason TODO!
        public AuthorizeFailedEvent(Guid transactionID,string cardNumber,Money money):base(cardNumber,money)
        {
            TransactionID = transactionID;
        }
    }

    public class AuthorizationSuccessEvent: BaseEvent
    {
        Guid TransactionID;

        public AuthorizationSuccessEvent(Guid transactionID, string cardNumber, Money money) : base(cardNumber, money)
        {
            TransactionID = transactionID;
        }
    }



    public class CaptureFailedEvent : BaseEvent
    {
        public CaptureFailedEvent(string cardNumber,Money money) : base(cardNumber, money) { }
    }

    public class CaptureSuccessEvent : BaseEvent
    {
        public CaptureSuccessEvent(string cardNumber, Money money) : base(cardNumber, money) { }
    }



    public class VoidFailedEvent : BaseEvent
    {
        public VoidFailedEvent(string cardNumber, Money money) : base(cardNumber, money) { }
    }
    public class VoidSuccessEvent : BaseEvent
    {
        public VoidSuccessEvent(string cardNumber, Money money) : base(cardNumber, money) { }
    }


    public class RefundFailedEvent : BaseEvent
    {
        public RefundFailedEvent(string cardNumber, Money money) : base(cardNumber, money) { }
    }
    public class RefundSuccessEvent : BaseEvent
    {
        public RefundSuccessEvent(string cardNumber, Money money) : base(cardNumber, money) { }
    }

}
