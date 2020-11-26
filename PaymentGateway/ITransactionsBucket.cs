

namespace PaymentGateway
{
    public interface ITransactionsBucket
    {
        bool CreateTransactionRecord(Transaction transaction);
        Transaction RetrieveTransactionRecord(TransactionID transaction);
    }
}
