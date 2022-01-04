using System;
using System.Globalization;
using StockFischer.Models;
using StockFischer.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using OpenPGN.Models;

namespace StockFischer;

/// <summary>
/// Interaction logic for MoveList.xaml
/// </summary>
public partial class MoveList
{
    public static readonly DependencyProperty MovesProperty =
        DependencyProperty.Register("Moves", typeof(MoveCollection), typeof(MoveList), new PropertyMetadata(null));

    private MainWindowViewModel viewModel = null;
    public MoveList()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        viewModel = e.NewValue as MainWindowViewModel;
    }

    public MoveCollection Moves
    {
        get => (MoveCollection)GetValue(MovesProperty);
        set => SetValue(MovesProperty, value);
    }
    

    private void OnClicked(object sender, MouseButtonEventArgs e)
    {
        var pair = ((ContentControl) sender).DataContext as MovePair;
        var move = ((ContentControl) sender).Tag as MoveModel;

        Moves.CurrentPair = pair;
        Moves.Current = move;
        
        viewModel.Board.GoToMove(move);
    }

    private void PrevClick(object sender, MouseButtonEventArgs e)
    {
        MainWindowViewModel vm = DataContext as MainWindowViewModel;
        vm.Board.GoBack();
    }

    private void NextClick(object sender, MouseButtonEventArgs e)
    {
        MainWindowViewModel vm = DataContext as MainWindowViewModel;
        vm.Board.GoForward();
    }

    private void PlayClick(object sender, MouseButtonEventArgs e)
    {
        MainWindowViewModel vm = DataContext as MainWindowViewModel;

        Task.Run(() =>
        {
            Dispatcher.Invoke(async () =>
            {
                while (vm.Board.GoForward())
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