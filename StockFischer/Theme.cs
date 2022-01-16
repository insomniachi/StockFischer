using System.Drawing;

namespace StockFischer;

public static class ThemeHelper
{
    public static ThemeInfo Theme { get; set; } = new ThemeInfo { Name = StockFischer.Theme.Default };
}

public class ThemeInfo
{
    public Theme Name { get; set; }
    public static Brush WhiteSquare { get; set; } = Brushes.Tan;
    public static Brush BlackSquare { get; set; } = Brushes.Brown;
}

public enum Theme
{
    Default,
}
