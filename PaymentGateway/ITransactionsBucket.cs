

using System;
using System.Collections.Concurrent;

namespace PaymentGateway
{
    /// <summary>
    /// Interface that describes the in memory object's functions for storing and
    /// retrieving transactions
    /// </summary>
    public interface ITransactionsBucket
    {
        void CreateTransactionRecord(Transaction transaction);
        bool RetrieveTransactionRecord(TransactionID transactionID,out Transaction transaction);
        bool PutTransactionRecord(Transaction transaction);
        
    }
}
