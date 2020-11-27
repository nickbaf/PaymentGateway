using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway
{
    public interface ITransaction
    {
        public Task<List<String>> CaptureTransaction(Money moneyToCapture);
        public Task<List<String>> RefundTransaction(Money moneyToRefund);
    }
}
