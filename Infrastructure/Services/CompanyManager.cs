﻿using DataAccess.Data;
using Infrastructure.Interfaces;
using SharedProject.Models;

namespace Infrastructure.Services;

public class CompanyManager(ICompanyData companyData) : ICompanyManager
{
    public async Task<IEnumerable<Company>> GetAllCompanies()
    {
        return await companyData.GetAllCompaniesAsync();
    }

    public async Task<int> GetCompanyId(string companyName)
    {
        return await companyData.GetCompanyID(companyName);
    }

    public async Task UpdateCompany(Company company)
    {
        await companyData.UpdateCompanyAsync(company.ID, company.CompanyName,
            company.TransactionCount, company.EventCount, company.NoteCount);
    }

    public async Task IncrementNoteCount(string companyName)
    {
        var id = await companyData.GetCompanyID(companyName);
        var company = await companyData.GetCompanyAsync(id);
        company.NoteCount++;
        await UpdateCompany(company);
    }

    public async Task DecrementNoteCount(string companyName)
    {
        var id = await companyData.GetCompanyID(companyName);
        var company = await companyData.GetCompanyAsync(id);
        if (company.NoteCount > 0)
        {
            company.NoteCount--;
        }
        await UpdateCompany(company);
    }
}
