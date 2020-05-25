using System;
using System.Collections.Generic;
using System.Globalization;
using PAYMAP_BACKEND.Utils;

namespace PAYMAP_BACKEND
{

    public enum LogType
    {
        Application, CommandManager, DatabaseManager, LogManager, NetworkManager, ServerManager, WindowManager, Undefined
    }
    
    public enum LogLevel
    {
        Debug, Info, Warn, Error, Fatal
    }

    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public struct Log
    {
        public string DateTime { get; set; }
        public LogType LogType { get; set; }
        public LogLevel LogLevel { get; set; }
        public string LogLocation { get; set; }
        public string LogContent { get; set; }
        public string LogDetail { get; set; }
        
    }

    public class LogManager
    {
        private static LogManager _instance;

        private static DateTime _dateTime;
        private static LinkedList<Log> _logs;

        public static int LogFilterSize = 100;
        
        public static bool IsModuleRunning = false;
        public static bool IsModuleHealthy = false;

        private LogManager()
        {
            _logs = new LinkedList<Log>();
            _dateTime = DateTime.Now;
        }

        public static LogManager GetInstance()
        {
            return _instance ?? (_instance = new LogManager());
        }

        private static bool IsAuthorizedLog(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return App.PaymapSettings.LogLevel1;
                case LogLevel.Info:
                    return App.PaymapSettings.LogLevel2;
                case LogLevel.Warn:
                    return App.PaymapSettings.LogLevel3;
                case LogLevel.Error:
                    return App.PaymapSettings.LogLevel4;
                case LogLevel.Fatal:
                    return App.PaymapSettings.LogLevel5;
                default:
                    return false;
            }
        }
        
        // ReSharper disable ConvertIfStatementToSwitchStatement
        public static bool IsFilterTargetLog(Log log)
        {
            if (log.LogLevel == LogLevel.Debug && !App.PaymapSettings.LogLevel1) return false;
            if (log.LogLevel == LogLevel.Info && !App.PaymapSettings.LogLevel2) return false;
            if (log.LogLevel == LogLevel.Warn && !App.PaymapSettings.LogLevel3) return false;
            if (log.LogLevel == LogLevel.Error && !App.PaymapSettings.LogLevel4) return false;
            if (log.LogLevel == LogLevel.Fatal && !App.PaymapSettings.LogLevel5) return false;
            if (log.LogType == LogType.Application && !App.PaymapSettings.LogFilterType1) return false;
            if (log.LogType == LogType.CommandManager && !App.PaymapSettings.LogFilterType2) return false;
            if (log.LogType == LogType.DatabaseManager && !App.PaymapSettings.LogFilterType3) return false;
            if (log.LogType == LogType.LogManager && !App.PaymapSettings.LogFilterType4) return false;
            if (log.LogType == LogType.NetworkManager && !App.PaymapSettings.LogFilterType5) return false;
            if (log.LogType == LogType.ServerManager && !App.PaymapSettings.LogFilterType6) return false;
            if (log.LogType == LogType.WindowManager && !App.PaymapSettings.LogFilterType7) return false;
            if (log.LogType == LogType.Undefined && !App.PaymapSettings.LogFilterType8) return false;
            return true;
        }
        
        private static void NewAuthorizedLog(Log newLog)
        {
            _logs.AddLast(newLog);
            WindowManager.UpdateHeaderDot(true, newLog.LogLevel == LogLevel.Warn ? 1 : 0, newLog.LogLevel == LogLevel.Error ? 1 : 0, newLog.LogLevel == LogLevel.Fatal ? 1 : 0);
            WindowManager.OnLiveLog(newLog);
        }

        public static void NewLog(LogType logType, LogLevel logLevel, string logLocation, string logContent, string logDetail = null)
        {
            if (!IsAuthorizedLog(logLevel)) return;
            _dateTime = DateTime.Now;
            Log newLog = new Log
            {
                DateTime = DateTimeFormatter.FormatDateTime(_dateTime),
                LogType = logType,
                LogLevel = logLevel,
                LogLocation = logLocation,
                LogContent = logContent,
                LogDetail = logDetail
            };
            NewAuthorizedLog(newLog);
        }
        
        public static void NewLog(LogType logType, LogLevel logLevel, string logLocation, Exception exception)
        {
            if (!IsAuthorizedLog(logLevel)) return;
            _dateTime = DateTime.Now;
            Log newLog = new Log
            {
                DateTime = DateTimeFormatter.FormatDateTime(_dateTime),
                LogType = logType,
                LogLevel = logLevel,
                LogLocation = logLocation,
                LogContent = exception?.Message ?? "NULL EXCEPTION",
                LogDetail = exception?.ToString() ?? "NULL EXCEPTION"
            };
            NewAuthorizedLog(newLog);
        }

        public static LinkedList<Log> GetLogs(bool getFilteredLogs, int logSize = -1)
        {
            if (!getFilteredLogs) return _logs;

            if (logSize == -1) logSize = LogFilterSize;
            
            LinkedList<Log> recentLogs = new LinkedList<Log>();

            foreach (Log log in _logs)
            {
                if (!IsFilterTargetLog(log)) continue;
                recentLogs.AddLast(log);
                if (recentLogs.Count >= logSize) break;
            }
            
            return recentLogs;
        }

    }
}