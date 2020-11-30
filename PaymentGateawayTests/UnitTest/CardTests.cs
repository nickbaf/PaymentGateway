using NFluent;
using NUnit.Framework;
using PaymentGateway;

namespace PaymentGateawayTests
{
    [TestFixture]
    public class CardValidationShould
    {

        [Test]
        public void RefuseExpiredCard()
        {
            ExpirationMonthAndYear ex = new ExpirationMonthAndYear("05", "20");
            Check.That(ex.CardHasNotExpiredYet()).IsFalse();
        }


        [Test]
        public void AcceptNonExpiredCard()
        {
            ExpirationMonthAndYear ex = new ExpirationMonthAndYear("03", "21");
            Check.That(ex.CardHasNotExpiredYet()).IsTrue();
        }

        [Test]
        public void RefuseInvalidCard()
        {
            ExpirationMonthAndYear ex = new ExpirationMonthAndYear("03", "21");
            Card c = new Card("4000000000000000",ex,"123");
            Check.That(c.IsCreditCardValid(out _)).IsFalse();
        }


        [Test]
        public void AcceptCreditCardExpiringThisMonth()
        {
            ExpirationMonthAndYear ex = new ExpirationMonthAndYear(System.DateTime.Now.Month.ToString(), System.DateTime.Now.Year.ToString());
            Card c = new Card("5186124094923094", ex, "123");
            Check.That(c.IsCreditCardValid(out _)).IsTrue();
        }

        [Test]
        public void RefuseNullCreditcard()
        {
             Card c = new Card(null,null,null);
            Check.That(c.IsCreditCardValid(out _)).IsFalse();
        }



    }
}
