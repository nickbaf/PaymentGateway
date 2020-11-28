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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentGateway.Controllers
{
    [Route("api/authorize")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {

        private readonly ILogger<AuthorizeController> _logger;
        public AuthorizeController(ILogger<AuthorizeController> logger) {
            _logger = logger;
        }

        // POST api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Post([FromBody] AuthorizationRequestModel request, [FromServices] IGuid transactionIDGenerator, [FromServices] ITransactionsBucket transactionsBucket)
        {
            
            TransactionID tID = new TransactionID(transactionIDGenerator.Create());
            _logger.LogInformation(new LogConvention(tID.ID,"New TransactionID created.").ToString());

            // transactionsBucket.CreateTransactionRecord(transaction);
            AuthorizeCommand command = new AuthorizeCommand(tID,request.Card,request.Money);
            var result = await command.Execute();         
            switch (result)
            {
                case AuthorizationSuccessEvent:
                    _logger.LogInformation(new LogConvention(tID.ID, $"Authorization Resulted(AuthorizationSuccessEvent)").ToString());
                    _logger.LogInformation(new LogConvention (tID.ID,
                        System.Text.Json.JsonSerializer.Serialize<IEvent>((result as AuthorizationSuccessEvent))).ToString());
                    transactionsBucket.CreateTransactionRecord(new Transaction(tID,command.Card,command.Money));
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
        public CaptureController(ILogger<CaptureController> logger)
        {
            _logger = logger;
        }
        // GET: api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Get([FromBody] CaptureRequestModel request, [FromServices] ITransactionsBucket transactionsBucket)
        {
            if (transactionsBucket.RetrieveTransactionRecord(request?.TransactionID, out Transaction transaction))
            {
                _logger.LogInformation(new LogConvention(request.TransactionID.ID, "Transaction retrieved.").ToString());
                CaptureCommand command = new CaptureCommand(request?.TransactionID, request.Money, transaction);
                var result = await command.Execute();
                switch (result)
                {
                    case CaptureSuccessEvent:
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID, $"Capture Resulted(CaptureSuccessEvent)").ToString());
                        transactionsBucket.PutTransactionRecord(transaction); //updated transaction as pass by reference
                        return new OkObjectResult(result as CaptureSuccessEvent);
                    case CaptureFailedEvent:
                        transactionsBucket.PutTransactionRecord(transaction);
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
        public VoidController(ILogger<VoidController> logger)
        {
            _logger = logger;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Get([FromBody] VoidRequestModel request, [FromServices] ITransactionsBucket transactionsBucket)
        {
            if (transactionsBucket.RetrieveTransactionRecord(request?.TransactionID, out Transaction transaction))
            {
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
        public RefundController(ILogger<RefundController> logger)
        {
            _logger = logger;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Get([FromBody] RefundRequestModel request, [FromServices] ITransactionsBucket transactionsBucket)
        {
            if (transactionsBucket.RetrieveTransactionRecord(request?.TransactionID, out Transaction transaction))
            {
                _logger.LogInformation(new LogConvention(request.TransactionID.ID, "Transaction retrieved.").ToString());
                RefundCommand command = new RefundCommand(request.Money, transaction);
                var result = await command.Execute();
                switch (result)
                {
                    case RefundSuccessEvent:
                        _logger.LogInformation(new LogConvention(request.TransactionID.ID, $"Refund Resulted(RefundSuccessEvent)").ToString());
                        transactionsBucket.PutTransactionRecord(transaction); //updated transaction as pass by reference
                        return new OkObjectResult(result as RefundSuccessEvent);
                    case RefundFailedEvent:
                        transactionsBucket.PutTransactionRecord(transaction);
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




