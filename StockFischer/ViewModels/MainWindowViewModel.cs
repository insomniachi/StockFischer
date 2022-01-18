using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using StockFischer.Messages;
using System.Windows.Input;

namespace StockFischer.ViewModels;

public class MainWindowViewModel : ReactiveObject, IScreen
{
    private readonly IViewService _viewService;

    /// <summary>
    /// Open a game from a pgn file
    /// </summary>
    public ICommand OpenPgnCommand { get; }

    /// <summary>
    /// Enable AutoPlay, engine will make move for both sides when desired depth is reached.
    /// </summary>
    public ICommand AutoPlayCommand { get; }

    /// <summary>
    /// Command to stop whatever the engine is doing now
    /// </summary>
    public ICommand StopEngineCommand { get; }

    public ICommand WatchGameCommand { get; }

    /// <summary>
    /// Navigation router
    /// </summary>
    public RoutingState Router { get; } = new();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="viewService"></param>
    public MainWindowViewModel(IViewService viewService)
    {
        _viewService = viewService;

        OpenPgnCommand = ReactiveCommand.Create(() =>
        {
            var game = _viewService.OpenPgn();

            if (game is { })
            {
                MessageBus.Current.SendMessage(new GameOpenedMessage { Game = game });
            }
        });
        AutoPlayCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(new StartAutoPlayMessage()));
        StopEngineCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(new StopEngineMessage()));
        WatchGameCommand = ReactiveCommand.Create(() => Router.Navigate.Execute(App.Services.GetRequiredService<SpectateViewModel>()));
    }

}
