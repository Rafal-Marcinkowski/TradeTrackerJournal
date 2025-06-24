using SharedProject.Models;

namespace DataAccess.Data;
public interface ICompanyData
{
    Task DeleteCompanyAsync(int id);
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    Task<Company> GetCompanyAsync(int id);
    Task<int> GetCompanyID(string companyName);
    Task InsertCompanyAsync(Company company);
    Task InsertCompanyAsync(string companyName, int transactionCount, int eventCount, int noteCount);
    Task UpdateCompanyAsync(int id, string companyName, int transactionCount, int eventCount, int noteCount);
}