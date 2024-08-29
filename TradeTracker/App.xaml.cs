using DataAccess.Data;
using DataAccess.DBAccess;
using Infrastructure;
using Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Prism.Unity;
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
        containerRegistry.RegisterInstance<ILogger>(LogManager.GetLogger());
        containerRegistry.RegisterSingleton<ITransactionData, TransactionData>();
        containerRegistry.RegisterSingleton<ITransactionCommentData, TransactionCommentData>();
        containerRegistry.RegisterSingleton<ICompanyData, CompanyData>();
        containerRegistry.RegisterSingleton<ISQLDataAccess, SQLDataAccess>();
        containerRegistry.RegisterSingleton<IDailyDataProvider, DailyDataProvider>();

        containerRegistry.Register<MainWindowViewModel>();
        containerRegistry.Register<TransactionsJournalMenuViewModel>();
        containerRegistry.Register<SessionOpeningViewModel>();
        containerRegistry.Register<EventsViewModel>();
        containerRegistry.Register<AddTransactionViewModel>();
        containerRegistry.Register<OpenPositionsViewModel>();
        containerRegistry.Register<TransactionsOverviewViewModel>();

        containerRegistry.RegisterForNavigation<TransactionsJournalMenuView>();
        containerRegistry.RegisterForNavigation<EventsView>();
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


