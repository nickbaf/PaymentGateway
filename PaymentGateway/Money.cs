using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway
{
    public class Money
    {
        public float Amount { get; set; } //float vs string... Choosing float for memory performance
        public string Currency { get; set; }


        public Money(float amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public Money(float amount)
        {
            Amount = amount;
        }

        public Money()
        {

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
            return !errors.Any();

        }

    }

}
