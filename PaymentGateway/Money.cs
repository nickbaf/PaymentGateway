using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway
{
    public class Money: IMoneyValidators
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


        public bool ValidateMoneyToCapture(Money moneyToBeCaptured, out List<String> errors)
        {
            errors = new List<string>();

            if (moneyToBeCaptured.Amount == 0)
            {
                errors.Add("Amount is zero.");
            }
            else if (moneyToBeCaptured.Amount < 0)
            {
                errors.Add("Amount is negative.");
            }

            if (!Currency.ToUpper().Equals(moneyToBeCaptured.Currency.ToUpper()))
            {
                errors.Add("Invalid Currency.");
            }

            if ((Amount - moneyToBeCaptured.Amount) < 0)
            {
                errors.Add("Cannot capture more than the authorized amount.");
            }
            return !errors.Any();

        }


        public bool ValidateMoneyToRefund(Money moneyToBeRefunded, out List<String> errors)
        {
            errors = new List<string>();

            if (moneyToBeRefunded.Amount == 0)
            {
                errors.Add("Amount is zero.");
            }
            else if (moneyToBeRefunded.Amount < 0)
            {
                errors.Add("Amount is negative.");
            }

            if (!Currency.ToUpper().Equals(moneyToBeRefunded.Currency.ToUpper()))
            {
                errors.Add("Invalid Currency");
            }

            if ((Amount - moneyToBeRefunded.Amount) < 0)
            {
                errors.Add("Cannot refund more than the amount already captured.");
            }
            return !errors.Any();

        }
    }

}
