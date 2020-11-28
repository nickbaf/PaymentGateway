using System;
using System.Threading.Tasks;
using PaymentGateway.Events;

namespace PaymentGateway.Commands
{
    public interface ICommand<IEvent>
    {
        Task<IEvent> Execute();
        void SentCommandToBank(ICommand<IEvent> command);
    }
}
