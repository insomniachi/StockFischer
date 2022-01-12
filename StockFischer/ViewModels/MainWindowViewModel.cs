using OpenPGN;
using OpenPGN.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StockFischer.Engine;
using StockFischer.Models;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StockFischer.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private UCIEngine _engine;
        private LiveBoard _board;
        public LiveBoard Board
        {
            get => _board;
            set
            {
                if(_board is not null)
                {
                    ((INotifyPropertyChanged)_board.Moves).PropertyChanged -= MainWindowViewModel_PropertyChanged;
                }

                _board = value;

                this.RaisePropertyChanged(nameof(Board));

                ((INotifyPropertyChanged)_board.Moves).PropertyChanged += MainWindowViewModel_PropertyChanged;
            }
        }

        private async void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Current")
            {
                StopEngine();
                _engine.SetFenPosition(Board.Moves.Current.Fen);
                await Task.Delay(500);
                _engine.Go();
            }
        }

        [Reactive]
        public Color Perspective { get; set; } = Color.White;

        [Reactive]
        public bool IsPromoting { get; set; }

        [Reactive]
        public PotentialVariation EngineVariation { get; set; }

        public string InputFen { get; set; }
        public bool AutoPlayEnabled { get; set; }

        public ICommand LoadFenCommand { get; set; }
        public ICommand TogglePerspectiveCommand { get; set; }
        public ICommand OpenPgnCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand GoForwardCommand { get; set; }
        public ICommand AutoPlayCommand { get; set; }
        public ICommand StopEngineCommand { get; set; }

        public MainWindowViewModel()
        {
            Board = LiveBoard.NewGame();
            //Board = LiveBoard.FromPgnFile(@"C:\Users\Athul\Desktop\pgn.txt");
            //Board.MovePlayed += OnMove;

            //this.WhenAnyValue(x => x.Board)
            //    .Buffer(2, 1)
            //    .Select(b => (Previous: b[0], Current: b[1]))
            //    .Subscribe(OnBoardChanged);

            LoadFenCommand = ReactiveCommand.Create(LoadFen);
            TogglePerspectiveCommand = ReactiveCommand.Create(TogglePerspective);
            OpenPgnCommand = ReactiveCommand.Create(OpenPgn);
            GoBackCommand = ReactiveCommand.Create(Board.GoBack);
            GoForwardCommand = ReactiveCommand.Create(Board.GoForward);
            AutoPlayCommand = ReactiveCommand.Create(AutoPlay);
            StopEngineCommand = ReactiveCommand.Create(StopEngine);

            // TODO make this path configurable
            _engine = new(@"C:\Users\Athul\Downloads\stockfish_14_win_x64_avx2\stockfish_14_win_x64_avx2\stockfish_14_x64_avx2.exe");
            _engine.PotentialVariationCalculated += OnPotentialVariationCalculated;
        }

        /// <summary>
        /// Gets called by the engine regulary by the engine when it calculates a variation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPotentialVariationCalculated(object sender, PotentialVariation e)
        {
            // if engine is calculating blacks move, invert the evaluation
            if(Board.ActiveColor == Color.Black)
            {
                e.Evaluation *= -1;
                e.MateIn *= -1;
            }

            var move = e.Moves.First();

            if (AutoPlayEnabled && e.Depth == 27)
            {
                StopEngine();
                Application.Current.Dispatcher.Invoke(() => Board.TryMove(move.OriginSquare, move.TargetSquare)); 
            }

            EngineVariation = e;
        }

        /// <summary>
        /// Load positon from fen
        /// </summary>
        private void LoadFen() => Board = LiveBoard.FromFen(InputFen);
        
        /// <summary>
        /// Toggle board perspective
        /// </summary>
        private void TogglePerspective() => Perspective = Perspective.Invert();
        
        /// <summary>
        /// Load position and moves made from pgn
        /// </summary>
        private void OpenPgn()
        {
            Microsoft.Win32.OpenFileDialog dialog = new();
            var result = dialog.ShowDialog();

            if(result == true)
            {
                Board = LiveBoard.FromPgnFile(dialog.FileName);
            }
        }

        /// <summary>
        /// Auto play with engine from the current postion
        /// </summary>
        private void AutoPlay()
        {
            AutoPlayEnabled = true;

            if(Board.Moves.Current is null)
            {
                _engine.SetNewGamePosition();
            }
            else
            {
                _engine.SetFenPosition(Board.Moves.Current.Fen);
            }

            _engine.Go();
        }

        /// <summary>
        /// Tell engine to stop calculating
        /// </summary>
        private void StopEngine() => _engine.Stop();
    }
}
