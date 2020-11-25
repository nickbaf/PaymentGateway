using System;
using System.Security;

namespace PaymentGateway
{
    
    public class Card : ICreditCardValidation
    {
        private string Number;
        private ExpirationMonthAndYear ExpirationMonthAndYear;
        private SecureString CVV;
        public Card(string number, ExpirationMonthAndYear expiration,string cvv)
        {
            Number = number;
            ExpirationMonthAndYear = expiration;
            CVV = new SecureString();
            foreach(char c in cvv.ToCharArray()){
                CVV.AppendChar(c);
            }
        }

        public bool CheckCVV() {
            return CVV==null?false:true;
        }

        public bool CheckExpiry()
        {
            return ExpirationMonthAndYear.CardHasNotExpiredYet();
        }


        ~Card()
        {
            CVV.Dispose();
            
        }

        public bool IsCreditCardValid()
        {
            ICreditCardValidation validator = this;
            return validator.CheckCard(this, out _);
        }

 
    }

    public class ExpirationMonthAndYear
    {
        private readonly string Month;
        private readonly string Year;

        public ExpirationMonthAndYear(string month, string year)
        {
            Month = month;
            Year = year;
        }

        public bool CardHasNotExpiredYet()
        {
            DateTime Dt = DateTime.Now;
            if(int.Parse(Year) > (Dt.Year % 100))
            {
                return true;
            }else if(int.Parse(Year) == (Dt.Year % 100))
            {
                if (int.Parse(Month) >= Dt.Month)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
