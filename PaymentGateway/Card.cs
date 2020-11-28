﻿using System;
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
            //foreach(char c in cvv.ToCharArray()){
            //    CVV.AppendChar(c);
            //}
        }

        public Card()
        {

        }

        public bool CheckCVV() {
            return short.TryParse(CVV,out _)?true:false;
        }

        public bool? CheckExpiry()
        {
            return ExpirationMonthAndYear?.CardHasNotExpiredYet();
        }


        ~Card()
        {
           // CVV.Dispose();
            
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
