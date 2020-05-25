using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using PAYMAP_BACKEND.Views;
using MahApps.Metro.Controls;
using PAYMAP_BACKEND.Utils;

namespace PAYMAP_BACKEND
{
    partial class WindowManager
    {
        
        private static DashboardView _dashboardView;

        private static TextBlock _dashboardStatusNetwork;
        private static TextBlock _dashboardStatusDatabase;
        private static TextBlock _dashboardStatusServer;
        private static TextBlock _dashboardStatusCrawler;
        private static TextBlock _dashboardStatusCommand;
        private static TextBlock _dashboardStatusDEVX;
        private static TextBlock _dashboardRuntimeText;
        private static TextBlock _dashboardRuntimeLive;
        private static TextBlock _dashboardRuntimeTotal;
        private static TextBlock _dashboardRuntimeError;
        private static TextBlock _dashboardRuntimeLog1;
        private static TextBlock _dashboardRuntimeLog2;
        private static TextBlock _dashboardRuntimeLog3;
        private static TextBlock _dashboardRuntimeLog4;
        private static TextBlock _dashboardNetworkCount;
        private static TextBlock _dashboardNetworkBandwidth;
        private static TextBlock _dashboardDatabaseCount;
        private static TextBlock _dashboardDatabaseBandwidth;
        private static CartesianChart _dashboardGraphCPU;
        private static CartesianChart _dashboardGraphRAM;

        private static Thread _dashboardMonitor;
        private static bool _dashboardFlagMonitor;
        public static int DashboardIntervalMonitor = 1000;

        private static int _dashboardTimeTotal;
        private static int _dashboardTimeLive;
        private static int _dashboardTimeError;
        private static int _dashboardTimeTick;
        private static int _dashboardLogInfo;
        private static int _dashboardLogWarn;
        private static int _dashboardLogError;
        private static int _dashboardLogFatal;

        private static SeriesCollection _dashboardResourceCPU;
        private static SeriesCollection _dashboardResourceRAM;

