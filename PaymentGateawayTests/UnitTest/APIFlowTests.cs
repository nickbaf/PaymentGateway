﻿using System;
using NUnit.Framework;
using NFluent;
using PaymentGateway.Controllers;
using PaymentGateway;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Events;
using PaymentGateway.Models;
using System.Threading.Tasks;
using NLog;
using Microsoft.Extensions.Logging;
using Moq;

namespace PaymentGateawayTests
{
    [TestFixture]
    public class APIShould
    {
        AuthorizeController AuthController;
        CaptureController CaptureController;
        RefundController RefundController;
        VoidController VoidController;
        TransactionBucket TransactionBucket;
        TransactionIDGenerator TransactionIDGenerator;
        Card Card;
        Money Money;
        [SetUp]
        public void Setup()
        {
            var mockAuthLogger = new Mock<ILogger<AuthorizeController>>();
            var mockCaptLogger = new Mock<ILogger<CaptureController>>();
            var mockVoidLogger = new Mock<ILogger<VoidController>>();
            var mockRefundLogger = new Mock<ILogger<RefundController>>();
            var mockTransactionBucket = new Mock<TransactionBucket>();
            AuthController = new AuthorizeController(mockAuthLogger.Object,mockTransactionBucket.Object);
            CaptureController = new CaptureController(mockCaptLogger.Object, mockTransactionBucket.Object);
            RefundController = new RefundController(mockRefundLogger.Object, mockTransactionBucket.Object);
            VoidController = new VoidController(mockVoidLogger.Object, mockTransactionBucket.Object);
            TransactionBucket = new TransactionBucket();
            TransactionIDGenerator = new TransactionIDGenerator();
            Card = new Card("5186124094923094", new ExpirationMonthAndYear("11", "25"), "123");
            Money = new Money(156.33F, "JPY");
        }


        [Test]
        public void SucessfullyAuthorize_AndCaptureOnce()
        {

            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(Card, Money), TransactionIDGenerator).Result;
            //Too many TypeCasts?Ahhhh ikr, had to cut some corners as time is finite.... :(
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            Money MoneyToCapture = new Money(156.33F, "JPY");
            var captureResponse = CaptureController.Get(new CaptureRequestModel(tID, MoneyToCapture)).Result;

            Check.That(captureResponse).IsInstanceOf<OkObjectResult>();
            Check.That((captureResponse as OkObjectResult).Value).IsInstanceOf<CaptureSuccessEvent>();
            CaptureSuccessEvent result = (CaptureSuccessEvent)(captureResponse as OkObjectResult).Value;

            Check.That<float>(result.AmountAndCurrencyAvailable.Amount).IsEqualTo<float>(0F);
            Check.That<string>(result.CardNumber).Equals(Card.Number);
        }

        [Test]
        public async Task SucessfullyAuthorizate_AndCaptureMany()
        {
            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(Card, Money), TransactionIDGenerator).Result;
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;

            for (double f = 0; f < 156.31f; f += 0.0100F)
            {
               await CaptureController.Get(new CaptureRequestModel(tID, new Money(0.01f, "JPY")));
            }
            var captureResponse = CaptureController.Get(new CaptureRequestModel(tID, new Money(0.01f, "JPY"))).Result;

            Check.That(captureResponse).IsInstanceOf<OkObjectResult>();
            Check.That((captureResponse as OkObjectResult).Value).IsInstanceOf<CaptureSuccessEvent>();
            CaptureSuccessEvent result = (CaptureSuccessEvent)(captureResponse as OkObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(0F,0.01F);
            Check.That<string>(result.CardNumber).Equals(Card.Number);
        }


        [Test]
        public async Task SucessfullyAuthorize_CaptureOnce_AndRefundOnce()
        {
            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(Card, Money), TransactionIDGenerator).Result;
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            Money moneyToCaptureAndRefund = new Money(56.33F, "JPY");
            await CaptureController.Get(new CaptureRequestModel(tID, moneyToCaptureAndRefund));
            var refundResponse = RefundController.Get(new RefundRequestModel(tID, moneyToCaptureAndRefund)).Result;
            Check.That(refundResponse).IsInstanceOf<OkObjectResult>();
            Check.That((refundResponse as OkObjectResult).Value).IsInstanceOf<RefundSuccessEvent>();
            RefundSuccessEvent result = (RefundSuccessEvent)(refundResponse as OkObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(56.33F, 0.01F);
            Check.That<string>(result.CardNumber).Equals(Card.Number);


        }

        [Test]
        public async Task SucessfullyAuthorize_CaptureOnce_AndRefundMany()
        {
            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(Card, Money), TransactionIDGenerator).Result;
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            Money moneyToCaptureAndRefund = new Money(156.33F, "JPY");
            await CaptureController.Get(new CaptureRequestModel(tID, moneyToCaptureAndRefund));
            for (double f = 0; f < 156.31f; f += 0.0100F)
            {
                await RefundController.Get(new RefundRequestModel(tID, new Money(0.01f,"JPY")));

            }
            var refundResponse = RefundController.Get(new RefundRequestModel(tID, new Money(0.01f, "JPY"))).Result;
            Check.That(refundResponse).IsInstanceOf<OkObjectResult>();
            Check.That((refundResponse as OkObjectResult).Value).IsInstanceOf<RefundSuccessEvent>();
            RefundSuccessEvent result = (RefundSuccessEvent)(refundResponse as OkObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(0F, 0.01F);
            Check.That<string>(result.CardNumber).Equals(Card.Number);

        }

        [Test]
        public void SucessfullyAuthorize_AndVoid()
        {
            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(Card, Money), TransactionIDGenerator).Result;
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            var voidResponse = VoidController.Get(new VoidRequestModel(tID)).Result;
            Check.That(voidResponse).IsInstanceOf<OkObjectResult>();
            Check.That((voidResponse as OkObjectResult).Value).IsInstanceOf<VoidSuccessEvent>();
            VoidSuccessEvent result = (VoidSuccessEvent)(voidResponse as OkObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(156.33F, 0.01F);
            Check.That<string>(result.CardNumber).Equals(Card.Number);
        }


        //**********************************************************************

        [Test]
        public async Task SucessfullyAuthorize_CaptureOnce_RefundOnce_RefuseToCapture()
        {
            var response = AuthController.Post(
           new PaymentGateway.Models.AuthorizationRequestModel(Card, Money), TransactionIDGenerator).Result;
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            await CaptureController.Get(new CaptureRequestModel(tID, new Money(16.33F, "JPY")));
            await RefundController.Get(new RefundRequestModel(tID, new Money(6.33F, "JPY")));
            var failedCapture = CaptureController.Get(new CaptureRequestModel(tID, new Money(26.33F, "JPY"))).Result;
            Check.That(failedCapture).IsInstanceOf<BadRequestObjectResult>();
            Check.That((failedCapture as BadRequestObjectResult).Value).IsInstanceOf<CaptureFailedEvent>();
            CaptureFailedEvent result = (CaptureFailedEvent)(failedCapture as BadRequestObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(146.33F, 0.01F);
            Check.That<string>(result.CardNumber).Equals(Card.Number);


        }

    }
}
