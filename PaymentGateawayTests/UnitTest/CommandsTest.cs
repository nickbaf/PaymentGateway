using System;
using NUnit.Framework;
using NFluent;
using PaymentGateway;
using PaymentGateway.Commands;
using PaymentGateway.Events;

namespace PaymentGateawayTests
{
    [TestFixture]
    public class CommandShouldSentEventOfType
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
        public void SuccessAsAuthorizationValid()
        {
            Check.That(new AuthorizeCommand(new TransactionID(), c, m).Execute().Result).IsInstanceOf<AuthorizationSuccessEvent>();
        }


        [Test]
        public void FailiureAsAuthorizationInvalid()
        {
            Check.That(new AuthorizeCommand(new TransactionID(),new Card("INVALID CARD",null,"XXXX"), m).Execute().Result).IsInstanceOf<AuthorizationFailedEvent>();
        }


        [Test]
        public void SuccessAsCaptureValid()
        {
            Check.That(new CaptureCommand(t.TransactionID,m,t).Execute().Result).IsInstanceOf<CaptureSuccessEvent>();

        }

        [Test]
        public void FailiureAsCaptureWithNoIDInvalid()
        {
            Check.That(new CaptureCommand(null, null, null).Execute().Result).IsInstanceOf<CaptureFailedEvent>();
        }

        [Test]
        public void FailiureAsCaptureInvalidBecauseNullMoney()
        {
            Check.That(new CaptureCommand(t.TransactionID, null, t).Execute().Result).IsInstanceOf<CaptureFailedEvent>();


        }

        [Test]
        public void FailiureAsACaptureInvalidBecauseNullTransaction()
        {
            Check.That(new CaptureCommand(t.TransactionID, m, null).Execute().Result).IsInstanceOf<CaptureFailedEvent>();


        }

        [Test]
        public void SuccessAsVoidValid()
        {
            Check.That(new VoidCommand(t).Execute().Result).IsInstanceOf<VoidSuccessEvent>();
        }

        [Test]
        public void FailiureAsVoidInvalid()
        {
            Check.That(new VoidCommand(null).Execute().Result).IsInstanceOf<VoidSuccessEvent>();
        }

        [Test]
        public void SucessAsRefundValid()
        {
            t = new Transaction(new TransactionID(), c, new Money(0f, "JPY"));
            t.SetAlreadyCapturedMoney(m);
            Check.That(new RefundCommand(m,t).Execute().Result).IsInstanceOf<RefundSuccessEvent>();
        }

        [Test]
        public void FailiureAsRefundInvalidBecauseMoneyInvalid()
        {
            Check.That(new RefundCommand(new Money(float.MaxValue, null), t).Execute().Result).IsInstanceOf < RefundFailedEvent>();
        }

        [Test]
        public void FailiureAsARefundInvalid_BecauseNullArguments()
        {
            Check.That(new RefundCommand(null,null).Execute().Result).IsInstanceOf<RefundFailedEvent>();
        }



    }
}
