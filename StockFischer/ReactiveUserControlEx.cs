using ReactiveUI;
using System;

namespace StockFischer;

public class ReactiveUserControlEx<TViewModel> : ReactiveUserControl<TViewModel>
    where TViewModel : class
{
    public ReactiveUserControlEx()
    {
        this.WhenAnyValue(x => x.ViewModel)
            .Subscribe(vm => DataContext = vm);
    }
}
