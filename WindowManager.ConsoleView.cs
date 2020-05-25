using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using PAYMAP_BACKEND.Views;
using MahApps.Metro.Controls;

namespace PAYMAP_BACKEND
{
    partial class WindowManager
    {
        private static ConsoleView _consoleView;
        
        private static ToggleSwitch _consoleSwitch1;
        private static ToggleSwitch _consoleSwitch2;
        private static ToggleSwitch _consoleSwitch3;
        private static ToggleSwitch _consoleSwitch4;
        private static ToggleSwitch _consoleSwitch5;
        private static CheckBox _consoleFilterLevel1;
        private static CheckBox _consoleFilterLevel2;
        private static CheckBox _consoleFilterLevel3;
        private static CheckBox _consoleFilterLevel4;
        private static CheckBox _consoleFilterLevel5;
        private static CheckBox _consoleFilterType1;
        private static CheckBox _consoleFilterType2;
        private static CheckBox _consoleFilterType3;
        private static CheckBox _consoleFilterType4;
        private static CheckBox _consoleFilterType5;
        private static CheckBox _consoleFilterType6;
        private static CheckBox _consoleFilterType7;
        private static CheckBox _consoleFilterType8;
        private static DataGrid _consoleLogView;
        private static TextBox _consoleEditor;
        private static ToggleButton _consoleToggleLive;
        private static Button _consoleButtonRefresh;
        private static Button _consoleButtonClear;

        private static LinkedList<Log> _lastFilteredLogs;
        public static bool IsListeningLiveLog;
        
