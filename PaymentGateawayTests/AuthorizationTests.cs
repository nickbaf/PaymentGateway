using System;
using NFluent;
using NUnit.Framework;
using PaymentGateway;
using PaymentGateway.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Models;
using PaymentGateway.Events;

namespace PaymentGateawayTests
{
    [TestFixture]
    public class AuthorizationTests
    {

        [Test]
        public void Init()
        {
            AuthorizeController Controller = new AuthorizeController();

            var MockBucket = new TransactionBucket();//new Mock<TransactionBucket>();
            var MockGuidGenerator = new TransactionIDGenerator();//new Mock<IGuid>();

           // MockBucket.Setup(bucket => bucket.PutTransactionRecord());
          //  MockGuidGenerator.Setup(Guid => new Guid());
            
            Card c = new Card("4000000000000000", new ExpirationMonthAndYear("11", "25"), "123");
            Money m = new Money(156, "GBP");

            var response =  Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator, MockBucket).Result;

            Check.That((response as OkObjectResult).Value).IsInstanceOf<AuthorizationSuccessEvent>();
        }

        public TransactionBucket NewBucket()
        {
            return new TransactionBucket();
        }
       
    }
}
