using SharedModels.Models;

namespace DataAccess.Data;

public interface ICompanyData
{
    Task DeleteCompanyAsync(int id);
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    Task<Company> GetCompanyAsync(int id);
    Task InsertCompanyAsync(string companyName, int transactionCount);
    Task UpdateCompanyAsync(int id, string companyName, int transactionCount);
}