using DataAccess.Data;
using DataAccess.DBAccess;
using EventsTracker.MVVM.ViewModels;
using EventsTracker.MVVM.Views;
using Infrastructure;
using Infrastructure.Logging;
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

        TurnoverMedianTable.UpdateMedianTable();

        Task.Run(async () =>
        {
            await Task.Delay(3000);
            DailyTradeTracker dailyTradeTracker = Container.Resolve<DailyTradeTracker>();
            dailyTradeTracker.StartTracker();
        });
        //FirstStartUp();
    }

    private void FirstStartUp()
    {
        Infrastructure.FirstStartUp.Initialize init = new(new CompanyData(new SQLDataAccess(Container.Resolve<IConfiguration>(), Container.Resolve<ILogger>())));
        init.FillDatabaseWithCompanies();
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance<IConfiguration>(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build());

        containerRegistry.RegisterSingleton<DailyTradeTracker>();
        containerRegistry.RegisterInstance<ILogger>(Log.Logger);
        containerRegistry.RegisterSingleton<ITransactionData, TransactionData>();
        containerRegistry.RegisterSingleton<ITransactionCommentData, TransactionCommentData>();
        containerRegistry.RegisterSingleton<ICompanyData, CompanyData>();
        containerRegistry.RegisterSingleton<ISQLDataAccess, SQLDataAccess>();
        containerRegistry.RegisterSingleton<IDailyDataProvider, DailyDataProvider>();

        containerRegistry.Register<MainWindowViewModel>();
        containerRegistry.Register<EventsMainMenuViewModel>();
        containerRegistry.Register<TransactionsJournalMenuViewModel>();
        containerRegistry.Register<EventsViewModel>();
        containerRegistry.Register<AddEventViewModel>();
        containerRegistry.Register<AddTransactionViewModel>();
        containerRegistry.Register<OpenPositionsViewModel>();
        containerRegistry.Register<TransactionsOverviewViewModel>();
        containerRegistry.RegisterSingleton<SessionOpeningViewModel>();

        containerRegistry.RegisterForNavigation<TransactionsJournalMenuView>();
        containerRegistry.RegisterForNavigation<EventsMainMenuView, EventsMainMenuViewModel>();
        containerRegistry.RegisterForNavigation<AddEventView>();
        containerRegistry.RegisterForNavigation<SessionOpeningView, SessionOpeningViewModel>();
        containerRegistry.RegisterForNavigation<AddTransactionView>();
        containerRegistry.RegisterForNavigation<OpenPositionsView>();
        containerRegistry.RegisterForNavigation<TransactionsOverviewView>();
        containerRegistry.RegisterForNavigation<TransactionsOverviewMenuView>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Koniec aplikacji. \n");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}


