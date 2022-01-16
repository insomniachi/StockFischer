using StockFischer.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace StockFischer;

/// <summary>
/// Interaction logic for MoveList.xaml
/// </summary>
public partial class MoveList
{
    public static readonly DependencyProperty BoardProperty =
        DependencyProperty.Register("Board", typeof(LiveBoard), typeof(MoveList), new PropertyMetadata(null));

    public MoveList()
    {
        InitializeComponent();
    }

    public LiveBoard Board
    {
        get => (LiveBoard)GetValue(BoardProperty);
        set => SetValue(BoardProperty, value);
    }
    

    private void OnClicked(object sender, MouseButtonEventArgs e)
    {
        var pair = ((ContentControl) sender).DataContext as MovePair;
        var move = ((ContentControl) sender).Tag as MoveModel;

        Board.Moves.CurrentPair = pair;
        Board.Moves.Current = move;
        
        Board.GoToMove(move);
    }

    private void PrevClick(object sender, MouseButtonEventArgs e)
    {
        Board.GoBack();
    }

    private void NextClick(object sender, MouseButtonEventArgs e)
    {
        Board.GoForward();
    }

    private void PlayClick(object sender, MouseButtonEventArgs e)
    {
        Task.Run(() =>
        {
            Dispatcher.Invoke(async () =>
            {
                while (Board.GoForward())
                {
                    await Task.Delay(1000);
                }
            });

        });
    }
}

public class IsCurrentMoveConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values[0] == values[1] ? FontWeights.Bold : FontWeights.Normal;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}