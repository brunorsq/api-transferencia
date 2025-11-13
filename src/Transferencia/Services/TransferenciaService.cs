using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using Transferencia.Data.Repository;
using Transferencia.Dtos.Request;
using Transferencia.Dtos.Response;
using Transferencia.Entities;
using Transferencia.Settings;

namespace Transferencia.Services
{
    public class TransferenciaService
    {
        private readonly HttpClient _httpClient;
        private readonly Api _api;
        private readonly TransferenciaRepository _repository;
        public TransferenciaService(HttpClient httpClient, IOptions<Api> settings, TransferenciaRepository transferenciaRepository)
        {
            _httpClient = httpClient;
            _api = settings.Value;
            _repository = transferenciaRepository;
        }

        public async Task<Result> TransferirAsync(TransferirRequest? request, string? authToken, string? loginToken)
        {
            string? idContaOrigem = BuscarUsuarioToken(loginToken);

            var validar = ValidarParametrosAsync(request, idContaOrigem);

            if (!validar.IsSuccess)
                return validar;

            var movimentoOrigem = new AdicionarMovimentoRequest
            {
                Valor = request.Valor,
                TipoMovimento = "D"
            };

            _httpClient.DefaultRequestHeaders.Add("Login-Token", loginToken);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

            var responseOrigem = await _httpClient.PostAsJsonAsync(_api.Endpoints.AdicionarMovimento, movimentoOrigem);

            if (responseOrigem.StatusCode == HttpStatusCode.NoContent)
            {
                var movimentoDestino = new AdicionarMovimentoRequest
                {
                    NumeroConta = request.NumeroConta,
                    Valor = request.Valor,
                    TipoMovimento = "C"
                };

                var responseDestino = await _httpClient.PostAsJsonAsync(_api.Endpoints.AdicionarMovimento, movimentoDestino);

                if (responseDestino.StatusCode == HttpStatusCode.NoContent)
                {
                    await _repository.CreateAsync(new TransferenciaEntity().Create(idContaOrigem, request.NumeroConta, request.Valor));

                    return Result.Success();
                }

                var bodyResponseDestino = await responseDestino.Content.ReadAsStringAsync();

                var movimentoDesfaz = new AdicionarMovimentoRequest
                {
                    Valor = request.Valor,
                    TipoMovimento = "C"
                };

                var responseDesfaz = await _httpClient.PostAsJsonAsync(_api.Endpoints.AdicionarMovimento, movimentoDesfaz);
                
                return ValidarResponse(bodyResponseDestino);
            }

            var bodyResponseOrigem = await responseOrigem.Content.ReadAsStringAsync();
            return ValidarResponse(bodyResponseOrigem);
        }

        private static Result ValidarResponse(string bodyResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(bodyResponse);
                var tipo = doc.RootElement.GetProperty("tipo").GetString();
                var mensagem = doc.RootElement.GetProperty("mensagem").GetString();

                return Result.Failure(tipo, mensagem);
            }
            catch
            {
                return Result.Failure("UNKNOWN-ERROR", bodyResponse);
            }
        }

        public Result ValidarParametrosAsync(TransferirRequest? request, string? loginToken)
        {
            if (request == null)
                return Result.Failure("INVALID_PARAM", "Parâmetros não informados");

            if (loginToken.IsNullOrEmpty())
                return Result.Failure("INVALID_PARAM", "Usuário não encontrado");

            if (request.NumeroConta == null || request.NumeroConta == 0)
                return Result.Failure("INVALID_PARAM", "Número da conta não informada");


            if (request.Valor == null || request.Valor == 0)
                return Result.Failure("INVALID_PARAM", "Valor não informado");

            return Result.Success();
        }

        public string? BuscarUsuarioToken(string? token)
        {
            if (token.IsNullOrEmpty())
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(token);
            var usuario = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (expClaim != null && long.TryParse(expClaim, out var expSeconds))
            {
                var expDate = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;

                if (expDate < DateTime.UtcNow)
                    return null;
            }

            return usuario;
        }
    }
}
