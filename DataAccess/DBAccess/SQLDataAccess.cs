using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess.DBAccess;

public class SQLDataAccess(IConfiguration configuration) : ISQLDataAccess
{
    private readonly IConfiguration configuration = configuration;

    public async Task<IEnumerable<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters)
    {
        using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString("TradeTrackerJournal_DBConnectionString"));
        return await dbConnection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task SaveDataAsync<T>(string storedProcedure, T parameters)
    {
        using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString("TradeTrackerJournal_DBConnectionString"));
        await dbConnection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }
}
