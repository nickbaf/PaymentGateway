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
    public class AuthorizationControllerShould
    {

        [Test]
        public void SuccesfullyAuthorizeValidRequest()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            var transactionBucketMock = new Mock<TransactionBucket>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object, transactionBucketMock.Object);


            var MockGuidGenerator = new Mock<IGuid>();
            
            Card c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "25"), "123");
            Money m = new Money(156, "GBP");

            var response =  Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator.Object).Result;
            Check.That(response).IsInstanceOf<OkObjectResult>();
            Check.That((response as OkObjectResult).Value).IsInstanceOf<AuthorizationSuccessEvent>();
            AuthorizationSuccessEvent result = (AuthorizationSuccessEvent)(response as OkObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();            
        }

        [Test]
        public void RefuseToAuthorize_ExpiredCreditCard()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            var transactionBucketMock = new Mock<TransactionBucket>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object, transactionBucketMock.Object);
            var MockGuidGenerator =new Mock<IGuid>();


            Card c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "12"), "123");
            Money m = new Money(156F, "GBP");

            var response = Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator.Object).Result;
            Check.That(response).IsInstanceOf<BadRequestObjectResult>();
            Check.That((response as BadRequestObjectResult).Value).IsInstanceOf<AuthorizationFailedEvent>();
            AuthorizationFailedEvent result = (AuthorizationFailedEvent)(response as BadRequestObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);            
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();
        }

        [Test]
        public void RefuseToAuthorize_NegativeAmount()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            var transactionBucketMock = new Mock<TransactionBucket>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object, transactionBucketMock.Object);

           
            var MockGuidGenerator = new Mock<IGuid>();

            Card c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "25"), "123");
            Money m = new Money(-156F, "GBP");

            var response = Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator.Object).Result;
            Check.That(response).IsInstanceOf<BadRequestObjectResult>();
            Check.That((response as BadRequestObjectResult).Value).IsInstanceOf<AuthorizationFailedEvent>();
            AuthorizationFailedEvent result = (AuthorizationFailedEvent)(response as BadRequestObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();
        }

        [Test]
        public void RefuseToAuthorize_InvalidCreditCard()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            var transactionBucketMock = new Mock<TransactionBucket>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object, transactionBucketMock.Object);

            var MockGuidGenerator = new Mock<IGuid>();


            Card c = new Card("0000 I AM AN INVALID CREDIT CARD &&**((", new ExpirationMonthAndYear("11", "25"), "123");
            Money m = new Money(156F, "GBP");

            var response = Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator.Object).Result;
            Check.That(response).IsInstanceOf<BadRequestObjectResult>();
            Check.That((response as BadRequestObjectResult).Value).IsInstanceOf<AuthorizationFailedEvent>();
            AuthorizationFailedEvent result = (AuthorizationFailedEvent)(response as BadRequestObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();
        }

        [Test]
        public void RefuseToAuthorize_InvalidCreditCard_WithInvalidExpirationDate()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            var transactionBucketMock = new Mock<TransactionBucket>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object, transactionBucketMock.Object);

            var MockGuidGenerator = new Mock<IGuid>();


            Card c = new Card("0000", new ExpirationMonthAndYear("November",null), "123");
            Money m = new Money(156F, "GBP");

            var response = Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator.Object).Result;
            Check.That(response).IsInstanceOf<BadRequestObjectResult>();
            Check.That((response as BadRequestObjectResult).Value).IsInstanceOf<AuthorizationFailedEvent>();
            AuthorizationFailedEvent result = (AuthorizationFailedEvent)(response as BadRequestObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();
        }

        [Test]
        public void RefuseToAuthorize_NullRequest()
        {
            var loggerMock = new Mock<ILogger<AuthorizeController>>();
            var transactionBucketMock = new Mock<TransactionBucket>();
            AuthorizeController Controller = new AuthorizeController(loggerMock.Object, transactionBucketMock.Object);

            var MockGuidGenerator = new Mock<IGuid>();


            Card c = new Card(null, null,null);
            Money m = null;

            var response = Controller.Post(
                new PaymentGateway.Models.AuthorizationRequestModel(c, m), MockGuidGenerator.Object).Result;
            Check.That(response).IsInstanceOf<BadRequestObjectResult>();
            Check.That((response as BadRequestObjectResult).Value).IsInstanceOf<AuthorizationFailedEvent>();
            AuthorizationFailedEvent result = (AuthorizationFailedEvent)(response as BadRequestObjectResult).Value;
            Check.That<String>(result.CardNumber).IsEqualTo<String>(c.Number);
            Check.That<Money>(result.AmountAndCurrencyAvailable).IsEqualTo<Money>(m);
            Check.That<TransactionID>(result.TransactionID).IsNotNull<TransactionID>();
        }
    }
}
