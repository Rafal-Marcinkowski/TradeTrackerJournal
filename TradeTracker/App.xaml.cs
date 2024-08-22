using DataAccess.Data;
using DataAccess.DBAccess;
using Infrastructure;
using Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using Serilog;
using System.IO;
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
        Log.Information("Application Starting");

        Task.Run(() =>
        {
            var text = Infrastructure.DownloadHtmlData.DownloadPageSource.DownloadHtmlAsync("BSH").Result;
            File.WriteAllText("C:\\Users\\rafal\\Desktop\\Pogromcy\\TradeTrackerJournal\\ZawartoscStrony", text);
        });

        TurnoverMedianTable.UpdateMedianTable();
        Task.Run(async () =>
        {
            await Task.Delay(5000);
            DailyTradeTracker tradeTracker = new(Container.Resolve<ITransactionData>(), Container.Resolve<IDailyDataProvider>(), Container.Resolve<IEventAggregator>());
            tradeTracker.StartTracker();
        });
        // FirstStartUp();
    }

    private void FirstStartUp()
    {
        Infrastructure.FirstStartUp.Initialize init = new(new CompanyData(new SQLDataAccess(Container.Resolve<IConfiguration>())));
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

        containerRegistry.RegisterSingleton<ITransactionData, TransactionData>();
        containerRegistry.RegisterSingleton<ITransactionCommentData, TransactionCommentData>();
        containerRegistry.RegisterSingleton<ICompanyData, CompanyData>();
        containerRegistry.RegisterSingleton<ISQLDataAccess, SQLDataAccess>();
        containerRegistry.RegisterSingleton<IDailyDataProvider, DailyDataProvider>();

        containerRegistry.Register<MainWindowViewModel>();
        containerRegistry.Register<TransactionsJournalMenuViewModel>();
        containerRegistry.Register<EventsViewModel>();
        containerRegistry.Register<AddTransactionViewModel>();
        containerRegistry.Register<OpenPositionsViewModel>();
        containerRegistry.Register<TransactionsOverviewViewModel>();

        containerRegistry.RegisterForNavigation<TransactionsJournalMenuView>();
        containerRegistry.RegisterForNavigation<EventsView>();
        containerRegistry.RegisterForNavigation<AddTransactionView>();
        containerRegistry.RegisterForNavigation<OpenPositionsView>();
        containerRegistry.RegisterForNavigation<TransactionsOverviewView>();
        containerRegistry.RegisterForNavigation<TransactionsOverviewMenuView>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        Log.CloseAndFlush();
    }
}


