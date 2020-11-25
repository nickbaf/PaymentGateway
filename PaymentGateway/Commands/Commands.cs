using System;
using PaymentGateway.Events;

namespace PaymentGateway.Commands
{
    public abstract class BaseCommand:ICommand
    {
        public abstract BaseEvent Execute();

    }

    public class AuthorizeCommand : BaseCommand
    {
        private Card Card;
        private Money Money;
        public override BaseEvent Execute()
        {
            throw new NotImplementedException();
        }
    }



    public class CaptureCommand : BaseCommand
    {
        private Guid TransactionID;
        private Money Money;
        public override BaseEvent Execute()
        {
            throw new NotImplementedException();
        }
    }




    public class VoidCommand : BaseCommand
    {
        private Guid TransactionID;
        public override BaseEvent Execute()
        {
            throw new NotImplementedException();
        }
    }




    public class RefundCommand : BaseCommand
    {
        private Guid TransactionID;
        private Money Money;
        public override BaseEvent Execute()
        {
            throw new NotImplementedException();
        }
    }

}
