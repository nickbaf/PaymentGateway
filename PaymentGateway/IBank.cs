using System;
using System.Threading;
using System.Threading.Tasks;
using PaymentGateway.Commands;
using PaymentGateway.Events;

namespace PaymentGateway
{
    /// <summary>
    /// Class that simulates a direct link to a bank.
    /// Used thread.Yield() so as to simulate a delay as we transfer and receive
    /// data from the bank.
    /// </summary>
    public static class Bank 
    {
        public static int SendCommand(ICommand<IEvent> command)
        {
            Thread.Yield();
            return 200;
        }
    }
}
