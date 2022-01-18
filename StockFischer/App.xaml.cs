using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using StockFischer.ViewModels;
using StockFischer.Views;
using System;
using System.Configuration;
using System.IO;
using System.Windows;

namespace StockFischer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;
    public static IServiceProvider Services { get; private set; }

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json");
            })
            .ConfigureServices(services =>
            {
                services.UseMicrosoftDependencyResolver();
                var resolver = Locator.CurrentMutable;
                resolver.InitializeSplat();
                resolver.InitializeReactiveUI();
                ConfigureServices(services);
            })
            .ConfigureLogging(logging => 
            {
                logging.ClearProviders();
                logging.AddSerilog();
            })
            .Build();

        Services = _host.Services;
        Services.UseMicrosoftDependencyResolver();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();
        services.AddSingleton<AppSettings>();
        services.AddTransient<IViewService, ViewService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<IScreen, MainWindowViewModel>(x => x.GetRequiredService<MainWindowViewModel>());
        services.AddTransient<LiveBoardViewModel>();
        services.AddTransient<SpectateViewModel>();
        services.AddTransient<MainWindow>();

        //Routing
        services.AddTransient<IViewFor<LiveBoardViewModel>, BoardWithMovesAndEvaluation>();
        services.AddTransient<IViewFor<SpectateViewModel>, SpectateGameView>();
    }


    protected async override void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var window = Services.GetRequiredService<MainWindow>();
        window.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host.StopAsync();

        base.OnExit(e);
    }
}
