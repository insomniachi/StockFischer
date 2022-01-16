using Fluent;
using ReactiveUI;

namespace StockFischer;

public class ReactiveRibbonWindow<TViewModel> : RibbonWindow, IViewFor<TViewModel> where TViewModel : class
{
    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel) value;
    }
    public TViewModel ViewModel { get; set; }
}

