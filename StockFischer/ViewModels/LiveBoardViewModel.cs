using Microsoft.Extensions.Logging;
using OpenPGN;
using OpenPGN.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StockFischer.Engine;
using StockFischer.Messages;
using StockFischer.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileIO = System.IO.File;

namespace StockFischer.ViewModels;

public class LiveBoardViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly UCIEngine _engine;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="screen"></param>
    /// <param name="logger"></param>
    /// <param name="settings"></param>
    public LiveBoardViewModel(IScreen screen, ILogger<LiveBoardViewModel> logger, AppSettings settings)
    {
        _logger = logger;
        HostScreen = screen;

        LoadFenCommand = ReactiveCommand.Create<string>(LoadFen);
        TogglePerspectiveCommand = ReactiveCommand.Create(TogglePerspective);

        var engineInfo = settings.Engines.SingleOrDefault(x => x.Name == settings.DefaultEngine);
        if (engineInfo is { } && FileIO.Exists(engineInfo.Path))
        {
            _engine = new(engineInfo);
            _engine.PotentialVariationCalculated += OnPotentialVariationCalculated;
        }

        CanUseEngine = _engine is { };

        OnBoardChanged(new(null, Board));

        SubscribeEvents();
    }

    /// <summary>
    /// Model for the game
    /// </summary>
    [Reactive]
    public LiveBoard Board { get; set; } = LiveBoard.NewGame();

    /// <summary>
    /// Orientation of the board, are we viewing the board from White side or black.
    /// </summary>
    [Reactive]
    public Color Perspective { get; set; } = Color.White;

    /// <summary>
    /// Variations spat out by the engine are regularly updated in this property.
    /// will always have last calculated variation
    /// </summary>
    [Reactive]
    public PotentialVariationModel EngineVariation { get; set; }

    /// <summary>
    /// Implementation for <see cref="IRoutableViewModel.UrlPathSegment"/>
    /// </summary>
    public string UrlPathSegment { get; } = "liveboard";

    /// <summary>
    /// Implementation for <see cref="IRoutableViewModel.HostScreen"/>
    /// </summary>
    public IScreen HostScreen { get; }


    /// <summary>
    /// A flag to know whether engine can play the move on the board when desired
    /// depth is reached
    /// </summary>
    public bool AutoPlayEnabled { get; set; }

    /// <summary>
    /// Can we use engine, (is false when there is nothing in config or the engine doesn't exist on 
    /// <see cref="UCIEngineInfo.Path"/>)
    /// </summary>
    public bool CanUseEngine { get; }

    /// <summary>
    /// Command to load fen
    /// </summary>
    public ICommand LoadFenCommand { get; set; }

    /// <summary>
    /// Command to toggle perspective
    /// </summary>
    public ICommand TogglePerspectiveCommand { get; set; }


    /// <summary>
    /// Load positon from fen
    /// </summary>
    private void LoadFen(string fen) => Board = LiveBoard.FromFen(fen);

    /// <summary>
    /// Toggle board perspective
    /// </summary>
    private void TogglePerspective() => Perspective = Perspective.Invert();

    /// <summary>
    /// Called when the current move displayed in UI changes, so we trigger engine to start analyzing
    /// from the current position
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MoveCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Current" && CanUseEngine)
        {
            _engine.Stop();
            _engine.SetFenPosition(Board.Moves.Current.Fen);
            await Task.Delay(100);
            _engine.Go();
        }
    }

    /// <summary>
    /// Gets called by the engine regulary by the engine when it calculates a variation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPotentialVariationCalculated(object sender, PotentialVariation e)
    {
        var moves = e.Moves.ToList();
        EngineVariation = new(Board.Moves.Current?.Fen ?? BoardSetup.StartingPosition, e);

        if (AutoPlayEnabled && e.Depth == 27)
        {
            _engine.Stop();

            await Task.Delay(100);

            var move = moves.First();

            Application.Current.Dispatcher.Invoke(() => Board.TryMakeMove(move.From, move.To));
        }
    }

    /// <summary>
    /// Subscribe to events
    /// </summary>
    private void SubscribeEvents()
    {
        this.WhenAnyValue(x => x.Board)
            .Buffer(2, 1)
            .Select(b => (OldValue: b[0], NewValue: b[1]))
            .Subscribe(OnBoardChanged);


        MessageBus.Current.Listen<KeyEventArgs>()
            .Subscribe(OnKeyDown);
        MessageBus.Current.Listen<GameOpenedMessage>()
            .Subscribe(g => Board = LiveBoard.FromGame(g.Game));
        MessageBus.Current.Listen<StopEngineMessage>()
            .Subscribe(_ =>
            {
                AutoPlayEnabled = false;
                _engine.Stop();
            });
        MessageBus.Current.Listen<StartAutoPlayMessage>()
            .Subscribe(_ =>
            {
                AutoPlayEnabled = true;
                _engine.SetFenPosition(Board.Moves.Current?.Fen ?? BoardSetup.StartingPosition);
                _engine.Go();
            });
    }

    /// <summary>
    /// Called whenever we change <see cref="Board"/> to unsubscribe events from old and subcribe to the
    /// new value
    /// </summary>
    /// <param name="e"></param>
    private void OnBoardChanged((LiveBoard OldValue, LiveBoard NewValue) e)
    {
        if (e.OldValue is { })
        {
            ((INotifyPropertyChanged)e.OldValue.Moves).PropertyChanged -= MoveCollectionPropertyChanged;
            e.OldValue.MovePlayed -= OnMovePlayed;
        }
        if (e.NewValue is { })
        {
            ((INotifyPropertyChanged)e.NewValue.Moves).PropertyChanged += MoveCollectionPropertyChanged;
            e.NewValue.MovePlayed += OnMovePlayed;
        }
    }

    /// <summary>
    /// Event fired whenever a move is played,
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMovePlayed(object sender, MoveModel e)
    {
        _logger.LogDebug("Move Played : {Move}", e);
    }


    /// <summary>
    /// Key presses from UI
    /// </summary>
    /// <param name="e"></param>
    private void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                Board.GoToStart();
                break;
            case Key.Down:
                Board.GoToEnd();
                break;
            case Key.Right:
                Board.GoForward();
                break;
            case Key.Left:
                Board.GoBack();
                break;
        }
    }
}
