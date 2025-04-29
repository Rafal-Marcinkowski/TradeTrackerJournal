using SharedProject.Models;

namespace Infrastructure.Interfaces;
public interface ICompanyManager
{
    Task<IEnumerable<Company>> GetAllCompanies();
    Task<int> GetCompanyId(string companyName);
    Task UpdateCompany(Company company);
}