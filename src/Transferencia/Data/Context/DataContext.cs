using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Transferencia.Data.context
{
    public class DataContext
    {
        private readonly string _connectionString;

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
            EnsureDatabaseCreated();
        }

        public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);

        private void EnsureDatabaseCreated()
        {
            var dbFile = new SqliteConnectionStringBuilder(_connectionString).DataSource;
            var folder = Path.GetDirectoryName(dbFile);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS transferencia (
                    idtransferencia TEXT(37) PRIMARY KEY, -- identificacao unica da transferencia
                    idcontacorrente_origem TEXT(37) NOT NULL, -- identificacao unica da conta corrente de origem
                    idcontacorrente_destino TEXT(37) NOT NULL, -- identificacao unica da conta corrente de destino
                    datamovimento TEXT(25) NOT NULL, -- data do transferencia no formato DD/MM/YYYY
                    valor REAL NOT NULL, -- valor da transferencia. Usar duas casas decimais.
                    FOREIGN KEY(idtransferencia) REFERENCES transferencia(idtransferencia)
                );";
            connection.Execute(createTableSql);
        }
    }
}
