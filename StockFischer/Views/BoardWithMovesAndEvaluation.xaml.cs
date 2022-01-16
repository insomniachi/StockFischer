using StockFischer.ViewModels;

namespace StockFischer.Views
{
    /// <summary>
    /// Interaction logic for BoardWithMovesAndEvaluation.xaml
    /// </summary>
    public partial class BoardWithMovesAndEvaluation
    {
        public BoardWithMovesAndEvaluation(LiveBoardViewModel vm)
        {
            InitializeComponent();
            DataContext = ViewModel = vm;
        }
    }
}
