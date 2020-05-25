﻿using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using PAYMAP_BACKEND.Views;
using MahApps.Metro.Controls;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace PAYMAP_BACKEND
{
    partial class WindowManager
    {
        private static SettingView _settingView;

        private static Button _settingFileLoad;
        private static Button _settingFileSave;
        private static Button _settingFileImport;
        private static Button _settingFileExport;
        private static TextBox _settingPortMain;
        private static TextBox _settingPortRemote;
        private static TextBox _settingPortDEVX;
        private static ToggleSwitch _settingAuthLevel1;
        private static ToggleSwitch _settingAuthLevel2;
        private static ToggleSwitch _settingAuthLevel3;
        private static TextBox _settingVersionLive;
        private static TextBox _settingVersionMin;
        private static ToggleSwitch _settingThreadDashboard;
        private static TextBox _settingThreadDashboardInterval;
        private static ToggleSwitch _settingThreadDEVX;
        private static TextBox _settingThreadDEVXInterval;
        private static TextBox _settingAPIFirebase;
        private static TextBox _settingAPICatcher;
        private static TextBox _settingAPIReviewer;
        private static TextBox _settingDBAddress;
        private static TextBox _settingDBPort;
        private static ToggleSwitch _settingDEVXMaster;
        private static ToggleSwitch _settingDEVXReport;
        private static ToggleSwitch _settingDEVXCommand;
        private static Button _settingControlRestore;
        private static Button _settingControlApply;

        public static string SettingDefaultFilePath = AppDomain.CurrentDomain.BaseDirectory + "config.ini";
        
        private static void InitializeSettingWindow()
        {
            if (_settingView == null) return;
            _settingFileLoad = (Button) _settingView.FindName("SettingFileLoad");
            _settingFileSave = (Button) _settingView.FindName("SettingFileSave");
            _settingFileImport = (Button) _settingView.FindName("SettingFileImport");
            _settingFileExport = (Button) _settingView.FindName("SettingFileExport");
            if (_settingFileLoad == null || _settingFileSave == null || _settingFileImport == null || _settingFileExport == null) return;
            _settingPortMain = (TextBox) _settingView.FindName("SettingPortMain");
            _settingPortRemote = (TextBox) _settingView.FindName("SettingPortRemote");
            _settingPortDEVX = (TextBox) _settingView.FindName("SettingPortDEVX");
            if (_settingPortMain == null || _settingPortRemote == null || _settingPortDEVX == null) return;
            _settingAuthLevel1 = (ToggleSwitch) _settingView.FindName("SettingAuthLevel1");
            _settingAuthLevel2 = (ToggleSwitch) _settingView.FindName("SettingAuthLevel2");
            _settingAuthLevel3 = (ToggleSwitch) _settingView.FindName("SettingAuthLevel3");
            if (_settingAuthLevel1 == null || _settingAuthLevel2 == null || _settingAuthLevel3 == null) return;
            _settingVersionLive = (TextBox) _settingView.FindName("SettingVersionLive");
            _settingVersionMin = (TextBox) _settingView.FindName("SettingVersionMin");
            if (_settingVersionLive == null || _settingVersionMin == null) return;
            _settingThreadDashboard = (ToggleSwitch) _settingView.FindName("SettingThreadDashboard");
            _settingThreadDashboardInterval = (TextBox) _settingView.FindName("SettingThreadDashboardInterval");
            _settingThreadDEVX = (ToggleSwitch) _settingView.FindName("SettingThreadDEVX");
            _settingThreadDEVXInterval = (TextBox) _settingView.FindName("SettingThreadDEVXInterval");
            if (_settingThreadDashboard == null || _settingThreadDashboardInterval == null || _settingThreadDEVX == null || _settingThreadDEVXInterval == null) return;
            _settingAPIFirebase = (TextBox) _settingView.FindName("SettingAPIFirebase");
            _settingAPICatcher = (TextBox) _settingView.FindName("SettingAPICatcher");
            _settingAPIReviewer = (TextBox) _settingView.FindName("SettingAPIReviewer");
            if (_settingAPIFirebase == null || _settingAPICatcher == null || _settingAPIReviewer == null) return;
            _settingDBAddress = (TextBox) _settingView.FindName("SettingDBAddress");
            _settingDBPort = (TextBox) _settingView.FindName("SettingDBPort");
            if (_settingDBAddress == null || _settingDBPort == null) return;
            _settingDEVXMaster = (ToggleSwitch) _settingView.FindName("SettingDEVXMaster");
            _settingDEVXReport = (ToggleSwitch) _settingView.FindName("SettingDEVXReport");
            _settingDEVXCommand = (ToggleSwitch) _settingView.FindName("SettingDEVXCommand");
            if (_settingDEVXMaster == null || _settingDEVXReport == null || _settingDEVXCommand == null) return;
            _settingControlRestore = (Button) _settingView.FindName("SettingControlRestore");
            _settingControlApply = (Button) _settingView.FindName("SettingControlApply");
            if (_settingControlRestore == null || _settingControlApply == null) return;

            _settingFileLoad.Click += (sender, args) => LoadSettingFile(SettingDefaultFilePath);
            _settingFileSave.Click += (sender, args) => SaveSettingFile(SettingDefaultFilePath);
            _settingFileImport.Click += (sender, args) => LoadSettingFile(null);
            _settingFileExport.Click += (sender, args) => SaveSettingFile(null);
            _settingControlRestore.Click += (sender, args) => RefreshSettingData();
            _settingControlApply.Click += (sender, args) => ApplySettingData();
            
            RefreshSettingData();
        }

        private static void RefreshSettingData()
        {
            if (_settingPortMain != null) _settingPortMain.Text = App.PaymapSettings.PortMain.ToString();
            if (_settingPortRemote != null) _settingPortRemote.Text = App.PaymapSettings.PortRemote.ToString();
            if (_settingPortDEVX != null) _settingPortDEVX.Text = App.PaymapSettings.PortDEVX.ToString();
            if (_settingAuthLevel1 != null) _settingAuthLevel1.IsChecked = App.PaymapSettings.AuthLevel1;
            if (_settingAuthLevel2 != null) _settingAuthLevel2.IsChecked = App.PaymapSettings.AuthLevel2;
            if (_settingAuthLevel3 != null) _settingAuthLevel3.IsChecked = App.PaymapSettings.AuthLevel3;
            if (_settingVersionLive != null) _settingVersionLive.Text = App.PaymapSettings.VersionLive.ToString();
            if (_settingVersionMin != null) _settingVersionMin.Text = App.PaymapSettings.VersionMin.ToString();
            if (_settingThreadDashboard != null) _settingThreadDashboard.IsChecked = App.PaymapSettings.ThreadSwitchDashboard;
            if (_settingThreadDashboardInterval != null) _settingThreadDashboardInterval.Text = App.PaymapSettings.ThreadIntervalDashboard.ToString();
            if (_settingThreadDEVX != null) _settingThreadDEVX.IsChecked = App.PaymapSettings.ThreadSwitchDEVX;
            if (_settingThreadDEVXInterval != null) _settingThreadDEVXInterval.Text = App.PaymapSettings.ThreadIntervalDEVX.ToString();
            if (_settingAPIFirebase != null) _settingAPIFirebase.Text = App.PaymapSettings.APIFirebase;
            if (_settingAPICatcher != null) _settingAPICatcher.Text = App.PaymapSettings.APICatcher;
            if (_settingAPIReviewer != null) _settingAPIReviewer.Text = App.PaymapSettings.APIReviewer;
            if (_settingDBAddress != null) _settingDBAddress.Text = App.PaymapSettings.DBAddress;
            if (_settingDBPort != null) _settingDBPort.Text = App.PaymapSettings.DBPort.ToString();
            if (_settingDEVXMaster != null) _settingDEVXMaster.IsChecked = App.PaymapSettings.DEVXMaster;
            if (_settingDEVXReport != null) _settingDEVXReport.IsChecked = App.PaymapSettings.DEVXReport;
            if (_settingDEVXCommand != null) _settingDEVXCommand.IsChecked = App.PaymapSettings.DEVXCommand;
        }

        private static void ApplySettingData()
        {
            if (_settingPortMain != null) int.TryParse(_settingPortMain.Text, out App.PaymapSettings.PortMain);
            if (_settingPortRemote != null) int.TryParse(_settingPortRemote.Text, out App.PaymapSettings.PortRemote);
            if (_settingPortDEVX != null) int.TryParse(_settingPortDEVX.Text, out App.PaymapSettings.PortDEVX);
            if (_settingAuthLevel1 != null) App.PaymapSettings.AuthLevel1 = _settingAuthLevel1.IsChecked.HasValue && _settingAuthLevel1.IsChecked.Value;
            if (_settingAuthLevel2 != null) App.PaymapSettings.AuthLevel2 = _settingAuthLevel2.IsChecked.HasValue && _settingAuthLevel2.IsChecked.Value;
            if (_settingAuthLevel3 != null) App.PaymapSettings.AuthLevel3 = _settingAuthLevel3.IsChecked.HasValue && _settingAuthLevel3.IsChecked.Value;
            if (_settingVersionLive != null) int.TryParse(_settingVersionLive.Text, out App.PaymapSettings.VersionLive);
            if (_settingVersionMin != null) int.TryParse(_settingVersionMin.Text, out App.PaymapSettings.VersionMin);
            if (_settingThreadDashboard != null) App.PaymapSettings.ThreadSwitchDashboard = _settingThreadDashboard.IsChecked.HasValue && _settingThreadDashboard.IsChecked.Value;
            if (_settingThreadDashboardInterval != null) int.TryParse(_settingThreadDashboardInterval.Text, out App.PaymapSettings.ThreadIntervalDashboard);
            if (_settingThreadDEVX != null) App.PaymapSettings.ThreadSwitchDEVX = _settingThreadDEVX.IsChecked.HasValue && _settingThreadDEVX.IsChecked.Value;
            if (_settingThreadDEVXInterval != null) int.TryParse(_settingThreadDEVXInterval.Text, out App.PaymapSettings.ThreadIntervalDEVX);
            if (_settingAPIFirebase != null) App.PaymapSettings.APIFirebase = _settingAPIFirebase.Text;
            if (_settingAPICatcher != null) App.PaymapSettings.APICatcher = _settingAPICatcher.Text;
            if (_settingAPIReviewer != null) App.PaymapSettings.APIReviewer = _settingAPIReviewer.Text;
            if (_settingDBAddress != null) App.PaymapSettings.DBAddress = _settingDBAddress.Text;
            if (_settingDBPort != null) int.TryParse(_settingDBPort.Text, out App.PaymapSettings.DBPort);
            if (_settingDEVXMaster != null) App.PaymapSettings.DEVXMaster = _settingDEVXMaster.IsChecked.HasValue && _settingDEVXMaster.IsChecked.Value;
            if (_settingDEVXReport != null) App.PaymapSettings.DEVXReport = _settingDEVXReport.IsChecked.HasValue && _settingDEVXReport.IsChecked.Value;
            if (_settingDEVXCommand != null) App.PaymapSettings.DEVXCommand = _settingDEVXCommand.IsChecked.HasValue && _settingDEVXCommand.IsChecked.Value;
        }
        
        public static void SetSettingView(SettingView settingView)
        {
            _settingView = settingView;
            InitializeSettingWindow();
        }
        
        private static void LoadSettingFile(string filePath)
        {
            if (filePath == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Setting File|config.ini", 
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    LoadSettingFile(openFileDialog.FileName);
                    return;
                }
            }
            var tempPortMain = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionPort, PaymapSettingKeys.IntPortMain);
            if (!string.IsNullOrEmpty(tempPortMain)) int.TryParse(tempPortMain, out App.PaymapSettings.PortMain);
            var tempPortRemote = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionPort, PaymapSettingKeys.IntPortRemote);
            if (!string.IsNullOrEmpty(tempPortRemote)) int.TryParse(tempPortRemote, out App.PaymapSettings.PortRemote);
            var tempPortDEVX = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionPort, PaymapSettingKeys.IntPortDEVX);
            if (!string.IsNullOrEmpty(tempPortDEVX)) int.TryParse(tempPortDEVX, out App.PaymapSettings.PortDEVX);
            var tempAuthLevel1 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionAuth, PaymapSettingKeys.BoolAuthLevel1);
            if (!string.IsNullOrEmpty(tempAuthLevel1)) bool.TryParse(tempAuthLevel1, out App.PaymapSettings.AuthLevel1);
            var tempAuthLevel2 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionAuth, PaymapSettingKeys.BoolAuthLevel2);
            if (!string.IsNullOrEmpty(tempAuthLevel2)) bool.TryParse(tempAuthLevel2, out App.PaymapSettings.AuthLevel2);
            var tempAuthLevel3 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionAuth, PaymapSettingKeys.BoolAuthLevel3);
            if (!string.IsNullOrEmpty(tempAuthLevel3)) bool.TryParse(tempAuthLevel3, out App.PaymapSettings.AuthLevel3);
            var tempVersionLive = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionVersion, PaymapSettingKeys.IntVersionLive);
            if (!string.IsNullOrEmpty(tempVersionLive)) int.TryParse(tempVersionLive, out App.PaymapSettings.VersionLive);
            var tempVersionMin = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionVersion, PaymapSettingKeys.IntVersionMin);
            if (!string.IsNullOrEmpty(tempVersionMin)) int.TryParse(tempVersionMin, out App.PaymapSettings.VersionMin);
            var tempThreadDashboard = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionThread, PaymapSettingKeys.BoolThreadDashboard);
            if (!string.IsNullOrEmpty(tempThreadDashboard)) bool.TryParse(tempThreadDashboard, out App.PaymapSettings.ThreadSwitchDashboard);
            var tempThreadDashboardInterval = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionThread, PaymapSettingKeys.IntThreadIntervalDashboard);
            if (!string.IsNullOrEmpty(tempThreadDashboardInterval)) int.TryParse(tempThreadDashboardInterval, out App.PaymapSettings.ThreadIntervalDashboard);
            var tempThreadDEVX = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionThread, PaymapSettingKeys.BoolThreadDEVX);
            if (!string.IsNullOrEmpty(tempThreadDEVX)) bool.TryParse(tempThreadDEVX, out App.PaymapSettings.ThreadSwitchDEVX);
            var tempThreadDEVXInterval = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionThread, PaymapSettingKeys.IntThreadIntervalDEVX);
            if (!string.IsNullOrEmpty(tempThreadDEVXInterval)) int.TryParse(tempThreadDEVXInterval, out App.PaymapSettings.ThreadIntervalDEVX);
            var tempAPIFirebase = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionAPI, PaymapSettingKeys.StringAPIFirebase);
            if (!string.IsNullOrEmpty(tempAPIFirebase)) App.PaymapSettings.APIFirebase = tempAPIFirebase;
            var tempAPICatcher = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionAPI, PaymapSettingKeys.StringAPICatcher);
            if (!string.IsNullOrEmpty(tempAPICatcher)) App.PaymapSettings.APICatcher = tempAPICatcher;
            var tempAPIReviewer = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionAPI, PaymapSettingKeys.StringAPIReviewer);
            if (!string.IsNullOrEmpty(tempAPIReviewer)) App.PaymapSettings.APIReviewer = tempAPIReviewer;
            var tempDBLocation = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionDB, PaymapSettingKeys.StringDBAddress);
            if (!string.IsNullOrEmpty(tempDBLocation)) App.PaymapSettings.DBAddress = tempDBLocation;
            var tempDBPort = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionDB, PaymapSettingKeys.IntDBPort);
            if (!string.IsNullOrEmpty(tempDBPort)) int.TryParse(tempDBPort, out App.PaymapSettings.DBPort);
            var tempDEVXMaster = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionDEVX, PaymapSettingKeys.BoolDEVXMaster);
            if (!string.IsNullOrEmpty(tempDEVXMaster)) bool.TryParse(tempDEVXMaster, out App.PaymapSettings.DEVXMaster);
            var tempDEVXReport = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionDEVX, PaymapSettingKeys.BoolDEVXReport);
            if (!string.IsNullOrEmpty(tempDEVXReport)) bool.TryParse(tempDEVXReport, out App.PaymapSettings.DEVXReport);
            var tempDEVXCommand = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionDEVX, PaymapSettingKeys.BoolDEVXCommand);
            if (!string.IsNullOrEmpty(tempDEVXCommand)) bool.TryParse(tempDEVXCommand, out App.PaymapSettings.DEVXCommand);
            var tempLogLevel1 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel1);
            if (!string.IsNullOrEmpty(tempLogLevel1)) bool.TryParse(tempLogLevel1, out App.PaymapSettings.LogLevel1);
            var tempLogLevel2 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel2);
            if (!string.IsNullOrEmpty(tempLogLevel2)) bool.TryParse(tempLogLevel2, out App.PaymapSettings.LogLevel2);
            var tempLogLevel3 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel3);
            if (!string.IsNullOrEmpty(tempLogLevel3)) bool.TryParse(tempLogLevel3, out App.PaymapSettings.LogLevel3);
            var tempLogLevel4 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel4);
            if (!string.IsNullOrEmpty(tempLogLevel4)) bool.TryParse(tempLogLevel4, out App.PaymapSettings.LogLevel4);
            var tempLogLevel5 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel5);
            if (!string.IsNullOrEmpty(tempLogLevel5)) bool.TryParse(tempLogLevel5, out App.PaymapSettings.LogLevel5);
            var tempLogFilterLevel1 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel1);
            if (!string.IsNullOrEmpty(tempLogFilterLevel1)) bool.TryParse(tempLogFilterLevel1, out App.PaymapSettings.LogFilterLevel1);
            var tempLogFilterLevel2 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel2);
            if (!string.IsNullOrEmpty(tempLogFilterLevel2)) bool.TryParse(tempLogFilterLevel2, out App.PaymapSettings.LogFilterLevel2);
            var tempLogFilterLevel3 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel3);
            if (!string.IsNullOrEmpty(tempLogFilterLevel3)) bool.TryParse(tempLogFilterLevel3, out App.PaymapSettings.LogFilterLevel3);
            var tempLogFilterLevel4 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel4);
            if (!string.IsNullOrEmpty(tempLogFilterLevel4)) bool.TryParse(tempLogFilterLevel4, out App.PaymapSettings.LogFilterLevel4);
            var tempLogFilterLevel5 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel5);
            if (!string.IsNullOrEmpty(tempLogFilterLevel5)) bool.TryParse(tempLogFilterLevel5, out App.PaymapSettings.LogFilterLevel5);
            var tempLogFilterType1 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType1);
            if (!string.IsNullOrEmpty(tempLogFilterType1)) bool.TryParse(tempLogFilterType1, out App.PaymapSettings.LogFilterType1);
            var tempLogFilterType2 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType2);
            if (!string.IsNullOrEmpty(tempLogFilterType2)) bool.TryParse(tempLogFilterType2, out App.PaymapSettings.LogFilterType2);
            var tempLogFilterType3 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType3);
            if (!string.IsNullOrEmpty(tempLogFilterType3)) bool.TryParse(tempLogFilterType3, out App.PaymapSettings.LogFilterType3);
            var tempLogFilterType4 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType4);
            if (!string.IsNullOrEmpty(tempLogFilterType4)) bool.TryParse(tempLogFilterType4, out App.PaymapSettings.LogFilterType4);
            var tempLogFilterType5 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType5);
            if (!string.IsNullOrEmpty(tempLogFilterType5)) bool.TryParse(tempLogFilterType5, out App.PaymapSettings.LogFilterType5);
            var tempLogFilterType6 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType6);
            if (!string.IsNullOrEmpty(tempLogFilterType6)) bool.TryParse(tempLogFilterType6, out App.PaymapSettings.LogFilterType6);
            var tempLogFilterType7 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType7);
            if (!string.IsNullOrEmpty(tempLogFilterType7)) bool.TryParse(tempLogFilterType7, out App.PaymapSettings.LogFilterType7);
            var tempLogFilterType8 = ConfigSystem.ReadConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType8);
            if (!string.IsNullOrEmpty(tempLogFilterType8)) bool.TryParse(tempLogFilterType8, out App.PaymapSettings.LogFilterType8);
            RefreshSettingData();
        }

        private static void SaveSettingFile(string filePath)
        {
            if (filePath == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Setting File|config.ini", 
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    SaveSettingFile(saveFileDialog.FileName);
                    return;
                }
            }
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionPort, PaymapSettingKeys.IntPortMain, App.PaymapSettings.PortMain.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionPort, PaymapSettingKeys.IntPortRemote, App.PaymapSettings.PortRemote.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionPort, PaymapSettingKeys.IntPortDEVX, App.PaymapSettings.PortDEVX.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionAuth, PaymapSettingKeys.BoolAuthLevel1, App.PaymapSettings.AuthLevel1.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionAuth, PaymapSettingKeys.BoolAuthLevel2, App.PaymapSettings.AuthLevel2.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionAuth, PaymapSettingKeys.BoolAuthLevel3, App.PaymapSettings.AuthLevel3.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionVersion, PaymapSettingKeys.IntVersionLive, App.PaymapSettings.VersionLive.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionVersion, PaymapSettingKeys.IntVersionMin, App.PaymapSettings.VersionMin.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionAuth, PaymapSettingKeys.BoolAuthLevel1, App.PaymapSettings.AuthLevel1.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionThread, PaymapSettingKeys.BoolThreadDashboard, App.PaymapSettings.ThreadSwitchDashboard.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionThread, PaymapSettingKeys.IntThreadIntervalDashboard, App.PaymapSettings.ThreadIntervalDashboard.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionThread, PaymapSettingKeys.BoolThreadDEVX, App.PaymapSettings.ThreadSwitchDEVX.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionThread, PaymapSettingKeys.IntThreadIntervalDEVX, App.PaymapSettings.ThreadIntervalDEVX.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionAPI, PaymapSettingKeys.StringAPIFirebase, App.PaymapSettings.APIFirebase);
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionAPI, PaymapSettingKeys.StringAPICatcher, App.PaymapSettings.APICatcher);
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionAPI, PaymapSettingKeys.StringAPIReviewer, App.PaymapSettings.APIReviewer);
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionDB, PaymapSettingKeys.StringDBAddress, App.PaymapSettings.DBAddress);
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionDB, PaymapSettingKeys.IntDBPort, App.PaymapSettings.DBPort.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionDEVX, PaymapSettingKeys.BoolDEVXMaster, App.PaymapSettings.DEVXMaster.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionDEVX, PaymapSettingKeys.BoolDEVXReport, App.PaymapSettings.DEVXReport.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionDEVX, PaymapSettingKeys.BoolDEVXCommand, App.PaymapSettings.DEVXCommand.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel1, App.PaymapSettings.LogLevel1.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel2, App.PaymapSettings.LogLevel2.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel3, App.PaymapSettings.LogLevel3.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel4, App.PaymapSettings.LogLevel4.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogLevel5, App.PaymapSettings.LogLevel5.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel1, App.PaymapSettings.LogFilterLevel1.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel2, App.PaymapSettings.LogFilterLevel2.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel3, App.PaymapSettings.LogFilterLevel3.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel4, App.PaymapSettings.LogFilterLevel4.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterLevel5, App.PaymapSettings.LogFilterLevel5.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType1, App.PaymapSettings.LogFilterType1.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType2, App.PaymapSettings.LogFilterType2.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType3, App.PaymapSettings.LogFilterType3.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType4, App.PaymapSettings.LogFilterType4.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType5, App.PaymapSettings.LogFilterType5.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType6, App.PaymapSettings.LogFilterType6.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType7, App.PaymapSettings.LogFilterType7.ToString());
            ConfigSystem.WriteConfig(filePath, PaymapSettingKeys.SectionLog, PaymapSettingKeys.BoolLogFilterType8, App.PaymapSettings.LogFilterType8.ToString());
        }

        private static class PaymapSettingKeys
        {
            public const string SectionPort = "PORT";
            public const string IntPortMain = "PortMain";
            public const string IntPortRemote = "PortRemote";
            public const string IntPortDEVX = "PortDEVX";
            public const string SectionAuth = "AUTH";
            public const string BoolAuthLevel1 = "AuthLevel1";
            public const string BoolAuthLevel2 = "AuthLevel2";
            public const string BoolAuthLevel3 = "AuthLevel3";
            public const string SectionVersion = "VERSION";
            public const string IntVersionMin = "VersionMin";
            public const string IntVersionLive = "VersionLive";
            public const string SectionThread = "THREAD";
            public const string BoolThreadDashboard = "ThreadSwitchDashboard";
            public const string BoolThreadDEVX = "ThreadSwitchDEVX";
            public const string IntThreadIntervalDashboard = "ThreadIntervalDashboard";
            public const string IntThreadIntervalDEVX = "ThreadIntervalDEVX";
            public const string SectionAPI = "API";
            public const string StringAPIFirebase = "APIFirebase";
            public const string StringAPICatcher = "APICatcher";
            public const string StringAPIReviewer = "APIReviewer";
            public const string SectionDB = "DB";
            public const string StringDBAddress = "DBAddress";
            public const string IntDBPort = "DBPort";
            public const string SectionDEVX = "DEVX";
            public const string BoolDEVXMaster = "DEVXMaster";
            public const string BoolDEVXReport = "DEVXReport";
            public const string BoolDEVXCommand = "DEVXCommand";
            public const string SectionLog = "LOG";
            public const string BoolLogLevel1 = "LogLevel1";
            public const string BoolLogLevel2 = "LogLevel2";
            public const string BoolLogLevel3 = "LogLevel3";
            public const string BoolLogLevel4 = "LogLevel4";
            public const string BoolLogLevel5 = "LogLevel5";
            public const string BoolLogFilterLevel1 = "LogFilterLevel1";
            public const string BoolLogFilterLevel2 = "LogFilterLevel2";
            public const string BoolLogFilterLevel3 = "LogFilterLevel3";
            public const string BoolLogFilterLevel4 = "LogFilterLevel4";
            public const string BoolLogFilterLevel5 = "LogFilterLevel5";
            public const string BoolLogFilterType1 = "LogFilterType1";
            public const string BoolLogFilterType2 = "LogFilterType2";
            public const string BoolLogFilterType3 = "LogFilterType3";
            public const string BoolLogFilterType4 = "LogFilterType4";
            public const string BoolLogFilterType5 = "LogFilterType5";
            public const string BoolLogFilterType6 = "LogFilterType6";
            public const string BoolLogFilterType7 = "LogFilterType7";
            public const string BoolLogFilterType8 = "LogFilterType8";
        }

        private static class ConfigSystem
        {
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
            
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
            
            public static void WriteConfig(string file, string section, string key, string val)
            {
                WritePrivateProfileString(section, key, val, GetFile(file));
            }
            
            public static string ReadConfig(string file, string section, string key)
            {
                StringBuilder temp = new StringBuilder(255);
                GetPrivateProfileString(section, key, null, temp, 255, GetFile(file));
                return temp.ToString();
            }
            
            private static string GetFile(string file)
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file.EndsWith(".ini")? file : file + ".ini");
            }
        }
        
    }
}