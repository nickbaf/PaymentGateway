using System;
namespace PaymentGateway.Events
{
    /// <summary>
    /// Interface for the Events aka responses that our API sents back to the client.
    /// They must implement FormatError which sents back a formated message of the error that
    /// occured.
    /// </summary>
    public interface IEvent
    {

        public string FormatError(string cardNumber, ErrorList errors);
    }
}
