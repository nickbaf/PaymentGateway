using System;
using System.Collections.Concurrent;

namespace PaymentGateway
{
    public class TransactionBucket: ITransactionsBucket
    {

        private ConcurrentDictionary<Guid, Transaction> Bucket = new ConcurrentDictionary<Guid, Transaction>();
        public void Initialize()
        {
            Bucket = new ConcurrentDictionary<Guid, Transaction>();

        }
        public bool PutTransactionRecord(Transaction transaction)
        {
            if (!Bucket.TryAdd(transaction.TransactionID.ID, transaction)){
                return false;            }
            return true;
        }

        public void CreateTransactionRecord(Transaction transaction)
        {
            Bucket.TryAdd(transaction.TransactionID.ID, transaction);
        }

        public bool RetrieveTransactionRecord(TransactionID transactionID, out Transaction transaction)
        {
            Bucket.TryRemove(transactionID.ID,out transaction);
            if (transaction == null)
            {
                return false;
            }
            return true;
        }

     
    }
}
