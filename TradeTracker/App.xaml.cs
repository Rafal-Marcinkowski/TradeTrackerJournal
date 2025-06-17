using DataAccess.Data;
using EFCore.Data;
using EventTracker.Module;
using HotStockTracker.Module;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Logging;
using Infrastructure.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using SessionOpening.Module;
using StockNotepad.Module;
using System.Windows;
using TradeTracker.Module;
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

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<TradeTrackerModule>();
        moduleCatalog.AddModule<InfrastructureModule>();
        moduleCatalog.AddModule<EventTrackerModule>();
        moduleCatalog.AddModule<SessionOpeningModule>();
        moduleCatalog.AddModule<HotStockTrackerModule>();
        moduleCatalog.AddModule<StockNotepadModule>();
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

        containerRegistry.RegisterInstance(Log.Logger);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Koniec aplikacji. \n");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}