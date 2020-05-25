using System.Windows.Controls;
using PAYMAP_BACKEND.Views;
using MahApps.Metro.Controls;

namespace PAYMAP_BACKEND
{
    partial class WindowManager
    {
        private static MainWindow _mainWindow;
        
        private static Button _headerButtonDEVX;
        private static StackPanel _headerDotWarn;
        private static StackPanel _headerDotError;
        private static StackPanel _headerDotFatal;
        private static TextBlock _headerDotWarnText;
        private static TextBlock _headerDotErrorText;
        private static TextBlock _headerDotFatalText;
        private static Image _headerIconConnect;

        private static HamburgerMenu _mainMenu;

        private static int _headerDotWarnCount;
        private static int _headerDotErrorCount;
        private static int _headerDotFatalCount;
        
        private static void InitializeMainWindow()
        {
            _headerDotWarn = (StackPanel) _mainWindow.FindName("HeaderDotWarn");
            _headerDotError = (StackPanel) _mainWindow.FindName("HeaderDotError");
            _headerDotFatal = (StackPanel) _mainWindow.FindName("HeaderDotFatal");
            _headerDotWarnText = (TextBlock) _mainWindow.FindName("HeaderDotWarnCount");
            _headerDotErrorText = (TextBlock) _mainWindow.FindName("HeaderDotErrorCount");
            _headerDotFatalText = (TextBlock) _mainWindow.FindName("HeaderDotFatalCount");
            _headerButtonDEVX = (Button) _mainWindow.FindName("HeaderButtonDEVX");
            _headerIconConnect = (Image) _mainWindow.FindName("HeaderIconConnect");
            _mainMenu = (HamburgerMenu) _mainWindow.FindName("MainMenu");
            if (_headerDotWarn == null || _headerDotError == null || _headerDotFatal == null ||
                _headerDotWarnText == null || _headerDotErrorText == null || _headerDotFatalText == null) return;
            if (_headerButtonDEVX == null || _headerIconConnect == null || _mainMenu == null) return;

            _headerDotWarn.MouseLeftButtonDown += (sender, args) =>
            {
                App.PaymapSettings.LogFilterLevel1 = false;
                App.PaymapSettings.LogFilterLevel2 = false;
                App.PaymapSettings.LogFilterLevel3 = true;
                App.PaymapSettings.LogFilterLevel4 = false;
                App.PaymapSettings.LogFilterLevel5 = false;
                _mainMenu.SelectedItem = MainViewModel.MainMenuItemConsole;
                InitializeConsoleWindow();
                UpdateHeaderDot(false, 0, _headerDotErrorCount, _headerDotFatalCount);
            };
            _headerDotError.MouseLeftButtonDown += (sender, args) =>
            {
                App.PaymapSettings.LogFilterLevel1 = false;
                App.PaymapSettings.LogFilterLevel2 = false;
                App.PaymapSettings.LogFilterLevel3 = false;
                App.PaymapSettings.LogFilterLevel4 = true;
                App.PaymapSettings.LogFilterLevel5 = false;
                _mainMenu.SelectedItem = MainViewModel.MainMenuItemConsole;
                InitializeConsoleWindow();
                UpdateHeaderDot(false, _headerDotWarnCount, 0, _headerDotFatalCount);
            };
            _headerDotFatal.MouseLeftButtonDown += (sender, args) =>
            {
                App.PaymapSettings.LogFilterLevel1 = false;
                App.PaymapSettings.LogFilterLevel2 = false;
                App.PaymapSettings.LogFilterLevel3 = false;
                App.PaymapSettings.LogFilterLevel4 = false;
                App.PaymapSettings.LogFilterLevel5 = true;
                _mainMenu.SelectedItem = MainViewModel.MainMenuItemConsole;
                InitializeConsoleWindow();
                UpdateHeaderDot(false, _headerDotWarnCount, _headerDotErrorCount, 0);
            };
            _headerButtonDEVX.Click += (sender, args) => System.Diagnostics.Process.Start("https://devx.kr"); 
        }

        public static void UpdateHeaderDot(bool increaseCount, int warnCount, int errorCount, int fatalCount)
        {
            _headerDotWarnCount = increaseCount ? _headerDotWarnCount + warnCount : warnCount;
            _headerDotErrorCount = increaseCount ? _headerDotErrorCount + errorCount : errorCount;
            _headerDotFatalCount = increaseCount ? _headerDotFatalCount + fatalCount : fatalCount;
            _headerDotWarnText.Text = _headerDotWarnCount.ToString();
            _headerDotErrorText.Text = _headerDotErrorCount.ToString();
            _headerDotFatalText.Text = _headerDotFatalCount.ToString();
        }
    }
}