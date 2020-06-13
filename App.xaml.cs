using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

// ReSharper disable NotAccessedField.Local

namespace PAYMAP_BACKEND
{
    public partial class App
    {
        public const int BuildVersionCode = 1;
        public const string BuildVersionName = "200426.0";
        
        private CommandManager _commandManager;
        private DatabaseManager _databaseManager;
        private LogManager _logManager;
        private NetworkManager _networkManager;
        private ServerManager _serverManager;
        private WindowManager _windowManager;
        private CrawlManager _crawlManager;

        private void OnStartup(object sender, StartupEventArgs eventArgs)
        {
            //AppDomain.CurrentDomain.UnhandledException += (o, args) => OnGlobalException(args.ExceptionObject == null ? (Exception)args.ExceptionObject : null);
            //DispatcherUnhandledException += (o, args) => OnGlobalException(args.Exception);
            //TaskScheduler.UnobservedTaskException += (o, args) => OnGlobalException(args.Exception);
            
            _commandManager = CommandManager.GetInstance();
            _databaseManager = DatabaseManager.GetInstance();
            _logManager = LogManager.GetInstance();
            _networkManager = NetworkManager.GetInstance();
            _serverManager = ServerManager.GetInstance();
            _windowManager = WindowManager.GetInstance();
            _crawlManager = CrawlManager.GetInstance();

            WindowManager.ShowMainWindow();
            DatabaseManager.GetInstance().StartDatabase();
            ServerManager.GetInstance().StartWebServer();
            //CommandManager.StartCommandServer();
            //CommandManager.ConnectMasterServer();
        }

        public static void OnUserTerminate()
        {
            WindowManager.HideMainWindow();
            CommandManager.DisconnectMasterServer();
            CommandManager.StopCommandServer();
        }

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            OnGlobalException(args.Exception);
        }

        private static void OnGlobalException(Exception exception)
        {
            LogManager.NewLog(LogType.Application, LogLevel.Fatal, "OnGlobalException", exception);
        }
        
        public static class PaymapSettings
        {
            public static int PortMain = 9991;
            public static int PortRemote = 9981;
            public static int PortDEVX = 8888;
            public static bool AuthLevel1 = false;
            public static bool AuthLevel2 = false;
            public static bool AuthLevel3 = false;
            public static int VersionMin = 0;
            public static int VersionLive = 0;
            public static bool ThreadSwitchDashboard = false;
            public static bool ThreadSwitchDEVX = false;
            public static int ThreadIntervalDashboard = 1000;
            public static int ThreadIntervalDEVX = 1000;
            public static string APIFirebase = "API_FIREBASE";
            public static string APICatcher = "API_CATCHER";
            public static string APIReviewer = "API_REVIEWER";
            public static string DBAddress = "localhost";
            public static int DBPort = 3306;
            public static bool DEVXMaster = true;
            public static bool DEVXReport = true;
            public static bool DEVXCommand = true;
            public static bool LogLevel1 = true;
            public static bool LogLevel2 = true;
            public static bool LogLevel3 = true;
            public static bool LogLevel4 = true;
            public static bool LogLevel5 = true;
            public static bool LogFilterLevel1 = true;
            public static bool LogFilterLevel2 = true;
            public static bool LogFilterLevel3 = true;
            public static bool LogFilterLevel4 = true;
            public static bool LogFilterLevel5 = true;
            public static bool LogFilterType1 = true;
            public static bool LogFilterType2 = true;
            public static bool LogFilterType3 = true;
            public static bool LogFilterType4 = true;
            public static bool LogFilterType5 = true;
            public static bool LogFilterType6 = true;
            public static bool LogFilterType7 = true;
            public static bool LogFilterType8 = true;
        }
        
    }
}