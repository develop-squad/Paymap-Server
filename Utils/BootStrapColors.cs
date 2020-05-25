using System.Windows.Media;

namespace PAYMAP_BACKEND.Utils
{
    public class BootStrapColors
    {
        private BootStrapColors()
        {
            
        }
        
        public static Color ColorPrimary => Color.FromRgb(0,123,255);
        public static Color ColorSuccess => Color.FromRgb(40, 167, 69);
        public static Color ColorDanger => Color.FromRgb(220, 53, 69);
        public static Color ColorWarning => Color.FromRgb(255,193,7);
        public static Color ColorInfo => Color.FromRgb(23, 162, 184);
        public static Color ColorDark => Color.FromRgb(52, 58, 64);
        public static Color ColorLight => Color.FromRgb(248,249,250);
        
        public static Brush BrushPrimary => new SolidColorBrush(ColorPrimary);
        public static Brush BrushSuccess => new SolidColorBrush(ColorSuccess);
        public static Brush BrushDanger => new SolidColorBrush(ColorDanger);
        public static Brush BrushWarning => new SolidColorBrush(ColorWarning);
        public static Brush BrushInfo => new SolidColorBrush(ColorInfo);
        public static Brush BrushDark => new SolidColorBrush(ColorDark);
        public static Brush BrushLight => new SolidColorBrush(ColorLight);
    }
}