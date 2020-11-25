using System;
using System.Collections.Concurrent;

namespace PaymentGateway
{
    public class TransactionBucket: ITransactionsBucket
    {
        private ConcurrentDictionary<TransactionID, Transaction> TransactionsBucket=new ConcurrentDictionary<TransactionID, Transaction>();

    }
}
