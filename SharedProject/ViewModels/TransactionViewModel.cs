namespace SharedProject.ViewModels;

public class TransactionViewModel : BindableBase
{
    private string selectedCompanyName;
    public string SelectedCompanyName
    {
        get => selectedCompanyName;
        set => SetProperty(ref selectedCompanyName, value);
    }

    private string entryDate = string.Empty;
    public string EntryDate
    {
        get => entryDate;
        set => SetProperty(ref entryDate, value);
    }

    private string entryPrice = string.Empty;
    public string EntryPrice
    {
        get => entryPrice;
        set => SetProperty(ref entryPrice, value);
    }

    private string positionSize = string.Empty;
    public string PositionSize
    {
        get => positionSize;
        set => SetProperty(ref positionSize, value);
    }

    private string numberOfShares = string.Empty;
    public string NumberOfShares
    {
        get => numberOfShares;
        set => SetProperty(ref numberOfShares, value);
    }

    private string avgSellPrice = string.Empty;
    public string AvgSellPrice
    {
        get => avgSellPrice;
        set => SetProperty(ref avgSellPrice, value);
    }

    private string initialDescription = string.Empty;
    public string InitialDescription
    {
        get => initialDescription;
        set => SetProperty(ref initialDescription, value);
    }

    private string description = string.Empty;
    public string Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    private string informationLink = string.Empty;
    public string InformationLink
    {
        get => informationLink;
        set => SetProperty(ref informationLink, value);
    }
}
