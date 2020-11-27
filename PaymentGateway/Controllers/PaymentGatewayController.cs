using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        public AuthorizeController() { }

        // POST api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Post([FromBody] AuthorizationRequestModel request, [FromServices] IGuid transactionIDGenerator, [FromServices] ITransactionsBucket transactionsBucket)
        {
            TransactionID tID = new TransactionID(transactionIDGenerator.Create());
            // transactionsBucket.CreateTransactionRecord(transaction);
            AuthorizeCommand command = new AuthorizeCommand(tID,request.Card,request.Money);
            var result = await command.Execute();         
            switch (result)
            {
                case AuthorizationSuccessEvent:
                   // AuthorizationSuccessEvent evt = (result as AuthorizationSuccessEvent);
                    transactionsBucket.CreateTransactionRecord(new Transaction(tID,command.Card,command.Money));
                    return new OkObjectResult(result as AuthorizationSuccessEvent);
                case AuthorizationFailedEvent:
                    return BadRequest(result);
                default:
                    return Unauthorized();
                    
            }
               

        }

       
       
    }


    [Route("api/capture")]
    public class CaptureController : ControllerBase
    {
        public CaptureController() { }
        // GET: api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Get([FromBody] CaptureRequestModel request, [FromServices] ITransactionsBucket transactionsBucket)
        {
            if (transactionsBucket.RetrieveTransactionRecord(request.TransactionID, out Transaction transaction))
            {
                CaptureCommand command = new CaptureCommand(request.TransactionID, request.Money, transaction);
                var result = await command.Execute();
                switch (result)
                {
                    case CaptureSuccessEvent:
                        // AuthorizationSuccessEvent evt = (result as AuthorizationSuccessEvent);
                        transactionsBucket.PutTransactionRecord(transaction); //updated transaction as pass by reference
                        return new OkObjectResult(result as CaptureSuccessEvent);
                    case CaptureFailedEvent:
                        transactionsBucket.PutTransactionRecord(transaction);
                        return BadRequest(result);
                    default:
                        return Unauthorized();
                }
            }
            else
            {
                return BadRequest("Invalid Transaction ID");
            }
        }
    }

    [Route("api/void")]
    public class VoidController : ControllerBase
    {
        public VoidController(){ }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Get([FromBody] VoidRequestModel request, [FromServices] ITransactionsBucket transactionsBucket)
        {
            if (transactionsBucket.RetrieveTransactionRecord(request.TransactionID, out Transaction transaction))
            {
                VoidCommand command = new VoidCommand(transaction);
                var result = await command.Execute();
                switch (result)
                {
                    case VoidSuccessEvent:
                        // AuthorizationSuccessEvent evt = (result as AuthorizationSuccessEvent);
                        return new OkObjectResult(result as VoidSuccessEvent);
                    case VoidFailedEvent:
                        return BadRequest(result);
                    default:
                        return Unauthorized();
                }
            }
            else
            {
                return BadRequest("Invalid Transaction ID");
            }
        }

    }


    [Route("api/refund")]
    public class RefundController : ControllerBase
    {
        public RefundController() { }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> Get([FromBody] RefundRequestModel request, [FromServices] ITransactionsBucket transactionsBucket)
        {
            if (transactionsBucket.RetrieveTransactionRecord(request.TransactionID, out Transaction transaction))
            {
                RefundCommand command = new RefundCommand(request.Money, transaction);
                var result = await command.Execute();
                switch (result)
                {
                    case RefundSuccessEvent:
                        // AuthorizationSuccessEvent evt = (result as AuthorizationSuccessEvent);
                        transactionsBucket.PutTransactionRecord(transaction); //updated transaction as pass by reference
                        return new OkObjectResult(result as CaptureSuccessEvent);
                    case CaptureFailedEvent:
                        transactionsBucket.PutTransactionRecord(transaction);
                        return BadRequest(result);
                    default:
                        return Unauthorized();
                }
            }
            else
            {
                return BadRequest("Invalid Transaction ID");
            }
        }
    }
}




