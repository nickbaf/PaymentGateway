using System;
using System.Collections.Generic;
using System.Security;

namespace PaymentGateway
{
    
    public class Card : ICreditCardValidation
    {
        public string Number { get; set; }
        public ExpirationMonthAndYear ExpirationMonthAndYear { get; set; }
        public string CVV { get; set; }
        public Card(string number, ExpirationMonthAndYear expiration,string cvv)
        {
            Number = number;
            ExpirationMonthAndYear = expiration;
            CVV = cvv;
        }

        public Card()
        {

        }

        /// <summary>
        /// From the tiny knowledge I have on payment systems:
        /// In the book "The Phoenix Project"(great book for software engineers)
        /// its mentioned that systems should never store the CVV after
        /// the initial check. So I'm removing the reference after the initial check.
        /// </summary>
        public bool CheckCVV() {
            bool result = short.TryParse(CVV, out _);
            CVV = null;
            return result;
        }

        public bool? CheckExpiry()
        {
            return ExpirationMonthAndYear?.CardHasNotExpiredYet();
        }


        ~Card()
        {
            CVV = null;
            
        }

        public bool IsCreditCardValid(out List<String> errors)
        {
            ICreditCardValidation validator = this;
            return validator.CheckCard(this, out errors);
        }

 
    }

    public class ExpirationMonthAndYear
    {
        public string Month { get; set; }
        public string Year { get; set; }

        public ExpirationMonthAndYear(string month, string year)
        {
            Month = month;
            Year = year;
        }

        public ExpirationMonthAndYear()
        {

        }

        public bool CardHasNotExpiredYet()
        {
            if(!int.TryParse(Year,out _) && !int.TryParse(Month,out _))
            {
                return false;
            }
            DateTime Dt = DateTime.Now;
            try
            {
                if (int.Parse(Year) > (Dt.Year % 100))
                {
                    return true;
                }
                else if (int.Parse(Year) == (Dt.Year % 100))
                {
                    if (int.Parse(Month) >= Dt.Month)
                    {
                        return true;
                    }
                }
            }
            catch (FormatException ex)
            {
                return false;
            }
            return false;
        }
    }
}
