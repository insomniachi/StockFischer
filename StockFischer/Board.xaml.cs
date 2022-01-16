using OpenPGN.Models;
using StockFischer.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace StockFischer;

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
        get => (LiveBoard)GetValue(LiveBoardProperty);
        set => SetValue(LiveBoardProperty, value);
    }

    public Color Perspective
    {
        get => (Color)GetValue(PerspectiveProperty);
        set => SetValue(PerspectiveProperty, value);
    }

    private static void OnLiveBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var board = d as Board;

        if (e.NewValue is LiveBoard b)
        {
            board!.ChessBoard.ItemsSource = b.Elements;
        }
    }

    private void BoardMouseDown(object sender, MouseButtonEventArgs e)
    {
        var pos = e.GetPosition(sender as Canvas);
        var x = (int)pos.X;
        var y = (int)pos.Y;

        var square = Perspective == Color.White
            ? Square.New(FileExtensions.FromInt(x, true), 8 - y)
            : Square.New(FileExtensions.FromInt(7 - x, true), y + 1);

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            LiveBoard.TryMakeMove(square);
        }
        else if (e.RightButton == MouseButtonState.Pressed)
        {
            LiveBoard.OnHighlightSquareSelected(square);
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
