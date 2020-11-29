using NFluent;
using NUnit.Framework;
using PaymentGateway;

namespace PaymentGateawayTests
{
    [TestFixture]
    public class CardTests
    {

        [Test]
        public void CardExpired()
        {
            ExpirationMonthAndYear ex = new ExpirationMonthAndYear("05", "20");
            Check.That(ex.CardHasNotExpiredYet()).IsFalse();
        }


        [Test]
        public void CardNotExpired()
        {
            ExpirationMonthAndYear ex = new ExpirationMonthAndYear("03", "21");
            Check.That(ex.CardHasNotExpiredYet()).IsTrue();
        }

        [Test]
        public void CardInvalid()
        {
            ExpirationMonthAndYear ex = new ExpirationMonthAndYear("03", "21");
            Card c = new Card("4000000000000000",ex,"123");
            Check.That(c.IsCreditCardValid(out _)).IsFalse();
        }

        //credit card same month year



    }
}
