using SharedProject.Models;

namespace Infrastructure.Interfaces;
public interface ICompanyManager
{
    Task DecrementNoteCount(string companyName);
    Task<IEnumerable<Company>> GetAllCompanies();
    Task<int> GetCompanyId(string companyName);
    Task IncrementNoteCount(string companyName);
    Task UpdateCompany(Company company);
}