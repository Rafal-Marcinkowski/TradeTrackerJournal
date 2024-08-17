using DataAccess.DBAccess;

namespace DataAccess.Data;

public class CompanyData
{
    private readonly ISQLDataAccess _dBAccess;
    private readonly ICompany company;

    public CompanyData(ISQLDataAccess dBAccess, ICompany company)
    {
        _dBAccess = dBAccess;
        this.company = company;
    }

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        return await _dBAccess.LoadDataAsync<Company, dynamic>("spCompanies_GetAll", new { });
    }

    public async Task<Company> GetCompanyAsync(int id)
    {
        var companies = await _dBAccess.LoadDataAsync<Company, dynamic>("spCompanies_Get", new { ID = id });
        return companies.FirstOrDefault();
    }

    public async Task InsertCompanyAsync(string companyName, short? ranking)
    {
        await _dBAccess.SaveDataAsync("spCompanies_Insert", new { CompanyName = companyName, Ranking = ranking });
    }

    public async Task UpdateCompanyAsync(int id, string companyName, short ranking)
    {
        await _dBAccess.SaveDataAsync("spCompanies_Update", new { ID = id, CompanyName = companyName, Ranking = ranking });
    }

    public async Task DeleteCompanyAsync(int id)
    {
        await _dBAccess.SaveDataAsync("spCompanies_Delete", new { ID = id });
    }
}
