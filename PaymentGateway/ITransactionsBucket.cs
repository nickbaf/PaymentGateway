

using System;
using System.Collections.Concurrent;

namespace PaymentGateway
{
    public interface ITransactionsBucket
    {
        void CreateTransactionRecord(Transaction transaction);
        bool RetrieveTransactionRecord(TransactionID transactionID,out Transaction transaction);
        bool PutTransactionRecord(Transaction transaction);
        
    }
}
