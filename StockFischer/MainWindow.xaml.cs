using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using StockFischer.ViewModels;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;

namespace StockFischer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow(MainWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = ViewModel = vm;

        MessageBus.Current.RegisterMessageSource(this.Events().PreviewKeyDown);

        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
                .DisposeWith(d);

            ViewModel.Router.Navigate.Execute(App.Services.GetRequiredService<LiveBoardViewModel>());
        });

    }
}