        private static void InitializeConsoleWindow()
        {
            if (_consoleView == null) return;
            _consoleSwitch1 = (ToggleSwitch) _consoleView.FindName("ConsoleSwitchLevel1");
            _consoleSwitch2 = (ToggleSwitch) _consoleView.FindName("ConsoleSwitchLevel2");
            _consoleSwitch3 = (ToggleSwitch) _consoleView.FindName("ConsoleSwitchLevel3");
            _consoleSwitch4 = (ToggleSwitch) _consoleView.FindName("ConsoleSwitchLevel4");
            _consoleSwitch5 = (ToggleSwitch) _consoleView.FindName("ConsoleSwitchLevel5");
            if (_consoleSwitch1 == null || _consoleSwitch2 == null || _consoleSwitch3 == null || _consoleSwitch4 == null || _consoleSwitch5 == null) return;
            _consoleLogView = (DataGrid) _consoleView.FindName("ConsoleLogView");
            if (_consoleLogView == null) return;
            _consoleFilterLevel1 = (CheckBox) _consoleView.FindName("ConsoleCheckLevel1");
            _consoleFilterLevel2 = (CheckBox) _consoleView.FindName("ConsoleCheckLevel2");
            _consoleFilterLevel3 = (CheckBox) _consoleView.FindName("ConsoleCheckLevel3");
            _consoleFilterLevel4 = (CheckBox) _consoleView.FindName("ConsoleCheckLevel4");
            _consoleFilterLevel5 = (CheckBox) _consoleView.FindName("ConsoleCheckLevel5");
            if (_consoleFilterLevel1 == null || _consoleFilterLevel2 == null || _consoleFilterLevel3 == null || _consoleFilterLevel4 == null || _consoleFilterLevel5 == null) return;
            _consoleFilterType1 = (CheckBox) _consoleView.FindName("ConsoleCheckModule1");
            _consoleFilterType2 = (CheckBox) _consoleView.FindName("ConsoleCheckModule2");
            _consoleFilterType3 = (CheckBox) _consoleView.FindName("ConsoleCheckModule3");
            _consoleFilterType4 = (CheckBox) _consoleView.FindName("ConsoleCheckModule4");
            _consoleFilterType5 = (CheckBox) _consoleView.FindName("ConsoleCheckModule5");
            _consoleFilterType6 = (CheckBox) _consoleView.FindName("ConsoleCheckModule6");
            _consoleFilterType7 = (CheckBox) _consoleView.FindName("ConsoleCheckModule7");
            _consoleFilterType8 = (CheckBox) _consoleView.FindName("ConsoleCheckModule8");
            if (_consoleFilterType1 == null || _consoleFilterType2 == null || _consoleFilterType3 == null || _consoleFilterType4 == null || 
                _consoleFilterType5 == null || _consoleFilterType6 == null || _consoleFilterType7 == null || _consoleFilterType8 == null) return;
            _consoleEditor = (TextBox) _consoleView.FindName("ConsoleCommandEditor");
            _consoleToggleLive = (ToggleButton) _consoleView.FindName("ConsoleLogLive");
            _consoleButtonRefresh = (Button) _consoleView.FindName("ConsoleLogRefresh");
            _consoleButtonClear = (Button) _consoleView.FindName("ConsoleLogClear");
            if (_consoleEditor == null || _consoleToggleLive == null || _consoleButtonRefresh == null || _consoleButtonClear == null) return;
            
            _consoleSwitch1.IsChecked = App.PaymapSettings.LogLevel1;
            _consoleSwitch2.IsChecked = App.PaymapSettings.LogLevel2;
            _consoleSwitch3.IsChecked = App.PaymapSettings.LogLevel3;
            _consoleSwitch4.IsChecked = App.PaymapSettings.LogLevel4;
            _consoleSwitch5.IsChecked = App.PaymapSettings.LogLevel5;
            _consoleFilterLevel1.IsChecked = App.PaymapSettings.LogFilterLevel1;
            _consoleFilterLevel2.IsChecked = App.PaymapSettings.LogFilterLevel2;
            _consoleFilterLevel3.IsChecked = App.PaymapSettings.LogFilterLevel3;
            _consoleFilterLevel4.IsChecked = App.PaymapSettings.LogFilterLevel4;
            _consoleFilterLevel5.IsChecked = App.PaymapSettings.LogFilterLevel5;
            _consoleFilterType1.IsChecked = App.PaymapSettings.LogFilterType1;
            _consoleFilterType2.IsChecked = App.PaymapSettings.LogFilterType2;
            _consoleFilterType3.IsChecked = App.PaymapSettings.LogFilterType3;
            _consoleFilterType4.IsChecked = App.PaymapSettings.LogFilterType4;
            _consoleFilterType5.IsChecked = App.PaymapSettings.LogFilterType5;
            _consoleFilterType6.IsChecked = App.PaymapSettings.LogFilterType6;
            _consoleFilterType7.IsChecked = App.PaymapSettings.LogFilterType7;
            _consoleFilterType8.IsChecked = App.PaymapSettings.LogFilterType8;
            _consoleToggleLive.IsChecked = IsListeningLiveLog;

            _consoleSwitch1.IsCheckedChanged += (sender, args) => App.PaymapSettings.LogLevel1 = _consoleSwitch1.IsChecked.Value;
            _consoleSwitch2.IsCheckedChanged += (sender, args) => App.PaymapSettings.LogLevel2 = _consoleSwitch2.IsChecked.Value;
            _consoleSwitch3.IsCheckedChanged += (sender, args) => App.PaymapSettings.LogLevel3 = _consoleSwitch3.IsChecked.Value;
            _consoleSwitch4.IsCheckedChanged += (sender, args) => App.PaymapSettings.LogLevel4 = _consoleSwitch4.IsChecked.Value;
            _consoleSwitch5.IsCheckedChanged += (sender, args) => App.PaymapSettings.LogLevel5 = _consoleSwitch5.IsChecked.Value;
            _consoleFilterLevel1.Checked += (sender, args) => App.PaymapSettings.LogFilterLevel1 = true;
            _consoleFilterLevel1.Unchecked += (sender, args) => App.PaymapSettings.LogFilterLevel1 = false;
            _consoleFilterLevel2.Checked += (sender, args) => App.PaymapSettings.LogFilterLevel2 = true;
            _consoleFilterLevel2.Unchecked += (sender, args) => App.PaymapSettings.LogFilterLevel2 = false;
            _consoleFilterLevel3.Checked += (sender, args) => App.PaymapSettings.LogFilterLevel3 = true;
            _consoleFilterLevel3.Unchecked += (sender, args) => App.PaymapSettings.LogFilterLevel3 = false;
            _consoleFilterLevel4.Checked += (sender, args) => App.PaymapSettings.LogFilterLevel4 = true;
            _consoleFilterLevel4.Unchecked += (sender, args) => App.PaymapSettings.LogFilterLevel4 = false;
            _consoleFilterLevel5.Checked += (sender, args) => App.PaymapSettings.LogFilterLevel5 = true;
            _consoleFilterLevel5.Unchecked += (sender, args) => App.PaymapSettings.LogFilterLevel5 = false;
            _consoleFilterType1.Checked += (sender, args) => App.PaymapSettings.LogFilterType1 = true;
            _consoleFilterType1.Unchecked += (sender, args) => App.PaymapSettings.LogFilterType1 = false;
            _consoleFilterType2.Checked += (sender, args) => App.PaymapSettings.LogFilterType2 = true;
            _consoleFilterType2.Unchecked += (sender, args) => App.PaymapSettings.LogFilterType2 = false;
            _consoleFilterType3.Checked += (sender, args) => App.PaymapSettings.LogFilterType3 = true;
            _consoleFilterType3.Unchecked += (sender, args) => App.PaymapSettings.LogFilterType3 = false;
            _consoleFilterType4.Checked += (sender, args) => App.PaymapSettings.LogFilterType4 = true;
            _consoleFilterType4.Unchecked += (sender, args) => App.PaymapSettings.LogFilterType4 = false;
            _consoleFilterType5.Checked += (sender, args) => App.PaymapSettings.LogFilterType5 = true;
            _consoleFilterType5.Unchecked += (sender, args) => App.PaymapSettings.LogFilterType5 = false;
            _consoleFilterType6.Checked += (sender, args) => App.PaymapSettings.LogFilterType6 = true;
            _consoleFilterType6.Unchecked += (sender, args) => App.PaymapSettings.LogFilterType6 = false;
            _consoleFilterType7.Checked += (sender, args) => App.PaymapSettings.LogFilterType7 = true;
            _consoleFilterType7.Unchecked += (sender, args) => App.PaymapSettings.LogFilterType7 = false;
            _consoleFilterType8.Checked += (sender, args) => App.PaymapSettings.LogFilterType8 = true;
            _consoleFilterType8.Unchecked += (sender, args) => App.PaymapSettings.LogFilterType8 = false;

            _consoleEditor.KeyDown += OnCommandEnter;
            _consoleToggleLive.Checked += (sender, args) => IsListeningLiveLog = true;
            _consoleToggleLive.Unchecked += (sender, args) => IsListeningLiveLog = false;
            _consoleButtonRefresh.Click += (sender, args) => RefreshLogView();
            _consoleButtonClear.Click += (sender, args) => ClearLogView();
            
            RefreshLogView();
        }

