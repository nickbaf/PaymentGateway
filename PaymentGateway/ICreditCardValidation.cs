using System;
using System.Collections.Generic;
using System.Security;
using System.Linq;

namespace PaymentGateway
{
    /// <summary>
    /// Interface that Validates a credit card
    /// </summary>
    public interface ICreditCardValidation
    {


        public sealed bool LuhnCheck(Card c)
        {
             
            String CardNumber = string.IsNullOrEmpty(c?.Number)?string.Empty:c.Number.Replace(" ","");

            if (CardNumber.Length > 0 && short.TryParse(CardNumber[CardNumber.Length - 1].ToString(), out short sum))
            {
                short b;
                for (int i = 0; i < CardNumber.Length - 1; i++)
                {
                    //not the most elegant way I know..... :(
                    if (!short.TryParse(CardNumber[i].ToString(), out b)) return false;
                    if (i % 2 == 0) b *= 2;


                    if (b > 9) b -= 9;

                    sum += b;
                }
                return sum % 10 == 0;
            }
            return false;
        }
    

        public sealed bool BasicCVVCheck(Card c)
        {
            return c.CheckCVV();
        }

        public sealed bool? CheckExpirationDate(Card c)
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
            bool? expirationDateCheck = CheckExpirationDate(c);
            if (expirationDateCheck == null || expirationDateCheck == false )
            {
                errors.Add("Credit card expired");
            }

            return !errors.Any();
        }

    }
}
