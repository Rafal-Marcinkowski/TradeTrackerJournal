namespace HotStockTracker.MVVM.Views;

public partial class HotStockOverviewView
{
    public HotStockOverviewView()
    {
        InitializeComponent();
        Loaded += HotStockOverviewView_Loaded;
    }

    private void HotStockOverviewView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        DayItemsScrollViewer.ScrollToRightEnd();
        DayItemsScrollViewer.UpdateLayout();
    }
}
