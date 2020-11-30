using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway
{
    /// <summary>
    /// Interface that defines the two transaction tasks.
    /// </summary>
    public interface ITransaction
    {
        public Task<List<String>> CaptureTransaction(Money moneyToCapture);
        public Task<List<String>> RefundTransaction(Money moneyToRefund);
    }
}
