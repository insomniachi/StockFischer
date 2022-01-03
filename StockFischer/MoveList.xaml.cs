using OpenPGN.Models;
using StockFischer.Models;
using StockFischer.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StockFischer
{
    /// <summary>
    /// Interaction logic for MoveList.xaml
    /// </summary>
    public partial class MoveList : UserControl
    {
        public static readonly DependencyProperty MovesProperty =
            DependencyProperty.Register("Moves", typeof(MoveCollection), typeof(MoveList), new PropertyMetadata(null));

        public MoveList()
        {
            InitializeComponent();
        }

        public MoveCollection Moves
        {
            get { return (MoveCollection)GetValue(MovesProperty); }
            set { SetValue(MovesProperty, value); }
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if(e.AddedCells.Count == 0)
            {
                return;
            }

            DataGridCellInfo item = e.AddedCells.SingleOrDefault();

            bool isWhite = item.Column.DisplayIndex == 0;

            Moves.CurrentPair = item.Column.GetCellContent(item.Item).DataContext as MovePair;

            Moves.Current = isWhite ? Moves.CurrentPair.White : Moves.CurrentPair.Black;

            MainWindowViewModel vm = DataContext as MainWindowViewModel;
            vm.Board.Load(BoardSetup.FromFen(Moves.Current.Fen));
        }

        private void PrevClick(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel vm = DataContext as MainWindowViewModel;

            if(Moves.GoBack())
            {
                vm.Board.Load(BoardSetup.FromFen(Moves.Current.Fen));
            }
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel vm = DataContext as MainWindowViewModel;

            if (Moves.GoForward())
            {
                vm.Board.Load(BoardSetup.FromFen(Moves.Current.Fen));
            }
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel vm = DataContext as MainWindowViewModel;

            Task.Run(() =>
            {
                Dispatcher.Invoke(async () =>
                {
                    while (Moves.GoForward())
                    {
                        vm.Board.Load(BoardSetup.FromFen(Moves.Current.Fen));
                        await Task.Delay(1000);
                    }
                });

            });
        }
    }
}
