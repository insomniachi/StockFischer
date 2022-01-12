using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockFischer;
using StockFischer.ViewModels;
using System;
using System.Windows;

namespace Chess
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services => ConfigureServices(services))
                .ConfigureLogging(logging =>  logging.AddConsole())
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<MainWindow>();
        }


        protected async override void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var window = _host.Services.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.StopAsync();

            base.OnExit(e);
        }
    }
}
