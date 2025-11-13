using Dapper;
using Transferencia.Data.context;
using Transferencia.Entities;

namespace Transferencia.Data.Repository
{
    public class TransferenciaRepository
    {
        private readonly DataContext _context;

        public TransferenciaRepository(DataContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(TransferenciaEntity transferencia)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                INSERT INTO 
                    transferencia (idtransferencia, 
                                    idcontacorrente_origem, 
                                    idcontacorrente_destino, 
                                    datamovimento, 
                                    valor)
                VALUES (@IdTransferencia, 
                        @IdContaCorrenteOrigem, 
                        @IdContaCorrenteDestino, 
                        @DataMovimento, 
                        @Valor)";
            await connection.ExecuteAsync(sql, transferencia);
        }
    }
}
