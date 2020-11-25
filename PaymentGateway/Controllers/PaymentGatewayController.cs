using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Post([FromBody] AuthorizationRequestModel request, [FromServices] IGuid guidGenerator)
        {
            Guid g = guidGenerator.Create();
            return new OkObjectResult(new AuthorizationSuccessModel(g, request.Money));
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




