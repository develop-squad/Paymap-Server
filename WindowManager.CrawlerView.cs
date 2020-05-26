using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using PAYMAP_BACKEND.Views;
using MahApps.Metro.Controls;
using WebSocketSharp;

namespace PAYMAP_BACKEND
{
    partial class WindowManager
    {
        private static CrawlerView _crawlerView;

        private static TextBlock _crawlerHeaderPower;
        private static TextBlock _crawlerHeaderStatus;
        private static TextBlock _crawlerHeaderTime;
        private static TextBlock _crawlerHeaderCount;
        private static DataGrid _crawlerDataView;
        private static CheckBox _crawlerCheckSync;
        private static ComboBox _crawlerComboBox1;
        private static ComboBox _crawlerComboBox2;
        private static ComboBox _crawlerComboBox3;
        private static TextBox _crawlerNameBox;
        private static Button _crawlerButtonStop;
        private static Button _crawlerButtonStart;
        
        private static void InitializeCrawlerWindow()
        {
            if (_crawlerView == null) return;
            _crawlerHeaderPower = (TextBlock) _crawlerView.FindName("CrawlerHeaderPower");
            _crawlerHeaderStatus = (TextBlock) _crawlerView.FindName("CrawlerHeaderStatus");
            _crawlerHeaderTime = (TextBlock) _crawlerView.FindName("CrawlerHeaderTime");
            _crawlerHeaderCount = (TextBlock) _crawlerView.FindName("CrawlerHeaderCount");
            if (_crawlerHeaderPower == null || _crawlerHeaderStatus == null || _crawlerHeaderTime == null || _crawlerHeaderCount == null) return;
            _crawlerDataView = (DataGrid) _crawlerView.FindName("CrawlerDataView");
            if (_crawlerDataView == null) return;
            _crawlerCheckSync = (CheckBox) _crawlerView.FindName("CrawlerSync");
            _crawlerComboBox1 = (ComboBox) _crawlerView.FindName("CrawlerType1");
            _crawlerComboBox2 = (ComboBox) _crawlerView.FindName("CrawlerType2");
            _crawlerComboBox3 = (ComboBox) _crawlerView.FindName("CrawlerType3");
            _crawlerNameBox = (TextBox) _crawlerView.FindName("CrawlerType4");
            if (_crawlerCheckSync == null || _crawlerComboBox1 == null || _crawlerComboBox2 == null || _crawlerComboBox3 == null || _crawlerNameBox == null) return;
            _crawlerButtonStop = (Button) _crawlerView.FindName("CrawlerButtonStop");
            _crawlerButtonStart = (Button) _crawlerView.FindName("CrawlerButtonStart");
            if (_crawlerButtonStop == null || _crawlerButtonStart == null) return;
            
            _crawlerComboBox1.ItemsSource = CrawlManager.SiDoDictionary.Keys;
            _crawlerComboBox1.SelectedItem = CrawlManager.SiDoDictionary.First().Key;
            _crawlerComboBox1.Items.Refresh();
            _crawlerComboBox3.ItemsSource = CrawlManager.TypeDictionary.Keys;
            _crawlerComboBox3.SelectedItem = CrawlManager.TypeDictionary.First().Key;
            _crawlerComboBox3.Items.Refresh();
            _crawlerComboBox1.SelectionChanged += (sender, args) =>
            {
                Dictionary<string, int> subData = CrawlManager.SiGunGuDictionary[CrawlManager.SiDoDictionary[_crawlerComboBox1.SelectedItem.ToString()]];
                _crawlerComboBox2.ItemsSource = subData.Keys;
                if (CrawlManager.FilterSiDo != CrawlManager.SiDoDictionary[_crawlerComboBox1.SelectedItem.ToString()]) _crawlerComboBox2.SelectedItem = subData.First().Key;
                _crawlerComboBox2.Items.Refresh();
                CrawlManager.FilterSiDo = CrawlManager.SiDoDictionary[_crawlerComboBox1.SelectedItem.ToString()];
            };
            _crawlerComboBox2.SelectionChanged += (sender, args) =>
            {
                if (_crawlerComboBox2.SelectedItem == null) return;
                CrawlManager.FilterSiGunGu = CrawlManager.SiGunGuDictionary[CrawlManager.FilterSiDo][_crawlerComboBox2.SelectedItem.ToString()];
            };
            _crawlerComboBox3.SelectionChanged += (sender, args) =>
            {
                CrawlManager.FilterType = _crawlerComboBox3.SelectedItem.ToString();
            };
            _crawlerCheckSync.Checked += (sender, args) =>
            {
                CrawlManager.ModuleSync = true;
            };
            _crawlerCheckSync.Unchecked += (sender, args) =>
            {
                CrawlManager.ModuleSync = false;
            };
            _crawlerNameBox.TextChanged += (sender, args) =>
            {
                CrawlManager.FilterName = _crawlerNameBox.Text;
            };
            _crawlerButtonStop.Click += (sender, args) =>
            {
                CrawlManager.StopCrawlZeroPay();
            };
            _crawlerButtonStart.Click += (sender, args) =>
            {
                CrawlManager.StartCrawlZeroPay();
            };

            UpdateCrawlData();
            LoadCrawlSetting();
        }

        private static void LoadCrawlSetting()
        {
            _crawlerCheckSync.IsChecked = CrawlManager.ModuleSync;
            _crawlerComboBox1.SelectedItem = CrawlManager.SiDoDictionary.FirstOrDefault(x => x.Value == CrawlManager.FilterSiDo).Key;
            _crawlerComboBox1.Items.Refresh();
            Dictionary<string, int> subData = CrawlManager.SiGunGuDictionary[CrawlManager.SiDoDictionary[_crawlerComboBox1.SelectedItem.ToString()]];
            _crawlerComboBox2.ItemsSource = subData.Keys;
            _crawlerComboBox2.SelectedItem = CrawlManager.SiGunGuDictionary[CrawlManager.FilterSiDo].FirstOrDefault(x => x.Value == CrawlManager.FilterSiGunGu).Key;
            _crawlerComboBox2.Items.Refresh();
            _crawlerComboBox3.SelectedItem = CrawlManager.FilterType;
            _crawlerNameBox.Text = CrawlManager.FilterName;
        }

        public static void UpdateCrawlData()
        {
            _crawlerHeaderPower.Text = CrawlManager.IsModuleRunning ? "ON" : "OFF";
            switch (CrawlManager.ModuleStatus)
            {
                case CrawlManager.RunStatus.Idle:
                    _crawlerHeaderStatus.Text = "IDLE";
                    break;
                case CrawlManager.RunStatus.Running:
                    _crawlerHeaderStatus.Text = "WORKING";
                    break;
                case CrawlManager.RunStatus.Error:
                    _crawlerHeaderStatus.Text = "ERROR";
                    break;
            }
            long workedTime = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - CrawlManager.StartTime;
            _crawlerHeaderTime.Text = $"{workedTime / 3600:00}:{workedTime % 3600 / 60:00}:{workedTime % 60:00}";
            _crawlerHeaderCount.Text = CrawlManager.FinishedCount.ToString();
            _crawlerDataView.ItemsSource = new List<CrawlData>(CrawlManager.CrawlingData);
            _crawlerDataView.Items.Refresh();
        }

        public static void SetCrawlerView(CrawlerView crawlerView)
        {
            _crawlerView = crawlerView;
            InitializeCrawlerWindow();
        }

    }
}