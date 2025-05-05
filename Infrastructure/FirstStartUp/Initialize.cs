using DataAccess.Data;
using SharedProject.Models;
using System.IO;
using System.Windows;

namespace Infrastructure.FirstStartUp;

public class Initialize(ICompanyData companyData)
{
    public void FillDatabaseWithCompanies()
    {
        try
        {
            translations = [];

            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                , "Pogromcy\\tabela.txt");

            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var lineWords = line.Trim().Split(' ');

                if (lineWords.Length == 0 || lineWords.Length < 2)
                    continue;

                if (!lineWords[^1].All(q => char.IsUpper(q) || char.IsDigit(q)))
                {
                    continue;
                }
                Translation translation = new()
                {
                    Key = String.Join(" ", lineWords.Reverse().Skip(1).Reverse()),
                    Value = lineWords[^1]
                };
                if (translation.Key?.Length == 0 || translation.Value?.Length == 0)
                {
                    continue;
                }
                if (translations.Any(q => q.Value == translation.Value))
                {
                    continue;
                }
                translations.Add(translation);
            }

            foreach (var item in translations)
            {
                Company company = new()
                {
                    CompanyName = item.Value
                };
                companyData.InsertCompanyAsync(company.CompanyName, company.TransactionCount, company.EventCount);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Nie udało się zainicjować translacji bo: " + ex.Message);
        }
    }

    private static List<Translation> translations;

    public class Translation
    {
        public string Key { get; set; } = "";

        public string Value { get; set; } = "";
    }
}
