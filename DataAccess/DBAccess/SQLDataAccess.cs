using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace DataAccess.DBAccess;

public class SQLDataAccess : ISQLDataAccess
{
    private readonly IConfiguration configuration;
    public SQLDataAccess(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters)
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString("TradeTrackerJournal_DBConnectionString"));
            return await dbConnection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            throw;
        }
    }

    public async Task SaveDataAsync<T>(string storedProcedure, T parameters)
    {
        try
        {

            using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString("TradeTrackerJournal_DBConnectionString"));
            await dbConnection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
        catch
        {
            File.AppendAllText("C:\\Users\\rafal\\Desktop\\Pogromcy\\TradeTrackerJournal\\helper", parameters.ToString());
        }
    }
}
