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
            translations = new List<Translation>();

            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                , "Pogromcy\\tabela.txt");

            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var lineWords = line.Trim().Split(' ');

                if (!lineWords.Any() || lineWords.Count() < 2)
                    continue;

                if (!lineWords.Last().All(q => char.IsUpper(q) || char.IsDigit(q)))
                {
                    continue;
                }
                Translation translation = new Translation
                {
                    Key = String.Join(" ", lineWords.Reverse().Skip(1).Reverse()),
                    Value = lineWords.Last()
                };
                if (translation.Key == "" || translation.Value == "")
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
                Company company = new();
                company.CompanyName = item.Value;
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
