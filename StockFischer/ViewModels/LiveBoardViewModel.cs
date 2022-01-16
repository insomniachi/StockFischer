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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StockFischer.ViewModels
{
    public class LiveBoardViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IViewService _viewService;
        public string UrlPathSegment => "liveboard";

        public IScreen HostScreen { get; }

        public LiveBoardViewModel(IScreen screen, IViewService viewService, AppSettings settings)
        {
            _viewService = viewService;
            HostScreen = screen;
            
            Board = LiveBoard.NewGame();

            //this.WhenAnyValue(x => x.Board)
            //    .Buffer(2, 1)
            //    .Select(b => (Previous: b[0], Current: b[1]))
            //    .Subscribe(OnBoardChanged);

            LoadFenCommand = ReactiveCommand.Create(LoadFen);
            TogglePerspectiveCommand = ReactiveCommand.Create(TogglePerspective);
            GoBackCommand = ReactiveCommand.Create(Board.GoBack);
            GoForwardCommand = ReactiveCommand.Create(Board.GoForward);

            var engineInfo = settings.Engines.Single(x => x.Name == settings.DefaultEngine);
            if(System.IO.File.Exists(engineInfo.Path))
            {
                _engine = new(engineInfo);
                _engine.PotentialVariationCalculated += OnPotentialVariationCalculated;
            }

            CanUseEngine = _engine is { };

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

        private UCIEngine _engine;
        private LiveBoard _board;
        public LiveBoard Board
        {
            get => _board;
            set
            {
                if (_board is not null)
                {
                    ((INotifyPropertyChanged)_board.Moves).PropertyChanged -= MoveCollectionPropertyChanged;
                }

                _board = value;

                this.RaisePropertyChanged(nameof(Board));

                ((INotifyPropertyChanged)_board.Moves).PropertyChanged += MoveCollectionPropertyChanged;
            }
        }

        [Reactive]
        public Color Perspective { get; set; } = Color.White;

        [Reactive]
        public bool IsPromoting { get; set; }

        [Reactive]
        public PotentialVariationModel EngineVariation { get; set; }

        public string InputFen { get; set; }
        public bool AutoPlayEnabled { get; set; }
        public bool CanUseEngine { get; set; }
        public ICommand LoadFenCommand { get; set; }
        public ICommand TogglePerspectiveCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand GoForwardCommand { get; set; }


        /// <summary>
        /// Load positon from fen
        /// </summary>
        private void LoadFen() => Board = LiveBoard.FromFen(InputFen);

        /// <summary>
        /// Toggle board perspective
        /// </summary>
        private void TogglePerspective() => Perspective = Perspective.Invert();


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
    }
}
