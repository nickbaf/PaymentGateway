using System;
using NFluent;
using NUnit.Framework;
using PaymentGateway;
using PaymentGateway.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Models;
using PaymentGateway.Events;
using NLog;
using Microsoft.Extensions.Logging;

namespace PaymentGateawayTests
{
    [TestFixture]
    public class AuthorizationTests
    {

        [Test]
        public void ValidAuthorization()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object);

            var MockBucket = new TransactionBucket();//new Mock<TransactionBucket>();
            var MockGuidGenerator = new TransactionIDGenerator();//new Mock<IGuid>();

           // MockBucket.Setup(bucket => bucket.PutTransactionRecord());
          //  MockGuidGenerator.Setup(Guid => new Guid());
            
            Card c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "25"), "123");
            Money m = new Money(156, "GBP");

            var response =  Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator, MockBucket).Result;
            Check.That(response).IsInstanceOf<OkObjectResult>();
            Check.That((response as OkObjectResult).Value).IsInstanceOf<AuthorizationSuccessEvent>();
            AuthorizationSuccessEvent result = (AuthorizationSuccessEvent)(response as OkObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();            
        }

        [Test]
        public void InValidAuthorization_Expired_CreditCard()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object);


            var MockBucket = new TransactionBucket();//new Mock<TransactionBucket>();
            var MockGuidGenerator = new TransactionIDGenerator();//new Mock<IGuid>();

            // MockBucket.Setup(bucket => bucket.PutTransactionRecord());
            //  MockGuidGenerator.Setup(Guid => new Guid());

            Card c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "12"), "123");
            Money m = new Money(156F, "GBP");

            var response = Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator, MockBucket).Result;
            Check.That(response).IsInstanceOf<BadRequestObjectResult>();
            Check.That((response as BadRequestObjectResult).Value).IsInstanceOf<AuthorizationFailedEvent>();
            AuthorizationFailedEvent result = (AuthorizationFailedEvent)(response as BadRequestObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);            
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();
        }


        public void InValidAuthorization_Negative_Amount()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object);

            var MockBucket = new TransactionBucket();//new Mock<TransactionBucket>();
            var MockGuidGenerator = new TransactionIDGenerator();//new Mock<IGuid>();

            // MockBucket.Setup(bucket => bucket.PutTransactionRecord());
            //  MockGuidGenerator.Setup(Guid => new Guid());

            Card c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "25"), "123");
            Money m = new Money(-156F, "GBP");

            var response = Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator, MockBucket).Result;
            Check.That(response).IsInstanceOf<BadRequestObjectResult>();
            Check.That((response as BadRequestObjectResult).Value).IsInstanceOf<AuthorizationFailedEvent>();
            AuthorizationFailedEvent result = (AuthorizationFailedEvent)(response as BadRequestObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();
        }


        public void InValidAuthorization_Invalid_CreditCard()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object);

            var MockBucket = new TransactionBucket();//new Mock<TransactionBucket>();
            var MockGuidGenerator = new TransactionIDGenerator();//new Mock<IGuid>();

            // MockBucket.Setup(bucket => bucket.PutTransactionRecord());
            //  MockGuidGenerator.Setup(Guid => new Guid());

            Card c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "25"), "123");
            Money m = new Money(156F, "GBP");

            var response = Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator, MockBucket).Result;
            Check.That(response).IsInstanceOf<BadRequestObjectResult>();
            Check.That((response as BadRequestObjectResult).Value).IsInstanceOf<AuthorizationFailedEvent>();
            AuthorizationFailedEvent result = (AuthorizationFailedEvent)(response as BadRequestObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();
        }

        public TransactionBucket NewBucket()
        {
            return new TransactionBucket();
        }
       
    }
}
