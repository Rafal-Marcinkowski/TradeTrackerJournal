using System.Collections.ObjectModel;
using System.IO;
using TradeTracker.MVVM.Models;
using Xceed.Wpf.Toolkit;

namespace TradeTracker;

public class Initialize
{
    public static ObservableCollection<Company> FillCompanies()
    {
        try
        {
            companies = new ObservableCollection<Company>();
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
                companies.Add(company);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Nie udało się zainicjować translacji bo: " + ex.Message);
        }
        return companies;
    }

    private static ObservableCollection<Company> companies;
    private static List<Translation> translations;

    public class Translation
    {
        public string Key { get; set; } = "";

        public string Value { get; set; } = "";
    }
}
