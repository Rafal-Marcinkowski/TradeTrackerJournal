using SharedProject.Services.Filtering;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace SharedProject.ViewModels;

public abstract class BaseListViewModel<T> : BaseViewModel
{
    private readonly IFilterService _filterService;
    private ICollectionView _collectionView;

    private ObservableCollection<T> _itemsSource;
    public ObservableCollection<T> ItemsSource
    {
        get => _itemsSource;
        set
        {
            if (SetProperty(ref _itemsSource, value))
            {
                _collectionView = CollectionViewSource.GetDefaultView(ItemsSource);
                _collectionView.Filter = _filterService.GetFilterPredicate();
            }
        }
    }

    private string _searchKeyword;
    public string SearchKeyword
    {
        get => _searchKeyword;
        set
        {
            if (SetProperty(ref _searchKeyword, value))
            {
                _filterService.ApplyFilter(value);
                _collectionView.Refresh();
                OnCollectionFiltered();
            }
        }
    }

    protected BaseListViewModel(IFilterService filterService = null)
    {
        _filterService = filterService ?? new FilterService<T>();
    }

    protected virtual void OnCollectionFiltered()
    {
        // Optional: Add custom logic after filtering
    }
}
