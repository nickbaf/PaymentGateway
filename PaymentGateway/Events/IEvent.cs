using System;
namespace PaymentGateway.Events
{
    public interface IEvent
    {

        public string FormatError(string cardNumber, ErrorList errors);
    }
}
