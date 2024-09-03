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

    private string maxPrice;
    public string MaxPrice
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

    private string positionSize;
    public string PositionSize
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
        if (decimal.TryParse(MaxPrice, out decimal result) && result > 0)
        {
            if (decimal.TryParse(PositionSize, out decimal posSize))
            {
                NumberOfShares = (int)(posSize / result);
            }
        }

        else
        {
            NumberOfShares = 0;
        }
    }
}
