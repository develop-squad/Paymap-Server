using System;
using System.Collections.Generic;
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
        private static TextBlock _crawlerHeaderPage;
        private static DataGrid _crawlerDataView;
        private static CheckBox _crawlerCheckSync;
        private static ComboBox _crawlerComboBox1;
        private static ComboBox _crawlerComboBox2;
        private static ComboBox _crawlerComboBox3;
        private static TextBox _crawlerTypeBox;
        private static Button _crawlerButtonStop;
        private static Button _crawlerButtonStart;
        
        private static void InitializeCrawlerWindow()
        {
            if (_crawlerView == null) return;
            _crawlerHeaderPower = (TextBlock) _crawlerView.FindName("CrawlerHeaderPower");
            _crawlerHeaderStatus = (TextBlock) _crawlerView.FindName("CrawlerHeaderStatus");
            _crawlerHeaderTime = (TextBlock) _crawlerView.FindName("CrawlerHeaderTime");
            _crawlerHeaderPage = (TextBlock) _crawlerView.FindName("CrawlerHeaderPage");
            if (_crawlerHeaderPower == null || _crawlerHeaderStatus == null || _crawlerHeaderTime == null || _crawlerHeaderPage == null) return;
            _crawlerDataView = (DataGrid) _crawlerView.FindName("CrawlDataView");
            if (_crawlerDataView == null) return;
            _crawlerCheckSync = (CheckBox) _crawlerView.FindName("CrawlerSync");
            _crawlerComboBox1 = (ComboBox) _crawlerView.FindName("CrawlerType1");
            _crawlerComboBox2 = (ComboBox) _crawlerView.FindName("CrawlerType2");
            _crawlerComboBox3 = (ComboBox) _crawlerView.FindName("CrawlerType3");
            _crawlerTypeBox = (TextBox) _crawlerView.FindName("CrawlerType4");
            if (_crawlerCheckSync == null || _crawlerComboBox1 == null || _crawlerComboBox2 == null || _crawlerComboBox3 == null || _crawlerTypeBox == null) return;
            _crawlerButtonStop = (Button) _crawlerView.FindName("CrawlerButtonStop");
            _crawlerButtonStart = (Button) _crawlerView.FindName("CrawlerButtonStart");
            if (_crawlerButtonStop == null || _crawlerButtonStart == null) return;
            
            _crawlerComboBox1.SelectionChanged += (sender, args) => InitializeSiGunGuData(_crawlerComboBox1, _crawlerComboBox2);
            _crawlerComboBox3.SelectedItem = _crawlerComboBox3.Items[0];
        }
        
        public static void SetCrawlerView(CrawlerView crawlerView)
        {
            _crawlerView = crawlerView;
            InitializeCrawlerWindow();
        }

        private static void InitializeSiGunGuData(ComboBox bigBox, ComboBox midBox)
        {
            if (bigBox == null || midBox == null) return;
            List<string> midBoxItems = new List<string>();
            switch (((ComboBoxItem)bigBox.SelectedItem).Content)
            {
                case "서울특별시":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("강남구");
                    midBoxItems.Add("강동구");
                    midBoxItems.Add("강북구");
                    midBoxItems.Add("강서구");
                    midBoxItems.Add("관악구");
                    midBoxItems.Add("광진구");
                    midBoxItems.Add("구로구");
                    midBoxItems.Add("금천구");
                    midBoxItems.Add("노원구");
                    midBoxItems.Add("도봉구");
                    midBoxItems.Add("동대문구");
                    midBoxItems.Add("동작구");
                    midBoxItems.Add("마포구");
                    midBoxItems.Add("서대문구");
                    midBoxItems.Add("서초구");
                    midBoxItems.Add("성동구");
                    midBoxItems.Add("성북구");
                    midBoxItems.Add("송파구");
                    midBoxItems.Add("양천구");
                    midBoxItems.Add("영등포구");
                    midBoxItems.Add("용산구");
                    midBoxItems.Add("은평구");
                    midBoxItems.Add("종로구");
                    midBoxItems.Add("중구");
                    midBoxItems.Add("중랑구");
                    break;
                case "부산광역시":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("강서구");
                    midBoxItems.Add("금정구");
                    midBoxItems.Add("기장군");
                    midBoxItems.Add("남구");
                    midBoxItems.Add("동구");
                    midBoxItems.Add("동래구");
                    midBoxItems.Add("부산진구");
                    midBoxItems.Add("북구");
                    midBoxItems.Add("사상구");
                    midBoxItems.Add("사하구");
                    midBoxItems.Add("서구");
                    midBoxItems.Add("수영구");
                    midBoxItems.Add("연제구");
                    midBoxItems.Add("영도구");
                    midBoxItems.Add("중구");
                    midBoxItems.Add("해운대구");
                    break;
                case "울산광역시":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("남구");
                    midBoxItems.Add("동구");
                    midBoxItems.Add("북구");
                    midBoxItems.Add("울주군");
                    midBoxItems.Add("중구");
                    break;
                case "인천광역시":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("강화군");
                    midBoxItems.Add("계양구");
                    midBoxItems.Add("남동구");
                    midBoxItems.Add("동구");
                    midBoxItems.Add("미추홀구");
                    midBoxItems.Add("부평구");
                    midBoxItems.Add("서구");
                    midBoxItems.Add("연수구");
                    midBoxItems.Add("옹진군");
                    midBoxItems.Add("중구");
                    break;
                case "대구광역시":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("남구");
                    midBoxItems.Add("달서구");
                    midBoxItems.Add("달성군");
                    midBoxItems.Add("동구");
                    midBoxItems.Add("북구");
                    midBoxItems.Add("서구");
                    midBoxItems.Add("수성구");
                    midBoxItems.Add("중구");
                    break;
                case "대전광역시":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("대덕구");
                    midBoxItems.Add("동구");
                    midBoxItems.Add("서구");
                    midBoxItems.Add("유성구");
                    midBoxItems.Add("중구");
                    break;
                case "광주광역시":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("광산구");
                    midBoxItems.Add("남구");
                    midBoxItems.Add("동구");
                    midBoxItems.Add("북구");
                    midBoxItems.Add("서구");
                    break;
                case "경기도":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("가평군");
                    midBoxItems.Add("고양시");
                    midBoxItems.Add("과천시");
                    midBoxItems.Add("광명시");
                    midBoxItems.Add("광주시");
                    midBoxItems.Add("구리시");
                    midBoxItems.Add("군포시");
                    midBoxItems.Add("김포시");
                    midBoxItems.Add("남양주시");
                    midBoxItems.Add("동두천시");
                    midBoxItems.Add("부천시");
                    midBoxItems.Add("성남시");
                    midBoxItems.Add("수원시");
                    midBoxItems.Add("시흥시");
                    midBoxItems.Add("안산시");
                    midBoxItems.Add("안성시");
                    midBoxItems.Add("안양시");
                    midBoxItems.Add("양주시");
                    midBoxItems.Add("양평군");
                    midBoxItems.Add("여주시");
                    midBoxItems.Add("연천군");
                    midBoxItems.Add("오산시");
                    midBoxItems.Add("용인시");
                    midBoxItems.Add("의왕시");
                    midBoxItems.Add("의정부시");
                    midBoxItems.Add("이천시");
                    midBoxItems.Add("파주시");
                    midBoxItems.Add("평택시");
                    midBoxItems.Add("포천시");
                    midBoxItems.Add("하남시");
                    midBoxItems.Add("화성시");
                    break;
                case "강원도":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("강릉시");
                    midBoxItems.Add("고성군");
                    midBoxItems.Add("동해시");
                    midBoxItems.Add("삼척시");
                    midBoxItems.Add("속초시");
                    midBoxItems.Add("양구군");
                    midBoxItems.Add("양양군");
                    midBoxItems.Add("영월군");
                    midBoxItems.Add("원주시");
                    midBoxItems.Add("인제군");
                    midBoxItems.Add("정선군");
                    midBoxItems.Add("철원군");
                    midBoxItems.Add("춘천시");
                    midBoxItems.Add("태백시");
                    midBoxItems.Add("평창군");
                    midBoxItems.Add("홍천군");
                    midBoxItems.Add("화천군");
                    midBoxItems.Add("횡성군");
                    break;
                case "충청북도":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("괴산군");
                    midBoxItems.Add("단양군");
                    midBoxItems.Add("보은군");
                    midBoxItems.Add("영동군");
                    midBoxItems.Add("옥천군");
                    midBoxItems.Add("음성군");
                    midBoxItems.Add("제천시");
                    midBoxItems.Add("증평군");
                    midBoxItems.Add("진천군");
                    midBoxItems.Add("청주시");
                    midBoxItems.Add("충주시");
                    break;
                case "충청남도":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("계룡시");
                    midBoxItems.Add("공주시");
                    midBoxItems.Add("금산군");
                    midBoxItems.Add("논산시");
                    midBoxItems.Add("당진시");
                    midBoxItems.Add("보령시");
                    midBoxItems.Add("부여군");
                    midBoxItems.Add("서산시");
                    midBoxItems.Add("서천군");
                    midBoxItems.Add("아산시");
                    midBoxItems.Add("예산군");
                    midBoxItems.Add("천안시");
                    midBoxItems.Add("청양군");
                    midBoxItems.Add("태안군");
                    midBoxItems.Add("홍성군");
                    break;
                case "경상북도":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("경산시");
                    midBoxItems.Add("경주시");
                    midBoxItems.Add("고령군");
                    midBoxItems.Add("구미시");
                    midBoxItems.Add("군위군");
                    midBoxItems.Add("김천시");
                    midBoxItems.Add("문경시");
                    midBoxItems.Add("봉화군");
                    midBoxItems.Add("상주시");
                    midBoxItems.Add("성주군");
                    midBoxItems.Add("안동시");
                    midBoxItems.Add("영덕군");
                    midBoxItems.Add("영양군");
                    midBoxItems.Add("영주시");
                    midBoxItems.Add("영천시");
                    midBoxItems.Add("예천군");
                    midBoxItems.Add("울릉군");
                    midBoxItems.Add("울진군");
                    midBoxItems.Add("의성군");
                    midBoxItems.Add("청도군");
                    midBoxItems.Add("청송군");
                    midBoxItems.Add("칠곡군");
                    midBoxItems.Add("포항시");
                    break;
                case "경상남도":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("거제시");
                    midBoxItems.Add("거창군");
                    midBoxItems.Add("고성군");
                    midBoxItems.Add("김해시");
                    midBoxItems.Add("남해군");
                    midBoxItems.Add("밀양시");
                    midBoxItems.Add("사천시");
                    midBoxItems.Add("산청군");
                    midBoxItems.Add("양산시");
                    midBoxItems.Add("의령군");
                    midBoxItems.Add("진주시");
                    midBoxItems.Add("창녕군");
                    midBoxItems.Add("창원시");
                    midBoxItems.Add("통영시");
                    midBoxItems.Add("하동군");
                    midBoxItems.Add("함안군");
                    midBoxItems.Add("함양군");
                    midBoxItems.Add("합천군");
                    break;
                case "전라북도":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("고창군");
                    midBoxItems.Add("군산시");
                    midBoxItems.Add("김제시");
                    midBoxItems.Add("남원시");
                    midBoxItems.Add("무주군");
                    midBoxItems.Add("부안군");
                    midBoxItems.Add("순창군");
                    midBoxItems.Add("완주군");
                    midBoxItems.Add("익산시");
                    midBoxItems.Add("임실군");
                    midBoxItems.Add("장수군");
                    midBoxItems.Add("전주시");
                    midBoxItems.Add("정읍시");
                    midBoxItems.Add("진안군");
                    break;
                case "전라남도":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("강진군");
                    midBoxItems.Add("고흥군");
                    midBoxItems.Add("곡성군");
                    midBoxItems.Add("광양시");
                    midBoxItems.Add("구례군");
                    midBoxItems.Add("나주시");
                    midBoxItems.Add("담양군");
                    midBoxItems.Add("목포시");
                    midBoxItems.Add("무안군");
                    midBoxItems.Add("보성군");
                    midBoxItems.Add("순천시");
                    midBoxItems.Add("신안군");
                    midBoxItems.Add("여수시");
                    midBoxItems.Add("영광군");
                    midBoxItems.Add("영암군");
                    midBoxItems.Add("완도군");
                    midBoxItems.Add("장성군");
                    midBoxItems.Add("장흥군");
                    midBoxItems.Add("진도군");
                    midBoxItems.Add("함평군");
                    midBoxItems.Add("해남군");
                    midBoxItems.Add("화순군");
                    break;
                case "제주특별자치도":
                    midBoxItems.Add("전체");
                    midBoxItems.Add("서귀포시");
                    midBoxItems.Add("제주시");
                    break;
                case "세종특별자치시":
                    midBoxItems.Add("전체");
                    break;
            }
            midBox.ItemsSource = midBoxItems;
            midBox.SelectedItem = midBoxItems[0];
            midBox.Items.Refresh();
        }
        
    }
}