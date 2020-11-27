using System;
using System.Collections.Generic;
using NUnit.Framework;
using PaymentGateway;
using NFluent;
using System.Threading.Tasks;

namespace PaymentGateawayTests
{
    [TestFixture]
    public class TransactionTests
    {


        public Card c;
        public Money m;
        public TransactionID tID;
        [SetUp]
        public void Setup()
        {
            c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "23"), "123");
            m = new Money(110110.1F, "JPY");
            tID = new TransactionID(new Guid());
        }
    

        [Test]
        public void ValidCaptureTransaction_ShouldPass()
        {
            Transaction ValidTransaction = new Transaction(tID, c, m);
            Money MoneyToBeCaptured = new Money(150.55F, "JPY");
            List<String> TransactionErrors=  ValidTransaction.CaptureTransaction(MoneyToBeCaptured).Result;
            Check.That<List<String>>(TransactionErrors).IsEmpty<List<String>>();            
        }

        [Test]
        public void ValidCaptureTransaction_ShouldRetractTheCorrectAmmount()
        {
            Transaction ValidTransaction = new Transaction(tID, c, m);
            Money MoneyToBeCaptured = new Money(150.55F, "JPY");
            m = new Money(110110.1F, "JPY"); //delcaring new object as variable m from setup is tied to object Valid transaction
            List<String> TransactionErrors = ValidTransaction.CaptureTransaction(MoneyToBeCaptured).Result;
            Check.That<float>(ValidTransaction.Money.Amount).IsEqualTo<float>(m.Amount - MoneyToBeCaptured.Amount);
        }

        [Test]
        public void ValidCaptureTransaction_ShouldCaptureTheCorrectAmmount()
        {
            Transaction ValidTransaction = new Transaction(tID, c, m);
            Money MoneyToBeCaptured = new Money(150.55F, "JPY");
            List<String> TransactionErrors = ValidTransaction.CaptureTransaction(MoneyToBeCaptured).Result;
            Check.That<float>(ValidTransaction.AlreadyCapturedMoney.Amount).IsEqualTo<float>(MoneyToBeCaptured.Amount);
        }
        [Test]
        public void InvalidCaptureTransaction_ShouldFail_AsMoneyNegative()
        {
            Transaction ValidTransaction = new Transaction(tID, c, m);
            Money MoneyToBeCaptured = new Money(-150.55F, "JPY");
            m = new Money(110110.1F, "JPY"); //delcaring new object as variable m from setup is tied to object Valid transaction
            List<String> TransactionErrors = ValidTransaction.CaptureTransaction(MoneyToBeCaptured).Result;
            Check.That<float>(ValidTransaction.Money.Amount).IsEqualTo<float>(m.Amount);
            //NFluent doesn't have .IsNotEmpty.....
            Check.That<List<String>>(TransactionErrors).Not.IsEmpty<List<String>>(); 

        }

        [Test]
        public void InvalidCaptureTransaction_ShouldFail_AsMoneyGreaterThanAmountAuthorized()
        {
            Transaction ValidTransaction = new Transaction(tID, c, m);
            Money MoneyToBeCaptured = new Money(99999999999.999999F, "JPY");
            m = new Money(110110.1F, "JPY"); //delcaring new object as variable m from setup is tied to object Valid transaction
            List<String> TransactionErrors = ValidTransaction.CaptureTransaction(MoneyToBeCaptured).Result;
            Check.That<float>(ValidTransaction.Money.Amount).IsEqualTo<float>(m.Amount);
            //NFluent doesn't have .IsNotEmpty.....
            Check.That<List<String>>(TransactionErrors).Not.IsEmpty<List<String>>();

        }


        public void InvalidCaptureTransaction_ShouldFail_AsCurrencyNotSame()
        {
            Transaction ValidTransaction = new Transaction(tID, c, m);
            Money MoneyToBeCaptured = new Money(999.99F, "JPY");
            m = new Money(110110.1F, "USD"); //delcaring new object as variable m from setup is tied to object Valid transaction
            List<String> TransactionErrors = ValidTransaction.CaptureTransaction(MoneyToBeCaptured).Result;
            Check.That<float>(ValidTransaction.Money.Amount).IsEqualTo<float>(m.Amount);
            //NFluent doesn't have .IsNotEmpty.....
            Check.That<List<String>>(TransactionErrors).Not.IsEmpty<List<String>>();

        }

        [Test]
        public void ValidRefundTransaction_ShouldPass()
        {
            Transaction ValidTransaction = new Transaction(tID, c, new Money(0,"JPY"));
            ValidTransaction.SetAlreadyCapturedMoney(m);
            Money MoneyToBeRefunded = new Money(150.55F, "JPY");
            List<String> TransactionErrors = ValidTransaction.RefundTransaction(MoneyToBeRefunded).Result;
            Check.That<List<String>>(TransactionErrors).IsEmpty<List<String>>();
        }

        //TODO continue here

    }
}
