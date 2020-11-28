using System;
using NUnit.Framework;
using NFluent;
using PaymentGateway.Controllers;
using PaymentGateway;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Events;
using PaymentGateway.Models;
using System.Threading.Tasks;

namespace PaymentGateawayTests
{
    [TestFixture]
    public class APIFlowTests
    {
        AuthorizeController AuthController;
        CaptureController CaptureController;
        RefundController RefundController;
        VoidController VoidController;
        TransactionBucket TransactionBucket;
        TransactionIDGenerator TransactionIDGenerator;
        Card c;
        Money m;
        [SetUp]
        public void Setup()
        {
             AuthController = new AuthorizeController();
             CaptureController = new CaptureController();
             RefundController = new RefundController();
             VoidController = new VoidController();
             TransactionBucket = new TransactionBucket();
             TransactionIDGenerator = new TransactionIDGenerator();
             c = new Card("5186124094923094", new ExpirationMonthAndYear("11", "25"), "123");
             m = new Money(156.33F, "JPY");
        }


        [Test]
        public void SucessfullAuthorization_SingleCaptureAsync()
        {

            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(c, m), TransactionIDGenerator, TransactionBucket).Result;
            //Too many TypeCasts?Ahhhh ikr, had to cut some corners as time is finite.... :(
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            Money MoneyToCapture = new Money(156.33F, "JPY");
            var captureResponse = CaptureController.Get(new CaptureRequestModel(tID, MoneyToCapture), TransactionBucket).Result;

            Check.That(captureResponse).IsInstanceOf<OkObjectResult>();
            Check.That((captureResponse as OkObjectResult).Value).IsInstanceOf<CaptureSuccessEvent>();
            CaptureSuccessEvent result = (CaptureSuccessEvent)(captureResponse as OkObjectResult).Value;

            Check.That<float>(result.AmountAndCurrencyAvailable.Amount).IsEqualTo<float>(0F);
            Check.That<string>(result.CardNumber).Equals(c.Number);
        }

        [Test]
        public async Task SucessfullAuthorization_MultipleCapture()
        {
            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(c, m), TransactionIDGenerator, TransactionBucket).Result;
            //Too many TypeCasts?Ahhhh ikr, had to cut some corners as time is finite.... :(
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;

            for (double f = 0; f < 156.31f; f += 0.0100F)
            {
               await CaptureController.Get(new CaptureRequestModel(tID, new Money(0.01f, "JPY")), TransactionBucket);
            }
            var captureResponse = CaptureController.Get(new CaptureRequestModel(tID, new Money(0.01f, "JPY")), TransactionBucket).Result;

            Check.That(captureResponse).IsInstanceOf<OkObjectResult>();
            Check.That((captureResponse as OkObjectResult).Value).IsInstanceOf<CaptureSuccessEvent>();
            CaptureSuccessEvent result = (CaptureSuccessEvent)(captureResponse as OkObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(0F,0.01F);
            Check.That<string>(result.CardNumber).Equals(c.Number);
        }


        [Test]
        public async Task SucessfullAuthorization_SingleCapture_SingleRefund()
        {
            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(c, m), TransactionIDGenerator, TransactionBucket).Result;
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            Money moneyToCaptureAndRefund = new Money(56.33F, "JPY");
            await CaptureController.Get(new CaptureRequestModel(tID, moneyToCaptureAndRefund), TransactionBucket);
            var refundResponse = RefundController.Get(new RefundRequestModel(tID, moneyToCaptureAndRefund), TransactionBucket).Result;
            Check.That(refundResponse).IsInstanceOf<OkObjectResult>();
            Check.That((refundResponse as OkObjectResult).Value).IsInstanceOf<RefundSuccessEvent>();
            RefundSuccessEvent result = (RefundSuccessEvent)(refundResponse as OkObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(56.33F, 0.01F);
            Check.That<string>(result.CardNumber).Equals(c.Number);


        }

        [Test]
        public async Task SucessfullAuthorization_SingleCapture_MultipleRefund()
        {
            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(c, m), TransactionIDGenerator, TransactionBucket).Result;
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            Money moneyToCaptureAndRefund = new Money(156.33F, "JPY");
            await CaptureController.Get(new CaptureRequestModel(tID, moneyToCaptureAndRefund), TransactionBucket);
            for (double f = 0; f < 156.31f; f += 0.0100F)
            {
                await RefundController.Get(new RefundRequestModel(tID, new Money(0.01f,"JPY")), TransactionBucket);

            }
            var refundResponse = RefundController.Get(new RefundRequestModel(tID, new Money(0.01f, "JPY")), TransactionBucket).Result;
            Check.That(refundResponse).IsInstanceOf<OkObjectResult>();
            Check.That((refundResponse as OkObjectResult).Value).IsInstanceOf<RefundSuccessEvent>();
            RefundSuccessEvent result = (RefundSuccessEvent)(refundResponse as OkObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(0F, 0.01F);
            Check.That<string>(result.CardNumber).Equals(c.Number);

        }

        [Test]
        public void SucessfullAuthorization_Void()
        {
            var response = AuthController.Post(
            new PaymentGateway.Models.AuthorizationRequestModel(c, m), TransactionIDGenerator, TransactionBucket).Result;
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            var voidResponse = VoidController.Get(new VoidRequestModel(tID), TransactionBucket).Result;
            Check.That(voidResponse).IsInstanceOf<OkObjectResult>();
            Check.That((voidResponse as OkObjectResult).Value).IsInstanceOf<VoidSuccessEvent>();
            VoidSuccessEvent result = (VoidSuccessEvent)(voidResponse as OkObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(156.33F, 0.01F);
            Check.That<string>(result.CardNumber).Equals(c.Number);
        }


        //**********************************************************************

        [Test]
        public async Task SucessfullAuthorization_SingleCapture_SingleRefund_FailedCapture()
        {
            var response = AuthController.Post(
           new PaymentGateway.Models.AuthorizationRequestModel(c, m), TransactionIDGenerator, TransactionBucket).Result;
            TransactionID tID = ((response as OkObjectResult).Value as AuthorizationSuccessEvent).TransactionID;
            await CaptureController.Get(new CaptureRequestModel(tID, new Money(16.33F, "JPY")), TransactionBucket);
            await RefundController.Get(new RefundRequestModel(tID, new Money(6.33F, "JPY")), TransactionBucket);
            var failedCapture = CaptureController.Get(new CaptureRequestModel(tID, new Money(26.33F, "JPY")), TransactionBucket).Result;
            Check.That(failedCapture).IsInstanceOf<BadRequestObjectResult>();
            Check.That((failedCapture as BadRequestObjectResult).Value).IsInstanceOf<CaptureFailedEvent>();
            CaptureFailedEvent result = (CaptureFailedEvent)(failedCapture as BadRequestObjectResult).Value;

            Check.That<double>(result.AmountAndCurrencyAvailable.Amount).IsCloseTo(146.33F, 0.01F);
            Check.That<string>(result.CardNumber).Equals(c.Number);


        }

    }
}
