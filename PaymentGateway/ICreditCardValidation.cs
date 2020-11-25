using System;
using System.Collections.Generic;
using System.Security;
using System.Linq;

namespace PaymentGateway
{
    public interface ICreditCardValidation
    {


        public sealed bool LuhnCheck(Card c)
        {
            return true;
        }

        public sealed bool BasicCVVCheck(Card c)
        {
            return c.CheckCVV();
        }

        public sealed bool CheckExpirationDate(Card c)
        {
            return c.CheckExpiry();
        }

        public sealed bool CheckCard(Card c,out List<String> errors)
        {
            errors = new List<string>();
            if (!LuhnCheck(c))
            {
                errors.Add("Credit card number invalid");
            }
            if (!BasicCVVCheck(c))
            {
                errors.Add("CVV number invalid");
            }
            if (!CheckExpirationDate(c))
            {
                errors.Add("Credit card expired");
            }

            return errors.Any();
        }

    }
}
