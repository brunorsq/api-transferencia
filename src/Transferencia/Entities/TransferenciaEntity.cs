namespace Transferencia.Entities
{
    public class TransferenciaEntity
    {
        public string? IdTransferencia { get; protected set; }
        public string? IdContaCorrenteOrigem { get; protected set; }
        public string? IdContaCorrenteDestino { get; protected set; }
        public string? DataMovimento { get; protected set; }
        public decimal? Valor { get; protected set; }

        public TransferenciaEntity Create(string? IdContaCorrenteOrigem, long? IdContaCorrenteDestino, decimal? valor)
        {
            return new TransferenciaEntity
            {
                IdTransferencia = Guid.NewGuid().ToString(),
                IdContaCorrenteOrigem = IdContaCorrenteOrigem,
                IdContaCorrenteDestino = IdContaCorrenteDestino.ToString(),
                DataMovimento = DateTime.Now.ToString(),
                Valor = valor
            };
        }
    }
}
