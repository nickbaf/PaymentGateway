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

        //credit card same month year

        

    }
}
