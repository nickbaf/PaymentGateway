﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway
{
    /// <summary>
    /// Class that represents Money that includes an amount and currency.
    /// Implements IMoneyValidators for checking that amount and currency are valid as well as the
    /// captured and refunded money will result in a valid Money object.
    /// </summary>
    public class Money: IMoneyValidators
    {
        public float Amount { get; set; } 
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

            if (moneyToBeCaptured == null)
            {
                errors.Add("Capture money invalid format");
                return !errors.Any();
            }
            if (moneyToBeCaptured?.Amount == 0)
            {
                errors.Add("Amount is zero.");
            }
            else if (moneyToBeCaptured?.Amount < 0)
            {
                errors.Add("Amount is negative.");
            }

            if (!Currency.ToUpper().Equals(moneyToBeCaptured?.Currency?.ToUpper()))
            {
                errors.Add("Invalid Currency.");
            }

            if ((Amount - moneyToBeCaptured?.Amount) < 0)
            {
                errors.Add("Cannot capture more than the authorized amount.");
            }
            return !errors.Any();

        }


        public bool ValidateMoneyToRefund(Money moneyToBeRefunded, out List<String> errors)
        {
            errors = new List<string>();

            if (moneyToBeRefunded == null)
            {
                errors.Add("Refund money invalid format");
                return !errors.Any();
            }
            if (moneyToBeRefunded.Amount == 0)
            {
                errors.Add("Amount is zero.");
            }
            else if (moneyToBeRefunded.Amount < 0)
            {
                errors.Add("Amount is negative.");
            }

            if (!Currency.ToUpper().Equals(moneyToBeRefunded?.Currency?.ToUpper()))
            {
                errors.Add("Invalid Currency");
            }

            if ((Amount - moneyToBeRefunded?.Amount) < 0)
            {
                errors.Add("Cannot refund more than the amount already captured.");
            }
            return !errors.Any();

        }

        public Money Clone()
        {
            return new Money(this.Amount, this.Currency);
        }
    }

}
