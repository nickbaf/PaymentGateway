using System;
using System.Collections.Concurrent;

namespace PaymentGateway
{
    public class TransactionBucket: ITransactionsBucket
    {
        private ConcurrentDictionary<TransactionID, Transaction> Bucket=new ConcurrentDictionary<TransactionID, Transaction>();

       
public bool CreateTransactionRecord(Transaction transaction)
        {
            if (!Bucket.TryAdd(transaction.TransactionID, transaction)){
                return false;            }
            return true;
        }

 
      public Transaction RetrieveTransactionRecord(TransactionID transactionID)
        {
            Bucket.TryGetValue(transactionID,out Transaction t);
            if (t == null)
            {
                return null;
            }
            return t;
        }

     
    }
}
