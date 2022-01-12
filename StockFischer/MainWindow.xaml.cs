using StockFischer.ViewModels;
using System.Windows;
using System.Windows.Input;
using ReactiveUI;

namespace StockFischer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext =  ViewModel = new();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    ViewModel!.Board.GoToStart();
                    break;
                case Key.Down:
                    ViewModel!.Board.GoToEnd();
                    break;
                case Key.Right:
                    ViewModel!.Board.GoForward();
                    break;
                case Key.Left:
                    ViewModel!.Board.GoBack();
                    break;
            }
        }
    }
}
