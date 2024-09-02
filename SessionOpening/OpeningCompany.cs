using Prism.Mvvm;

namespace SessionOpening;

public class OpeningCompany : BindableBase
{
    private string companyName = string.Empty;
    public string CompanyName
    {
        get => companyName;
        set => SetProperty(ref companyName, value);
    }

    private decimal maxPrice;
    public decimal MaxPrice
    {
        get => maxPrice;
        set
        {
            if (SetProperty(ref maxPrice, value))
            {
                CalculateNumberOfShares();
            }
        }
    }

    private decimal positionSize;
    public decimal PositionSize
    {
        get => positionSize;
        set
        {
            if (SetProperty(ref positionSize, value))
            {
                CalculateNumberOfShares();
            }
        }
    }

    private int numberOfShares;
    public int NumberOfShares
    {
        get => numberOfShares;
        set => SetProperty(ref numberOfShares, value);
    }

    private void CalculateNumberOfShares()
    {
        if (MaxPrice > 0)
        {
            NumberOfShares = (int)(PositionSize / MaxPrice);
        }

        else
        {
            NumberOfShares = 0;
        }
    }
}
