using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Service.Interface;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Notification.Api.Controllers
{
    /// <summary>
    /// Controler to send messages
    /// </summary>    
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiController]
    [Authorize("Bearer")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly TokenValidationParameters _tokenValidationParameters;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageService"></param>
        /// <param name="tokenValidationParameters"></param>
        public MessagesController(IMessageService messageService,
            TokenValidationParameters tokenValidationParameters)
        {
            _messageService = messageService;
            _tokenValidationParameters = tokenValidationParameters;
        }

        /// <summary>
        /// Received Messages (Other Ms)
        /// </summary>
        /// <returns></returns>
        [HttpGet("received", Name = "GetReceivedMessages")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<object> GetReceivedMessages()
        {
            try
            {
                var result = await _messageService.GetReceivedMessages();
                return this.StatusCode(StatusCodes.Status200OK, new
                {
                    count = result.Count,
                    messages = result
                });
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, "Ocorreu um erro ao buscar as mensagens recebidas");
            }
        }
 
        /// <summary>
        /// Sent Messages (By this Ms)
        /// </summary>
        /// <returns></returns>
        [HttpGet("sent", Name = "GetSentMessages")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<object> GetSentMessages()
        {
            try
            {
                var result = await _messageService.GetSentMessages();
                return this.StatusCode(StatusCodes.Status200OK, new {
                    count = result.Count, messages = result });
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, "Ocorreu um erro ao buscar as mensagens enviadas");
            }
        }
    }
}