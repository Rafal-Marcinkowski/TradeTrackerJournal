﻿using DataAccess.DBAccess;
using SharedProject.Models;

namespace DataAccess.Data;

public class CompanyData(ISQLDataAccess dBAccess) : ICompanyData
{
    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        return await dBAccess.LoadDataAsync<Company, dynamic>("GetAllCompanies", new { });
    }

    public async Task<Company> GetCompanyAsync(int id)
    {
        var companies = await dBAccess.LoadDataAsync<Company, dynamic>("GetCompany", new { ID = id });
        return companies.FirstOrDefault();
    }

    public async Task InsertCompanyAsync(Company company)
    {
        await InsertCompanyAsync(company.CompanyName, company.TransactionCount, company.EventCount, company.NoteCount);
    }

    public async Task InsertCompanyAsync(string companyName, int transactionCount, int eventCount, int noteCount)
    {
        await dBAccess.SaveDataAsync("InsertCompany", new { CompanyName = companyName, TransactionCount = transactionCount, EventCount = eventCount, NoteCount = noteCount });
    }

    public async Task UpdateCompanyAsync(int id, string companyName, int transactionCount, int eventCount, int noteCount)
    {
        await dBAccess.SaveDataAsync("UpdateCompany", new { ID = id, CompanyName = companyName, TransactionCount = transactionCount, EventCount = eventCount, NoteCount = noteCount });
    }

    public async Task DeleteCompanyAsync(int id)
    {
        await dBAccess.SaveDataAsync("DeleteCompany", new { ID = id });
    }

    public async Task<int> GetCompanyID(string companyName)
    {
        var companies = await GetAllCompaniesAsync();
        return companies.FirstOrDefault(q => q.CompanyName == companyName)?.ID ?? -1;
    }
}
