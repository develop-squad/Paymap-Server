using System;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PAYMAP_BACKEND
{
    public class CommandManager
    {
        private static CommandManager _instance;

        private static Thread _monitorMasterThread;
        private static bool _flagMonitorThread;
        public static int IntervalMonitorThread = 1000;

        private static  WebSocket _masterWebSocket;
        private static  WebSocketServer _commandWebSocketServer;
        
        public static bool IsModuleRemoteRunning = false;
        public static bool IsModuleRemoteHealthy = false;
        
        public static bool IsModuleDEVXRunning = false;
        public static bool IsModuleDEVXHealthy = false;

        private CommandManager()
        {
            
        }

        public static CommandManager GetInstance()
        {
            return _instance ?? (_instance = new CommandManager());
        }

        public static void StartCommandServer()
        {
            if (_commandWebSocketServer == null)
            {
                _commandWebSocketServer = new WebSocketServer(9981);
                _commandWebSocketServer.AddWebSocketService<RemoteCommand>("/RemoteCommand");
            }
            _commandWebSocketServer.Start();
        }

        public static void StopCommandServer()
        {
            _commandWebSocketServer?.Stop();
        }

        public static void ConnectMasterServer()
        {
            if (_masterWebSocket == null)
            {
                _masterWebSocket = new WebSocket("ws://localhost:8888/PHOENIXListener");
            }
            _masterWebSocket.Connect();
            
            _flagMonitorThread = true;
            if (_monitorMasterThread == null)
            {
                _monitorMasterThread = new Thread(MonitorMasterServer)
                {
                    Priority = ThreadPriority.Lowest
                };
            }
            if (!_monitorMasterThread.IsAlive)
            {
                _monitorMasterThread.Start();
            }
            
        }

        public static void DisconnectMasterServer(bool hardOff = false)
        {
            _masterWebSocket?.Close();
            
            _flagMonitorThread = true;
            if (!hardOff || _monitorMasterThread == null || !_monitorMasterThread.IsAlive) return;
            try
            {
                _monitorMasterThread.Interrupt();
            }
            catch (Exception exception)
            {
                LogManager.NewLog(LogType.CommandManager, LogLevel.Error, "DisconnectMasterServer", exception);
            }
        }

        private static void MonitorMasterServer()
        {
            while (true)
            {
                if (!_flagMonitorThread)
                {
                    LogManager.NewLog(LogType.CommandManager, LogLevel.Info, "MonitorMasterServer", "Master Monitor Thread Stop : Safe");
                    break;
                }

                if (_masterWebSocket == null)
                {
                    LogManager.NewLog(LogType.CommandManager, LogLevel.Error, "MonitorMasterServer", "Master Monitor Thread Stop : _masterWebSocket null");
                    break;
                }

                if (!_masterWebSocket.IsAlive)
                {
                    LogManager.NewLog(LogType.CommandManager, LogLevel.Info, "MonitorMasterServer", "Master Monitor Thread : trying to reconnect");
                    _masterWebSocket.Connect();
                }
                
                Thread.Sleep(IntervalMonitorThread);
            }
        }

        public static void OnServerCommand(string command)
        {
            LogManager.NewLog(LogType.CommandManager, LogLevel.Info, "OnServerCommand", command);
        }
        
        private class RemoteCommand : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                var commandRaw = e.Data;
                try
                {
                    LogManager.NewLog(LogType.CommandManager, LogLevel.Warn, "RemoteCommand : OnMessage", commandRaw);
                }
                catch (Exception exception)
                {
                    LogManager.NewLog(LogType.CommandManager, LogLevel.Error, "RemoteCommand : OnMessage", exception);
                }
                
            }
        }
    }
}