using System;
namespace PaymentGateway
{
    public class TransactionID
    {
        public Guid ID { get; }

        public TransactionID(Guid iD)
        {
            ID = iD;
        }

        public TransactionID()
        {
            ID = new Guid();
        }

       
        public override bool Equals(Object obj)
        {
            return ID.Equals((obj as TransactionID).ID);
        }
        //overriding equals must override hascode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
