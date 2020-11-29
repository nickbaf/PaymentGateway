using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Commands;
using PaymentGateway.Events;
using PaymentGateway.Models;


namespace PaymentGateway.Controllers
{
    [Route("api/authorize")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        //Dependency injection for the logger and transaction bucket
        private readonly ILogger<AuthorizeController> _logger;
        private readonly TransactionBucket _bucket;
        public AuthorizeController(ILogger<AuthorizeController> logger, ITransactionsBucket transactionsBucket) {
            _logger = logger;
            _bucket = (TransactionBucket)transactionsBucket;
        }


        // POST api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Post([FromBody] AuthorizationRequestModel request, [FromServices] IGuid transactionIDGenerator )
        {
            
            TransactionID tID = new TransactionID(transactionIDGenerator.Create());
            _logger.LogInformation(new LogConvention(tID.ID,"New TransactionID created.").ToString());


            AuthorizeCommand command = new AuthorizeCommand(tID,request.Card,request.Money); 
            var result = await command.Execute();                           
            switch (result)
            {
                case AuthorizationSuccessEvent:
                    _logger.LogInformation(new LogConvention(tID.ID, $"Authorization Resulted(AuthorizationSuccessEvent)").ToString());
                    _logger.LogInformation(new LogConvention (tID.ID,
                        System.Text.Json.JsonSerializer.Serialize<IEvent>((result as AuthorizationSuccessEvent))).ToString());
                    _bucket.CreateTransactionRecord(new Transaction(tID,command.Card,command.Money));
                    return new OkObjectResult(result as AuthorizationSuccessEvent);
                case AuthorizationFailedEvent:
                    _logger.LogInformation(new LogConvention(tID.ID, $"Authorization Resulted(AuthorizationFailedEvent)").ToString());
                    _logger.LogInformation(new LogConvention(tID.ID, (result as AuthorizationFailedEvent).Error).ToString());
                    return BadRequest(result);
                default:
                    _logger.LogInformation(new LogConvention(tID.ID, "Unauthorized.").ToString());
                    _logger.LogInformation(new LogConvention(tID.ID,
                      System.Text.Json.JsonSerializer.Serialize<IEvent>(result)).ToString());
                    return Unauthorized();                    
            }               
        }              
    }


    [Route("api/capture")]
    public class CaptureController : ControllerBase
    {
        private readonly ILogger<CaptureController> _logger;
        private readonly TransactionBucket _bucket;
        public CaptureController(ILogger<CaptureController> logger, ITransactionsBucket transactionsBucket)
        {
            _logger = logger;
            _bucket = (TransactionBucket)transactionsBucket;
        }


        // GET: api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Get([FromBody] CaptureRequestModel request)
        {
            /*
             * So what is this RetrieveTransactionRecord function does? 
             * Basically we remove the transaction from the memory and the notion
             * here is that if two captures for the same transaction with the same valid amount
             * arrive at the same time we will be modifying the same objects,worse we might capture
             * more money that we want.
             */
            if (_bucket.RetrieveTransactionRecord(request?.TransactionID, out Transaction transaction)) 
            {
                _logger.LogInformation(new LogConvention(request.TransactionID.ID, "Transaction retrieved.").ToString());
                CaptureCommand command = new CaptureCommand(request?.TransactionID, request.Money, transaction);
                var result = await command.Execute();
                switch (result)
                {
                    case CaptureSuccessEvent:
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID, $"Capture Resulted(CaptureSuccessEvent)").ToString());
                        _bucket.PutTransactionRecord(transaction); //updated transaction as pass by reference
                        return new OkObjectResult(result as CaptureSuccessEvent);
                    case CaptureFailedEvent:
                        _bucket.PutTransactionRecord(transaction);
                        return BadRequest(result);
                    default:
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID, "Unauthorized").ToString());
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID,
                        System.Text.Json.JsonSerializer.Serialize<IEvent>(result)).ToString());
                        return Unauthorized();
                }
            }
            else
            {
                _logger.LogInformation(new LogConvention(request?.TransactionID?.ID, "Bad Request").ToString());
                return BadRequest("Invalid Transaction ID");
            }
        }
    }

    [Route("api/void")]
    public class VoidController : ControllerBase
    {
        private readonly ILogger<VoidController> _logger;
        private readonly TransactionBucket _bucket;
        public VoidController(ILogger<VoidController> logger, ITransactionsBucket transactionsBucket)
        {
            _logger = logger;
            _bucket = (TransactionBucket)transactionsBucket;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Get([FromBody] VoidRequestModel request)
        {
            if (_bucket.RetrieveTransactionRecord(request?.TransactionID, out Transaction transaction)) {  
                _logger.LogInformation(new LogConvention(request.TransactionID.ID, "Transaction retrieved.").ToString());
                VoidCommand command = new VoidCommand(transaction);
                var result = await command.Execute();
                switch (result)
                {
                    case VoidSuccessEvent:
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID, $"Void Resulted(VoidSuccessEvent)").ToString());
                        return new OkObjectResult(result as VoidSuccessEvent);
                    case VoidFailedEvent:
                        return BadRequest(result);
                    default:
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID, "Unauthorized").ToString());
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID,
                      System.Text.Json.JsonSerializer.Serialize<IEvent>(result)).ToString());
                        return Unauthorized();
                }
            }
            else
            {
                _logger.LogInformation(new LogConvention(request?.TransactionID?.ID, "Bad Request").ToString());
                return BadRequest("Invalid Transaction ID");
            }
        }

    }


    [Route("api/refund")]
    public class RefundController : ControllerBase
    {
        private readonly ILogger<RefundController> _logger;
        private readonly TransactionBucket _bucket;
        public RefundController(ILogger<RefundController> logger, ITransactionsBucket transactionsBucket)
        {
            _logger = logger;
            _bucket = (TransactionBucket)transactionsBucket;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Get([FromBody] RefundRequestModel request)
        {
            if (_bucket.RetrieveTransactionRecord(request?.TransactionID, out Transaction transaction))
            {
                _logger.LogInformation(new LogConvention(request.TransactionID.ID, "Transaction retrieved.").ToString());
                RefundCommand command = new RefundCommand(request.Money, transaction);
                var result = await command.Execute();
                switch (result)
                {
                    case RefundSuccessEvent:
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID, $"Refund Resulted(RefundSuccessEvent)").ToString());
                        _bucket.PutTransactionRecord(transaction); //updated transaction as pass by reference
                        return new OkObjectResult(result as RefundSuccessEvent);
                    case RefundFailedEvent:
                        _bucket.PutTransactionRecord(transaction);
                        return BadRequest(result);
                    default:
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID, "Unauthorized").ToString());
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID,
                      System.Text.Json.JsonSerializer.Serialize<IEvent>(result)).ToString());
                        return Unauthorized();
                }
            }
            else
            {
                _logger.LogInformation(new LogConvention(request?.TransactionID?.ID, "Bad Request").ToString());
                return BadRequest("Invalid Transaction ID");
            }
        }
    }
}




