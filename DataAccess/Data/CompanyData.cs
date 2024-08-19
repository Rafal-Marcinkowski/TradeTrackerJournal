using DataAccess.DBAccess;
using SharedModels.Models;

namespace DataAccess.Data;

public class CompanyData : ICompanyData
{
    private readonly ISQLDataAccess dBAccess;

    public CompanyData(ISQLDataAccess dBAccess)
    {
        this.dBAccess = dBAccess;
    }

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        return await dBAccess.LoadDataAsync<Company, dynamic>("GetAllCompanies", new { });
    }

    public async Task<Company> GetCompanyAsync(int id)
    {
        var companies = await dBAccess.LoadDataAsync<Company, dynamic>("GetCompany", new { ID = id });
        return companies.FirstOrDefault();
    }

    public async Task InsertCompanyAsync(string companyName, int transactionCount)
    {
        await dBAccess.SaveDataAsync("InsertCompany", new { CompanyName = companyName, TransactionCount = transactionCount });
    }

    public async Task UpdateCompanyAsync(int id, string companyName, int transactionCount)
    {
        await dBAccess.SaveDataAsync("UpdateCompany", new { ID = id, CompanyName = companyName, TransactionCount = transactionCount });
    }

    public async Task DeleteCompanyAsync(int id)
    {
        await dBAccess.SaveDataAsync("DeleteCompany", new { ID = id });
    }

    public async Task<int> GetCompanyID(string companyName)
    {
        var companies = await GetAllCompaniesAsync();
        return companies.FirstOrDefault(q => q.CompanyName == companyName).ID;
    }
}
