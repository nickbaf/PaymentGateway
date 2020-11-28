using System;
using System.Threading;
using System.Threading.Tasks;
using PaymentGateway.Commands;
using PaymentGateway.Events;

namespace PaymentGateway
{

    public static class Bank 
    {
        public static int SendCommand(ICommand<IEvent> command)
        {
            Thread.Yield();
            return 200;
        }
    }
}
