using System;
namespace PaymentGateway
{
    public class CreateGuid:IGuid
    {

        public Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}