        private static void InitializeDashboardWindow()
        {
            if (_dashboardView == null) return;
            _dashboardStatusNetwork = (TextBlock) _dashboardView.FindName("DashboardModuleNetworkStatus");
            _dashboardStatusDatabase = (TextBlock) _dashboardView.FindName("DashboardModuleDatabaseStatus");
            _dashboardStatusServer = (TextBlock) _dashboardView.FindName("DashboardModuleServerStatus");
            _dashboardStatusCrawler = (TextBlock) _dashboardView.FindName("DashboardModuleCrawlerStatus");
            _dashboardStatusCommand = (TextBlock) _dashboardView.FindName("DashboardModuleCommandStatus");
            _dashboardStatusDEVX = (TextBlock) _dashboardView.FindName("DashboardModuleDEVXStatus");
            if (_dashboardStatusNetwork == null || _dashboardStatusDatabase == null || _dashboardStatusServer == null || 
                _dashboardStatusCrawler == null || _dashboardStatusCommand == null || _dashboardStatusDEVX == null) return;
            _dashboardRuntimeText = (TextBlock) _dashboardView.FindName("DashboardRuntimeText");
            _dashboardRuntimeLive = (TextBlock) _dashboardView.FindName("DashboardRuntimeLive");
            _dashboardRuntimeTotal = (TextBlock) _dashboardView.FindName("DashboardRuntimeTotal");
            _dashboardRuntimeError = (TextBlock) _dashboardView.FindName("DashboardRuntimeError");
            _dashboardRuntimeLog1 = (TextBlock) _dashboardView.FindName("DashboardRuntimeLog1");
            _dashboardRuntimeLog2 = (TextBlock) _dashboardView.FindName("DashboardRuntimeLog2");
            _dashboardRuntimeLog3 = (TextBlock) _dashboardView.FindName("DashboardRuntimeLog3");
            _dashboardRuntimeLog4 = (TextBlock) _dashboardView.FindName("DashboardRuntimeLog4");
            if (_dashboardRuntimeText == null || _dashboardRuntimeLive == null || _dashboardRuntimeTotal == null || _dashboardRuntimeError == null ||
                _dashboardRuntimeLog1 == null || _dashboardRuntimeLog2 == null || _dashboardRuntimeLog3 == null || _dashboardRuntimeLog4 == null) return;
            _dashboardNetworkCount = (TextBlock) _dashboardView.FindName("DashboardNetworkCount");
            _dashboardNetworkBandwidth = (TextBlock) _dashboardView.FindName("DashboardNetworkPerSecond");
            _dashboardDatabaseCount = (TextBlock) _dashboardView.FindName("DashboardDatabaseCount");
            _dashboardDatabaseBandwidth = (TextBlock) _dashboardView.FindName("DashboardDatabasePerSecond");
            if (_dashboardNetworkCount == null || _dashboardNetworkBandwidth == null || _dashboardDatabaseCount == null || _dashboardDatabaseBandwidth == null) return;
            _dashboardGraphCPU = (CartesianChart) _dashboardView.FindName("DashboardGraphCPU");
            _dashboardGraphRAM = (CartesianChart) _dashboardView.FindName("DashboardGraphRAM");
            if (_dashboardGraphCPU == null || _dashboardGraphRAM == null) return;

            if (_dashboardResourceCPU == null)
            {
                _dashboardResourceCPU = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "CPU:SS",
                        Values = new ChartValues<int> {0, 0, 0, 0, 0},
                        LineSmoothness = 0,
                        Fill = BootStrapColors.BrushDark,
                        Stroke = BootStrapColors.BrushDark,
                    },
                    new LineSeries
                    {
                        Title = "CPU:MM",
                        Values = new ChartValues<int> {0, 0, 0, 0, 0},
                        LineSmoothness = 0,
                        Fill = BootStrapColors.BrushWarning,
                        Stroke = BootStrapColors.BrushWarning,
                    },
                    new LineSeries
                    {
                        Title = "CPU:HH",
                        Values = new ChartValues<int> {0, 0, 0, 0, 0},
                        LineSmoothness = 0,
                        Fill = BootStrapColors.BrushDanger,
                        Stroke = BootStrapColors.BrushDanger,
                    }
                };
            }
            _dashboardGraphCPU.Series = _dashboardResourceCPU;
            _dashboardGraphCPU.AxisY[0].LabelFormatter = value => value + "%";
            _dashboardGraphCPU.AxisX[0].ShowLabels = false;
            _dashboardGraphCPU.AxisX[0].Labels = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, "NOW" };

            if (_dashboardResourceRAM == null)
            {
                _dashboardResourceRAM = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "RAM:SS",
                        Values = new ChartValues<int> {0, 0, 0, 0, 0},
                        LineSmoothness = 0,
                        Fill = BootStrapColors.BrushDark,
                        Stroke = BootStrapColors.BrushDark
                    },
                    new LineSeries
                    {
                        Title = "RAM:MM",
                        Values = new ChartValues<int> {0, 0, 0, 0, 0},
                        LineSmoothness = 0,
                        Fill = BootStrapColors.BrushInfo,
                        Stroke = BootStrapColors.BrushInfo
                    },
                    new LineSeries
                    {
                        Title = "RAM:HH",
                        Values = new ChartValues<int> {0, 0, 0, 0, 0},
                        LineSmoothness = 0,
                        Fill = BootStrapColors.BrushPrimary,
                        Stroke = BootStrapColors.BrushPrimary
                    }
                };
            }
            _dashboardGraphRAM.Series = _dashboardResourceRAM;
            _dashboardGraphRAM.AxisY[0].LabelFormatter = value => value + "KB";
            _dashboardGraphRAM.AxisX[0].ShowLabels = false;
            _dashboardGraphRAM.AxisX[0].Labels = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, "NOW" };

            StartDashboardMonitor();
        }

        public static void StartDashboardMonitor()
        {
            _dashboardFlagMonitor = true;
            if (_dashboardMonitor == null)
            {
                _dashboardMonitor = new Thread(MonitorDashboard)
                {
                    Priority = ThreadPriority.Lowest
                };
            }
            if (!_dashboardMonitor.IsAlive)
            {
                _dashboardMonitor.Start();
            }
        }

        public static void StopDashboardMonitor(bool hardOff = false)
        {
            _dashboardFlagMonitor = true;
            if (!hardOff || _dashboardMonitor == null || !_dashboardMonitor.IsAlive) return;
            try
            {
                _dashboardMonitor.Interrupt();
            }
            catch (Exception exception)
            {
                LogManager.NewLog(LogType.WindowManager, LogLevel.Error, "StopDashboardMonitor", exception);
            }
        }
        
        public static void SetDashboardView(DashboardView dashboardView)
        {
            _dashboardView = dashboardView;
            InitializeDashboardWindow();
        }

        private static void MonitorDashboard()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            PerformanceCounter performanceCPUCounter = new PerformanceCounter("Process", "% Processor Time", processName);
            PerformanceCounter performanceRAMCounter = new PerformanceCounter("Process", "Working Set - Private", processName);
            
            while (true)
            {
                if (!_dashboardFlagMonitor)
                {
                    LogManager.NewLog(LogType.WindowManager, LogLevel.Info, "MonitorDashboard", "Dashboard Monitor Thread Stop : Safe");
                    break;
                }

                _dashboardTimeTick++;
                
                if (ServerManager.IsModuleRunning)
                {
                    if (ServerManager.IsModuleHealthy)
                    {
                        _dashboardTimeLive++;
                        _dashboardTimeTotal++;
                    }
                    else
                    {
                        _dashboardTimeError++;
                    }
                }

                int cpuUsage = (int) performanceCPUCounter.NextValue() / Environment.ProcessorCount;
                int memUsage = (int) performanceRAMCounter.RawValue / 1024;
                
                _dashboardResourceCPU[0].Values.RemoveAt(0);
                _dashboardResourceCPU[0].Values.Add(cpuUsage);
                _dashboardResourceRAM[0].Values.RemoveAt(0);
                _dashboardResourceRAM[0].Values.Add(memUsage);
                if (_dashboardTimeTick % 60 == 0)
                {
                    _dashboardResourceCPU[1].Values.RemoveAt(0);
                    _dashboardResourceCPU[1].Values.Add(cpuUsage);
                    _dashboardResourceRAM[1].Values.RemoveAt(0);
                    _dashboardResourceRAM[1].Values.Add(memUsage);
                }
                if (_dashboardTimeTick % 3600 == 0)
                {
                    _dashboardResourceCPU[2].Values.RemoveAt(0);
                    _dashboardResourceCPU[2].Values.Add(cpuUsage);
                    _dashboardResourceRAM[2].Values.RemoveAt(0);
                    _dashboardResourceRAM[2].Values.Add(memUsage);
                }

                if (_dashboardStatusNetwork == null || _dashboardStatusDatabase == null || _dashboardStatusServer == null || 
                    _dashboardStatusCrawler == null || _dashboardStatusCommand == null || _dashboardStatusDEVX == null) continue;
                if (_dashboardRuntimeText == null || _dashboardRuntimeLive == null || _dashboardRuntimeTotal == null || _dashboardRuntimeError == null ||
                    _dashboardRuntimeLog1 == null || _dashboardRuntimeLog2 == null || _dashboardRuntimeLog3 == null || _dashboardRuntimeLog4 == null) continue;
                if (_dashboardNetworkCount == null || _dashboardNetworkBandwidth == null || _dashboardDatabaseCount == null || _dashboardDatabaseBandwidth == null) continue;
                
                Application.Current.Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    _dashboardStatusNetwork.Text = NetworkManager.IsModuleRunning ? (NetworkManager.IsModuleHealthy ? "ON" : "ERROR") : "OFF";
                    _dashboardStatusDatabase.Text = DatabaseManager.IsModuleRunning ? (DatabaseManager.IsModuleHealthy ? "ON" : "ERROR") : "OFF";
                    _dashboardStatusServer.Text = ServerManager.IsModuleRunning ? (ServerManager.IsModuleHealthy ? "ON" : "ERROR") : "OFF";
                    _dashboardStatusCrawler.Text = CrawlManager.IsModuleRunning ? (CrawlManager.IsModuleHealthy ? "ON" : "ERROR") : "OFF";
                    _dashboardStatusCommand.Text = CommandManager.IsModuleRemoteRunning ? (CommandManager.IsModuleRemoteHealthy ? "ON" : "ERROR") : "OFF";
                    _dashboardStatusDEVX.Text = CommandManager.IsModuleDEVXRunning ? (CommandManager.IsModuleDEVXHealthy ? "ON" : "ERROR") : "OFF";
                    
                    _dashboardStatusNetwork.Foreground = NetworkManager.IsModuleRunning ? 
                        (NetworkManager.IsModuleHealthy ? BootStrapColors.BrushSuccess : BootStrapColors.BrushDanger) : BootStrapColors.BrushDark;
                    _dashboardStatusDatabase.Foreground = DatabaseManager.IsModuleRunning ? 
                        (DatabaseManager.IsModuleHealthy ? BootStrapColors.BrushSuccess : BootStrapColors.BrushDanger) : BootStrapColors.BrushDark;
                    _dashboardStatusServer.Foreground = ServerManager.IsModuleRunning ? 
                        (ServerManager.IsModuleHealthy ? BootStrapColors.BrushSuccess : BootStrapColors.BrushDanger) : BootStrapColors.BrushDark;
                    _dashboardStatusCrawler.Foreground = WindowManager.IsModuleRunning ? 
                        (WindowManager.IsModuleHealthy ? BootStrapColors.BrushSuccess : BootStrapColors.BrushDanger) : BootStrapColors.BrushDark;
                    _dashboardStatusCommand.Foreground = CommandManager.IsModuleRemoteRunning ? 
                        (CommandManager.IsModuleRemoteRunning ? BootStrapColors.BrushSuccess : BootStrapColors.BrushDanger) : BootStrapColors.BrushDark;
                    _dashboardStatusDEVX.Foreground = CommandManager.IsModuleDEVXRunning ? 
                        (CommandManager.IsModuleDEVXRunning ? BootStrapColors.BrushSuccess : BootStrapColors.BrushDanger) : BootStrapColors.BrushDark;
                    
                    _dashboardRuntimeLive.Text = $"{_dashboardTimeLive/3600:D4}:{_dashboardTimeLive%3600/60:D2}:{_dashboardTimeLive%60:D2}";
                    _dashboardRuntimeTotal.Text = $"{_dashboardTimeTotal/3600:D4}:{_dashboardTimeTotal%3600/60:D2}:{_dashboardTimeTotal%60:D2}";
                    _dashboardRuntimeError.Text = $"{_dashboardTimeError/3600:D4}:{_dashboardTimeError%3600/60:D2}:{_dashboardTimeError%60:D2}";
                    
                    _dashboardRuntimeLog1.Text = _dashboardLogInfo.ToString();
                    _dashboardRuntimeLog2.Text = _dashboardLogWarn.ToString();
                    _dashboardRuntimeLog3.Text = _dashboardLogError.ToString();
                    _dashboardRuntimeLog4.Text = _dashboardLogFatal.ToString();

                    _dashboardNetworkCount.Text = NetworkManager.CountNetworkAPI.ToString();
                    _dashboardNetworkBandwidth.Text = _dashboardTimeLive != 0 ? $"{(float)NetworkManager.CountNetworkAPI/(float)_dashboardTimeLive:F4}" : "0.0000";
                    _dashboardDatabaseCount.Text = DatabaseManager.CountDatabaseAPI.ToString();
                    _dashboardDatabaseBandwidth.Text = _dashboardTimeLive != 0 ? $"{(float)DatabaseManager.CountDatabaseAPI/(float)_dashboardTimeLive:F4}" : "0.0000";

                }));

                Thread.Sleep(DashboardIntervalMonitor);
            }
        }
        
        public static void UpdateRuntimeLogCount(bool increaseCount, int infoCount, int warnCount, int errorCount, int fatalCount)
        {
            if (!ServerManager.IsModuleRunning) return;
            _dashboardLogInfo = increaseCount ? _dashboardLogInfo + warnCount : warnCount;
            _dashboardLogWarn = increaseCount ? _dashboardLogWarn + errorCount : errorCount;
            _dashboardLogError = increaseCount ? _dashboardLogError + fatalCount : fatalCount;
            _dashboardLogFatal = increaseCount ? _dashboardLogFatal + fatalCount : fatalCount;
        }
        
    }
}