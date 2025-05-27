using DataAccess.Data;
using DataAccess.DBAccess;
using EFCore.Data;
using EventTracker.MVVM.ViewModels;
using EventTracker.MVVM.Views;
using HotStockTracker.MVVM.ViewModels;
using HotStockTracker.MVVM.Views;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Logging;
using Infrastructure.Services;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using SessionOpening;
using System.Windows;
using TradeTracker.MVVM.ViewModels;
using TradeTracker.MVVM.Views;

namespace TradeTracker;

public partial class App : PrismApplication
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        LogManager.InitializeLogger();
        Log.Information("Początek aplikacji.");

        _ = TurnoverMedianTable.UpdateMedianTable();

        Task.Run(async () =>
        {
            await Task.Delay(3000);
            IDailyTracker dailyTradeTracker = Container.Resolve<IDailyTracker>();
            _ = dailyTradeTracker.StartTracker();
        });

        //FirstStartUp();
    }

    private void FirstStartUp()
    {
        Infrastructure.FirstStartUp.Initialize init = new(Container.Resolve<ICompanyData>());
        init.FillDatabaseWithCompanies();
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<AppDbContext>(() =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(Container.Resolve<IConfiguration>().GetConnectionString("HotStockEFCore_DBConnectionString"));
            return new AppDbContext(optionsBuilder.Options);
        });

        containerRegistry.RegisterInstance<IConfiguration>(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build());

        containerRegistry.RegisterSingleton<IDailyTracker, DailyTracker>();
        containerRegistry.RegisterInstance(Log.Logger);
        containerRegistry.RegisterSingleton<ITransactionData, TransactionData>();
        containerRegistry.RegisterSingleton<ICommentData, CommentData>();
        containerRegistry.RegisterSingleton<IEventData, EventData>();
        containerRegistry.RegisterSingleton<ICompanyData, CompanyData>();
        containerRegistry.RegisterSingleton<ISQLDataAccess, SQLDataAccess>();
        containerRegistry.RegisterSingleton<IDialogCoordinator, DialogCoordinator>();
        containerRegistry.RegisterSingleton<IDailyDataProvider, DailyDataProvider>();

        containerRegistry.Register<MainWindowViewModel>();
        containerRegistry.Register<EventsMainMenuViewModel>();
        containerRegistry.Register<TransactionsJournalMenuViewModel>();
        containerRegistry.Register<AddEventViewModel>();
        containerRegistry.Register<AddTransactionViewModel>();
        containerRegistry.Register<EventsMainMenuViewModel>();
        containerRegistry.Register<EventsOverviewViewModel>();
        containerRegistry.Register<OpenPositionsViewModel>();
        containerRegistry.Register<TransactionsOverviewViewModel>();
        containerRegistry.RegisterSingleton<SessionOpeningViewModel>();
        containerRegistry.RegisterSingleton<HotStockOverviewViewModel>();

        containerRegistry.RegisterSingleton<ICommentManager, CommentManager>();
        containerRegistry.RegisterSingleton<IViewManager, ViewManager>();
        containerRegistry.RegisterSingleton<ITransactionManager, TransactionManager>();
        containerRegistry.RegisterSingleton<IEventManager, Infrastructure.Services.EventManager>();
        containerRegistry.RegisterSingleton<ICompanyManager, CompanyManager>();
        containerRegistry.RegisterSingleton<ITradeTrackerFacade, TradeTrackerFacade>();

        containerRegistry.RegisterForNavigation<HotStockOverviewView>();
        containerRegistry.RegisterForNavigation<TransactionsJournalMenuView>();
        containerRegistry.RegisterForNavigation<EventsMainMenuView>();
        containerRegistry.RegisterForNavigation<AddEventView>();
        containerRegistry.RegisterForNavigation<SessionOpeningView>();
        containerRegistry.RegisterForNavigation<AddTransactionView>();
        containerRegistry.RegisterForNavigation<OpenPositionsView>();
        containerRegistry.RegisterForNavigation<TransactionsOverviewView>();
        containerRegistry.RegisterForNavigation<TransactionsOverviewMenuView>();
        containerRegistry.RegisterForNavigation<EventsOverviewMenuView>();
        containerRegistry.RegisterForNavigation<EventsOverviewView>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Koniec aplikacji. \n");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
