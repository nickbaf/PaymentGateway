using System;
using System.Threading.Tasks;
using PaymentGateway.Events;

namespace PaymentGateway.Commands
{
    /// <summary>
    /// The ICommand interface includes all the actions that must be taken
    /// for each api endpoint.
    /// </summary>
    /// <typeparam name="IEvent"></typeparam>
    public interface ICommand<IEvent>
    {
        /// <summary>
        /// The execute function is used to execute a series of commands
        /// related to the received request we received
        /// </summary>
        /// <returns></returns>
        Task<IEvent> Execute();
        /// <summary>
        /// The SentcommandToBank simulates the fact that we send a command to the
        /// bank to be processed.
        /// </summary>
        /// <param name="command"></param>
        void SentCommandToBank(ICommand<IEvent> command);
    }
}
