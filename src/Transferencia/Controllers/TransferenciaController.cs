using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transferencia.Dtos.Request;
using Transferencia.Dtos.Response;
using Transferencia.Services;

namespace Transferencia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/transferencia")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class TransferenciaController : ControllerBase
    {
        private readonly TransferenciaService _transferenciaService;
        
        public TransferenciaController(TransferenciaService transferenciaService)
        {
            _transferenciaService = transferenciaService;
        }


        [HttpPost("transferir")]
        public async Task<IActionResult> TransferirAsync([FromBody] TransferirRequest? request, [FromHeader(Name = "Login-Token")] string? token)
        {
            try
            {
                var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
                
                var authToken = authorizationHeader.StartsWith("Bearer ")
                    ? authorizationHeader.Substring("Bearer ".Length).Trim()
                    : authorizationHeader;

                var retorno = await _transferenciaService.TransferirAsync(request, authToken, token);

                if (retorno.IsSuccess)
                    return NoContent();

                return BadRequest(new { tipo = retorno.ErrorType, mensagem = retorno.Message });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
