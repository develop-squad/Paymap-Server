using System;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using PAYMAP_BACKEND.Utils;

namespace PAYMAP_BACKEND.Views
{
    public class MainViewModel : PropertyChangedViewModel
    {
        public static HamburgerMenuItemCollection MainMenuItems;
        public static HamburgerMenuItemCollection MainMenuOptionItems;

        public static HamburgerMenuImageItem MainMenuItemDashboard;
        public static HamburgerMenuImageItem MainMenuItemCrawler;
        public static HamburgerMenuImageItem MainMenuItemConsole;
        public static HamburgerMenuImageItem MainMenuItemSetting;
        public static HamburgerMenuImageItem MainMenuItemPAYMAP;

        public MainViewModel()
        {
            this.CreateMenuItems();
        }

        private void CreateMenuItems()
        {
            MainMenuItemDashboard = new HamburgerMenuImageItem()
            {
                Thumbnail = new BitmapImage(new Uri(@"/PAYMAP_BACKEND;component/Resources/icon_dashboard.png", UriKind.Relative)),
                Label = "Dashboard",
                ToolTip = "Dashboard",
                Tag = new DashboardViewModel(this)
            };
            MainMenuItemCrawler = new HamburgerMenuImageItem()
            {
                Thumbnail = new BitmapImage(new Uri(@"/PAYMAP_BACKEND;component/Resources/icon_spider.png", UriKind.Relative)),
                Label = "Crawler",
                ToolTip = "Crawler",
                Tag = new CrawlerViewModel(this)
            };
            MainMenuItemConsole = new HamburgerMenuImageItem()
            {
                Thumbnail = new BitmapImage(new Uri(@"/PAYMAP_BACKEND;component/Resources/icon_log.png", UriKind.Relative)),
                Label = "Console",
                ToolTip = "Console",
                Tag = new ConsoleViewModel(this)
            };
            MainMenuItemSetting = new HamburgerMenuImageItem()
            {
                Thumbnail = new BitmapImage(new Uri(@"/PAYMAP_BACKEND;component/Resources/icon_switch.png", UriKind.Relative)),
                Label = "Setting",
                ToolTip = "Setting",
                Tag = new SettingViewModel(this)
            };
            MenuItems = new HamburgerMenuItemCollection
            {
               MainMenuItemDashboard, MainMenuItemCrawler, MainMenuItemConsole, MainMenuItemSetting 
            };

            MainMenuItemPAYMAP = new HamburgerMenuImageItem()
            {
                Thumbnail = new BitmapImage(new Uri(@"/PAYMAP_BACKEND;component/Resources/icon_zeropay_white.png", UriKind.Relative)),
                Label = "PAYMAP",
                ToolTip = "PAYMAP",
                Tag = new SplashViewModel(this)
            };

            MenuOptionItems = new HamburgerMenuItemCollection
            {
                MainMenuItemPAYMAP
            };
        }

        public HamburgerMenuItemCollection MenuItems
        {
            get => MainMenuItems;
            set
            {
                if (Equals(value, MainMenuItems)) return;
                MainMenuItems = value;
                OnPropertyChanged();
            }
        }

        public HamburgerMenuItemCollection MenuOptionItems
        {
            get => MainMenuOptionItems;
            set
            {
                if (Equals(value, MainMenuOptionItems)) return;
                MainMenuOptionItems = value;
                OnPropertyChanged();
            }
        }
    }
}