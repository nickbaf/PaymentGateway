using System;
namespace PaymentGateway
{
    /// <summary>
    /// Interface for creating guids.
    /// Critical for transaction IDs.
    /// </summary>
    public interface IGuid
    {
        Guid Create();
    }
}
