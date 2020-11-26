using System;
namespace PaymentGateway.Commands
{
    public interface ICommandProcessor<in ICommand,out IEvent>
    {
        public IEvent ProcessCommand(ICommand command);
    }
}
