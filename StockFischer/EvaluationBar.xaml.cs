using StockFischer.Engine;
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
            DependencyProperty.Register("Evaluation", typeof(Evaluation), typeof(EvaluationBar), new PropertyMetadata(null, OnEvaluationChanged));

        private static void OnEvaluationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eb = d as EvaluationBar;

            if(e.NewValue is Evaluation evaluation)
            {
                var barHeight = eb.Bar.ActualHeight;
                var blackBarHeight = barHeight / 2;
                var ev = evaluation.Score;
                double offset;
                
                if (double.IsNegativeInfinity(ev))
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

                if(ev >= 0)
                {
                    eb.EvaluationText.VerticalAlignment = VerticalAlignment.Bottom;
                    eb.EvaluationText.Foreground = Brushes.Black;
                }
                else
                {
                    eb.EvaluationText.VerticalAlignment = VerticalAlignment.Top;
                    eb.EvaluationText.Foreground = Brushes.White;
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
