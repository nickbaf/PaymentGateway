using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway
{
    public class Money
    {
        private float Amount; //float vs string... Choosing float for memory performance
        private string Currency;


        public Money(float amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public Money(float amount)
        {
            Amount = amount;
        }

        public void SetCurrency(string currency)
        {
            Currency = currency;
        }

        public bool ValidateAmount(out List<String> errors)
        {
            errors = new List<string>();
            if (Amount == 0)
            {
                errors.Add("Amount is zero.");
            }
            else if (Amount < 0)
            {
                errors.Add("Amount is negative.");
            }
            return errors.Any();

        }

    }

}
