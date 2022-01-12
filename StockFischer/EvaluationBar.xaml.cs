using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFischer
{
    /// <summary>
    /// Interaction logic for EvaluationBar.xaml
    /// </summary>
    public partial class EvaluationBar : UserControl
    {
        private readonly DoubleAnimation _animation = new DoubleAnimation { Duration = TimeSpan.FromMilliseconds(500) };
        private readonly Storyboard _storyBoard = new();

        public static readonly DependencyProperty EvaluationProperty =
            DependencyProperty.Register("Evaluation", typeof(double), typeof(EvaluationBar), new PropertyMetadata(0.0, OnEvaluationChanged));

        private static void OnEvaluationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eb = d as EvaluationBar;

            if(e.NewValue is double ev)
            {
                var barHeight = eb.Bar.ActualHeight;
                var blackBarHeight = barHeight / 2;
                var offset = 0.0;

                if(double.IsNegativeInfinity(ev))
                {
                    offset = barHeight / 2;
                }
                else if(double.IsPositiveInfinity(ev))
                {
                    offset = -barHeight / 2;
                }
                else
                {
                    offset = blackBarHeight * -ev / 10;
                }

                var newHeight = blackBarHeight + offset;

                eb._animation.From = eb.BlackBar.ActualHeight;
                eb._animation.To = newHeight;

                eb._storyBoard.Begin(eb);

                eb.BlackBar.Height = newHeight;
            }
        }

        public EvaluationBar()
        {
            InitializeComponent();
            _storyBoard.Children.Add(_animation);
            Storyboard.SetTargetName(_animation, BlackBar.Name);
            Storyboard.SetTargetProperty(_animation, new PropertyPath(HeightProperty));

            Loaded += (_, __) => BlackBar.Height = Bar.ActualHeight / 2;
        }

        public double Evaluation
        {
            get { return (double)GetValue(EvaluationProperty); }
            set { SetValue(EvaluationProperty, value); }
        }

    }
}
