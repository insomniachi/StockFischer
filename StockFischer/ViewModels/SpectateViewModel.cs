using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StockFischer.Liches;
using System.Windows.Input;

namespace StockFischer.ViewModels;

public class SpectateViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ILogger _logger;
    public string UrlPathSegment => "watch";
    public IScreen HostScreen { get; }

    [Reactive]
    public LiveGameModel Game { get; set; }


    public ICommand StreamGameCommand { get; }

    public SpectateViewModel(IScreen screen, ILogger<SpectateViewModel> logger)
    {
        _logger = logger;
        HostScreen = screen;

        StreamGameCommand = ReactiveCommand.Create<string>(StreamGame);
    }

    private void StreamGame(string gameId) => Game = new LiveGameModel(gameId, _logger);
}
