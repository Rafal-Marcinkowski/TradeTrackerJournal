using Dapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess.DBAccess;

public class SQLDataAccess(IConfiguration configuration, ILogger logger) : ISQLDataAccess
{
    public async Task<IEnumerable<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters)
    {
        using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString("TradeTrackerJournal_DBConnectionString"));
        return await dbConnection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task SaveDataAsync<T>(string storedProcedure, T parameters)
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString("TradeTrackerJournal_DBConnectionString"));
            await dbConnection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            logger.Error<Exception>($"Błąd przy zapisywaniu danych do bazy danych: ", ex);
        }
    }
}
