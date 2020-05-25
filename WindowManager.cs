using System;
using System.Windows.Controls;
using PAYMAP_BACKEND.Views;
using MahApps.Metro.Controls;

namespace PAYMAP_BACKEND
{
    public partial class WindowManager
    {
        private static WindowManager _instance;
        
        public static bool IsModuleRunning = false;
        public static bool IsModuleHealthy = false;

        private WindowManager()
        {
            
        }

        public static WindowManager GetInstance()
        {
            return _instance ?? (_instance = new WindowManager());
        }

        public static void ShowMainWindow()
        {
            if (_mainWindow == null)
            {
                _mainWindow = new MainWindow();
            }
            _mainWindow.Show();
        }

        public static void HideMainWindow()
        {
            try
            {
                _mainWindow?.Close();
            }
            catch (Exception exception)
            {
                LogManager.NewLog(LogType.WindowManager, LogLevel.Error, "HideMainWindow", exception);
            }
        }

        public static void OnMainWindowLoaded()
        {
            InitializeMainWindow();
        }

        public static void OnMainWindowClosed()
        {
            App.OnUserTerminate();
        }

    }
}