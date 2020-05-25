using System.ComponentModel;
using System.Windows;

namespace PAYMAP_BACKEND.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
            InitializeComponent();
        }
        
        private static void OnWindowLoaded(object sender, RoutedEventArgs args)
        {
            WindowManager.OnMainWindowLoaded();
        }
        
        private static void OnWindowClosing(object sender, CancelEventArgs args)
        {
            WindowManager.OnMainWindowClosed();
        }
    }
}