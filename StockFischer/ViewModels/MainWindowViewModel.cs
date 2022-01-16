using OpenPGN;
using OpenPGN.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StockFischer.Messages;
using System.Windows.Input;

namespace StockFischer.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        private readonly IViewService _viewService;

        public ICommand OpenPgnCommand { get; }
        public ICommand AutoPlayCommand { get; }
        public ICommand StopEngineCommand { get; }
        public RoutingState Router { get; } = new();

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
        }
        
    }
}
