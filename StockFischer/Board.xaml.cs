using StockFischer.Models;
using OpenPGN;
using OpenPGN.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace StockFischer
{

    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        public static readonly DependencyProperty LiveBoardProperty =
            DependencyProperty.Register("LiveBoard", typeof(LiveBoard), typeof(Board), new PropertyMetadata(null, OnLiveBoardChanged));
        public static readonly DependencyProperty PerspectiveProperty =
            DependencyProperty.Register("Perspective", typeof(Color), typeof(Board), new PropertyMetadata(Color.White));


        public Board()
        {
            InitializeComponent();
        }

        public LiveBoard LiveBoard
        {
            get { return (LiveBoard)GetValue(LiveBoardProperty); }
            set { SetValue(LiveBoardProperty, value); }
        }

        public Color Perspective
        {
            get { return (Color)GetValue(PerspectiveProperty); }
            set { SetValue(PerspectiveProperty, value); }
        }

        private static void OnLiveBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = d as Board;

            if (e.NewValue is LiveBoard b)
            {
                board!.ChessBoard.ItemsSource = b.Elements;
            }
        }

        private void BoardMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(sender as Canvas);
            int x = (int)pos.X;
            int y = (int)pos.Y;

            Square square = Perspective == Color.White
                ? Square.New(FileExtensions.FromInt(x, true), 8 - y)
                : Square.New(FileExtensions.FromInt(7 - x, true), y + 1);

            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                LiveBoard.OnSquareSelected(square);
            }

        }
    }

    public class FileToCanvasPositionConverter : IValueConverter
    {
        public Color Perspective { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            File file = (File)value;

            if (Perspective == Color.White)
            {
                return file.ToInt() - 1;
            }
            else
            {
                return 8 - file.ToInt();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class RankToCanvasPositionConverter : IValueConverter
    {
        public Color Perspective { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var rank = (int)value;

                if (Perspective == Color.White)
                {
                    return 8 - rank;
                }

                return rank - 1;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
