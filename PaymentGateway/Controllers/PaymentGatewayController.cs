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
               
            

           // return new OkObjectResult(new AuthorizationSuccessModel(g, request.Money));
            //return AcceptedAtRoute(nameof(AuthorizationSuccessModel),
            //           routeValues: new { gateWayPaymentId = paymentDto.GatewayPaymentId },
            //           value: paymentDto);
        }

       
       
    }


    [Route("api/capture")]
    public class CaptureController : ControllerBase
    {
        // GET: api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public IActionResult Get([FromBody] CaptureRequestModel request)
        {
            return Ok();
        }
    }

    [Route("api/void")]
    public class VoidController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public IActionResult Get([FromBody] VoidRequestModel request)
        {
            return Ok();
        }

    }


    [Route("api/refund")]
    public class RefundController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public IActionResult Gt([FromBody] RefundRequestModel request)
        {
            return Ok();
        }
    }
}




