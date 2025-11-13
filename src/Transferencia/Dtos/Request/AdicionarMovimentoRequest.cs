namespace Transferencia.Dtos.Request
{
    public class AdicionarMovimentoRequest
    {
        public long? NumeroConta { get; set; }
        public decimal? Valor { get; set; }
        public string? TipoMovimento { get; set; }
    }
}
