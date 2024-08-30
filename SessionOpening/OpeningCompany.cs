using Prism.Mvvm;

namespace SessionOpening;

public class OpeningCompany : BindableBase
{
    private string companyName;
    public string CompanyName
    {
        get => companyName;
        set => SetProperty(ref companyName, value);
    }

    private decimal? maxPrice;
    public decimal? MaxPrice
    {
        get => maxPrice;
        set => SetProperty(ref maxPrice, value);
    }

    private decimal? positionSize;
    public decimal? Positionsize
    {
        get => positionSize;
        set => SetProperty(ref positionSize, value);
    }

    private int? numberOfShares;
    public int? NumberOfShares
    {
        get => numberOfShares;
        set => SetProperty(ref numberOfShares, value);
    }
}
