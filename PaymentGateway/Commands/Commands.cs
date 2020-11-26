using System;
using System.Collections.Generic;
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
        private Guid TransactionID;
        private Money Money;
        public Task<IEvent> Execute()
        {
            throw new NotImplementedException();
        }
    }




    public class VoidCommand : ICommand<IEvent>
    {
        private Guid TransactionID;
        public Task<IEvent> Execute()
        {
            throw new NotImplementedException();
        }
    }




    public class RefundCommand : ICommand<IEvent>
    {
        private Guid TransactionID;
        private Money Money;
        public Task<IEvent> Execute()
        {
            throw new NotImplementedException();
        }
    }

}
