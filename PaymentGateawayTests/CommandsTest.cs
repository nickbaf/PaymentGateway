using System;
using NUnit.Framework;
using NFluent;
using PaymentGateway;
using PaymentGateway.Commands;
using PaymentGateway.Events;

namespace PaymentGateawayTests
{
    [TestFixture]
    public class CommandsTest
    {
        Card c;
        Money m;
        Transaction t;
        [SetUp]
        public void SetUp()
        {
            c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "25"), "123");
            m = new Money(156.33F, "JPY");
            t = new Transaction(new TransactionID(),c,m);
        }

        [Test]
        public void ValidAuthorizationCommand()
        {
            Check.That(new AuthorizeCommand(new TransactionID(), c, m).Execute().Result).IsInstanceOf<AuthorizationSuccessEvent>();
        }


        [Test]
        public void InvalidAuthorizationCommand()
        {
            Check.That(new AuthorizeCommand(new TransactionID(),new Card("INVALID CARD",null,"XXXX"), m).Execute().Result).IsInstanceOf<AuthorizationFailedEvent>();
        }


        [Test]
        public void ValidCaptureCommand()
        {
            Check.That(new CaptureCommand(t.TransactionID,m,t).Execute().Result).IsInstanceOf<CaptureSuccessEvent>();

        }

        [Test]
        public void InvalidCaptureCommand_BecauseNoID()
        {
            Check.That(new CaptureCommand(null, null, null).Execute().Result).IsInstanceOf<CaptureFailedEvent>();
        }

        [Test]
        public void InvalidCaptureCommand_BecauseNullMoney()
        {
            Check.That(new CaptureCommand(t.TransactionID, null, t).Execute().Result).IsInstanceOf<CaptureFailedEvent>();


        }

        [Test]
        public void InvalidCaptureCommand_BecauseNullTransaction()
        {
            Check.That(new CaptureCommand(t.TransactionID, m, null).Execute().Result).IsInstanceOf<CaptureFailedEvent>();


        }

        [Test]
        public void ValidVoidCommand()
        {

        }

        [Test]
        public void InvalidVoidCommand()
        {

        }

        [Test]
        public void ValidRefundCommand()
        {

        }

        [Test]
        public void InValidRefundCommand()
        {

        }





    }
}
