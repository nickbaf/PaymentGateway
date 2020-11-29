using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway
{
    /// <summary>
    /// Interface that defines what are the checks for a transaction
    /// </summary>
    public interface ITransaction
    {
        public Task<List<String>> CaptureTransaction(Money moneyToCapture);
        public Task<List<String>> RefundTransaction(Money moneyToRefund);
    }
}
