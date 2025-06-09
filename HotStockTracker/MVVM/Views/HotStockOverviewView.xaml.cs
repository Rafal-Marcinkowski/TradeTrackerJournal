using System.Windows;
using System.Windows.Controls;

namespace HotStockTracker.MVVM.Views;

public partial class HotStockOverviewView
{
    public HotStockOverviewView()
    {
        InitializeComponent();
    }

    private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
    {
        var scrollViewer = sender as ScrollViewer;
        scrollViewer?.ScrollToRightEnd();
    }
}