        private static void OnCommandEnter(object sender, EventArgs args)
        {
            if (_consoleEditor == null || _consoleEditor.Text.Trim().Length == 0) return;
            if (args is KeyEventArgs eventArgs && eventArgs.Key == Key.Enter)
            {
                string inputCommand = _consoleEditor.Text.Trim();
                _consoleEditor.Clear();
                CommandManager.OnServerCommand(inputCommand);
            }
        }

        public static void ClearLogView()
        {
            if (_consoleLogView == null) return;
            _lastFilteredLogs.Clear();
            _consoleLogView.Items.Refresh();
        }

        public static void RefreshLogView()
        {
            if (_consoleLogView == null) return;
            _lastFilteredLogs = LogManager.GetLogs(true);
            _consoleLogView.ItemsSource = _lastFilteredLogs;
            _consoleLogView.ScrollIntoView(_lastFilteredLogs.Count);
        }

        public static void OnLiveLog(Log log)
        {
            if (!IsListeningLiveLog) return;
            if (!LogManager.IsFilterTargetLog(log)) return;
            if (_lastFilteredLogs == null || _consoleLogView == null) return;
            _lastFilteredLogs.AddLast(log);
            _consoleLogView.Items.Refresh();
            _consoleLogView.ScrollIntoView(_lastFilteredLogs.Count);
        }
    }
}