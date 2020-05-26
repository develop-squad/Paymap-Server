using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using System.Windows;
using PAYMAP_BACKEND.Utils;

namespace PAYMAP_BACKEND
{
    public enum CrawlDataResult
    {
        Ready, Queue, Finish, Error
    }
        
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public struct CrawlData
    {
        public string Name { get; set; }
        public string AddressSiDo { get; set; }
        public string AddressSiGunGu { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public CrawlDataResult Status { get; set; }
    }
    
    public class CrawlManager
    {
        private static CrawlManager _instance;

        public static bool IsModuleRunning = false;
        public static bool IsModuleHealthy = false;

        private static bool _moduleRunFlag = false;
        private static Thread _crawlerThread;
        
        public enum RunStatus
        {
            Idle, Running, Error
        }

        public static Dictionary<string, int> SiDoDictionary;
        public static Dictionary<int, Dictionary<string, int>> SiGunGuDictionary;
        public static Dictionary<string, bool> TypeDictionary;

        public static List<CrawlData> CrawlingData;
        public static RunStatus ModuleStatus = RunStatus.Idle;
        public static bool ModuleSync = false;
        public static int FilterSiDo = 1;
        public static int FilterSiGunGu = 0;
        public static string FilterType = string.Empty;
        public static string FilterName = string.Empty;
        public static long StartTime = 0;
        public static int FinishedCount = 0; 
        
        private CrawlManager()
        {
            SiDoDictionary = new Dictionary<string, int>();
            SiGunGuDictionary = new Dictionary<int, Dictionary<string, int>>();
            TypeDictionary = new Dictionary<string, bool>();
            InitializeZeroPayData();
            CrawlingData = new List<CrawlData>();
        }

        public static CrawlManager GetInstance()
        {
            return _instance ?? (_instance = new CrawlManager());
        }

        public static void StartCrawlZeroPay()
        {
            _moduleRunFlag = true;
            _crawlerThread = new Thread(CrawlZeroPay)
            {
                Priority = ThreadPriority.Normal
            };
            if (!_crawlerThread.IsAlive)
            {
                _crawlerThread.Start();
            }
        }

        public static void StopCrawlZeroPay(bool hardOff = false)
        {
            _moduleRunFlag = false;
            if (!hardOff || _crawlerThread == null || !_crawlerThread.IsAlive) return;
            try
            {
                _crawlerThread.Interrupt();
                _crawlerThread = null;
            }
            catch (Exception exception)
            {
                LogManager.NewLog(LogType.WindowManager, LogLevel.Error, "StopDashboardMonitor", exception);
            }
        }
        
        private static void CrawlZeroPay()
        {
            StartTime = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            
            int pointerIndex = 1;

            WebClient crawlerClient = new WebClient {Encoding = Encoding.UTF8};

            while (true)
            {
                if (!_moduleRunFlag)
                {
                    LogManager.NewLog(LogType.WindowManager, LogLevel.Info, "CrawlZeroPay", "CrawlZeroPay Thread Stop : Safe");
                    break;
                }

                if (CrawlingData.Count >= 100)
                {
                    // TODO REFRESH HOLDING DATA STATUS
                    LogManager.NewLog(LogType.WindowManager, LogLevel.Info, "CrawlZeroPay", "CrawlZeroPay Parse Limit");
                    break;
                }

                string requestURL = $"https://www.zeropay.or.kr/intro/frncSrchList_json.do?firstIndex={pointerIndex}&lastIndex={pointerIndex+9}&searchCondition=&tryCode={FilterSiDo:00}";
                if (FilterSiGunGu != 0) requestURL += $"&skkCode={FilterSiGunGu}";
                if (FilterType != null && FilterType.Equals(string.Empty)) requestURL += $"&bztypName={FilterType}";
                if (FilterName != null && FilterName.Equals(string.Empty)) requestURL += $"&pobsAfstrName={FilterName}";
                var webResult = crawlerClient.DownloadString(requestURL);

                try
                {
                    JObject zeroJson = JObject.Parse(webResult);
                    JToken zeroResult;
                    if (!zeroJson.TryGetValue("result", out zeroResult) || zeroResult.ToString().Equals("FAIL"))
                    {
                        LogManager.NewLog(LogType.WindowManager, LogLevel.Error, "CrawlZeroPay", "CrawlZeroPay Parse Fail");
                        break;
                    }

                    JToken zeroTotal;
                    if (!zeroJson.TryGetValue("totalCnt", out zeroTotal) || pointerIndex >= zeroTotal.Value<int>())
                    {
                        LogManager.NewLog(LogType.WindowManager, LogLevel.Info, "CrawlZeroPay", "CrawlZeroPay Parse Checked", 
                            $"Size : {zeroTotal}, SiDo : {SiDoDictionary.FirstOrDefault(x => x.Value == CrawlManager.FilterSiDo).Key}, " +
                            $"SiGunGu : {SiGunGuDictionary[FilterSiDo].FirstOrDefault(x => x.Value == CrawlManager.FilterSiGunGu).Key}");
                        break;
                    }

                    JToken zeroList;
                    if (!zeroJson.TryGetValue("list", out zeroList) || (zeroList as JArray).Count == 0)
                    {
                        LogManager.NewLog(LogType.WindowManager, LogLevel.Info, "CrawlZeroPay", "CrawlZeroPay Parse Empty", 
                            $"Size : {zeroTotal}, SiDo : {SiDoDictionary.FirstOrDefault(x => x.Value == CrawlManager.FilterSiDo).Key}, " +
                            $"SiGunGu : {SiGunGuDictionary[FilterSiDo].FirstOrDefault(x => x.Value == CrawlManager.FilterSiGunGu).Key}");
                        break;
                    }

                    int zeroListCount = zeroList.Count();
                    for (int i = 0; i < zeroListCount; i++)
                    {
                        JToken zeroObject = zeroList[i];
                        CrawlData zeroData = new CrawlData();
                        zeroData.Name = zeroObject["pobsAfstrName"].Value<string>();
                        zeroData.Type = zeroObject["bztypName"].Value<string>();
                        string zeroAddress = zeroObject["pobsBaseAddr"].Value<string>();
                        zeroAddress += " " + zeroObject["pobsDtlAddr"].Value<string>();
                        try
                        {
                            string[] zeroAddressArray = zeroAddress.Split(' ');
                            zeroData.AddressSiDo = zeroAddressArray[0];
                            zeroData.AddressSiGunGu = zeroAddressArray[1];
                            zeroData.Address = zeroAddress.Substring(zeroAddressArray[0].Length + zeroAddressArray[1].Length + 2);
                        }
                        catch (Exception exception)
                        {
                            zeroData.AddressSiDo = string.Empty;
                            zeroData.AddressSiGunGu = string.Empty;
                            zeroData.Address = zeroAddress;
                        }
                        zeroData.Status = CrawlDataResult.Ready;
                        CrawlingData.Add(zeroData);
                    }
                    
                }
                catch (Exception exception)
                {
                    LogManager.NewLog(LogType.WindowManager, LogLevel.Error, "CrawlZeroPay", "CrawlZeroPay Parse Error", exception.StackTrace);
                    break;
                }

                pointerIndex += 10;
                
                Application.Current.Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new Action(WindowManager.UpdateCrawlData));
            }
        }
        
        public static void InitializeZeroPayData()
        {
            SiDoDictionary.Add("서울특별시", 1);
            SiDoDictionary.Add("부산광역시", 2);
            SiDoDictionary.Add("울산광역시", 3);
            SiDoDictionary.Add("인천광역시", 4);
            SiDoDictionary.Add("대구광역시", 5);
            SiDoDictionary.Add("대전광역시", 6);
            SiDoDictionary.Add("광주광역시", 7);
            SiDoDictionary.Add("경기도", 8);
            SiDoDictionary.Add("강원도", 9);
            SiDoDictionary.Add("충청북도", 10);
            SiDoDictionary.Add("충청남도", 11);
            SiDoDictionary.Add("경상북도", 12);
            SiDoDictionary.Add("경상남도", 13);
            SiDoDictionary.Add("전라북도", 14);
            SiDoDictionary.Add("전라남도", 15);
            SiDoDictionary.Add("제주특별자치도", 16);
            SiDoDictionary.Add("세종특별자치시", 17);
            Dictionary<string, int> subDictionary1 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary2 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary3 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary4 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary5 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary6 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary7 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary8 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary9 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary10 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary11 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary12 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary13 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary14 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary15 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary16 = new Dictionary<string, int>();
            Dictionary<string, int> subDictionary17 = new Dictionary<string, int>();
            SiGunGuDictionary.Add(1, subDictionary1);
            SiGunGuDictionary.Add(2, subDictionary2);
            SiGunGuDictionary.Add(3, subDictionary3);
            SiGunGuDictionary.Add(4, subDictionary4);
            SiGunGuDictionary.Add(5, subDictionary5);
            SiGunGuDictionary.Add(6, subDictionary6);
            SiGunGuDictionary.Add(7, subDictionary7);
            SiGunGuDictionary.Add(8, subDictionary8);
            SiGunGuDictionary.Add(9, subDictionary9);
            SiGunGuDictionary.Add(10, subDictionary10);
            SiGunGuDictionary.Add(11, subDictionary11);
            SiGunGuDictionary.Add(12, subDictionary12);
            SiGunGuDictionary.Add(13, subDictionary13);
            SiGunGuDictionary.Add(14, subDictionary14);
            SiGunGuDictionary.Add(15, subDictionary15);
            SiGunGuDictionary.Add(16, subDictionary16);
            SiGunGuDictionary.Add(17, subDictionary17);
            subDictionary1.Add("전체", 0);
            subDictionary1.Add("강남구", 1);
            subDictionary1.Add("강동구", 2);
            subDictionary1.Add("강북구", 3);
            subDictionary1.Add("강서구", 4);
            subDictionary1.Add("관악구", 5);
            subDictionary1.Add("광진구", 6);
            subDictionary1.Add("구로구", 7);
            subDictionary1.Add("금천구", 8);
            subDictionary1.Add("노원구", 9);
            subDictionary1.Add("도봉구", 10);
            subDictionary1.Add("동대문구", 11);
            subDictionary1.Add("동작구", 12);
            subDictionary1.Add("마포구", 13);
            subDictionary1.Add("서대문구", 14);
            subDictionary1.Add("서초구", 15);
            subDictionary1.Add("성동구", 16);
            subDictionary1.Add("성북구", 17);
            subDictionary1.Add("송파구", 18);
            subDictionary1.Add("양천구", 19);
            subDictionary1.Add("영등포구", 20);
            subDictionary1.Add("용산구", 21);
            subDictionary1.Add("은평구", 22);
            subDictionary1.Add("종로구", 23);
            subDictionary1.Add("중구", 24);
            subDictionary1.Add("중랑구", 25);
            subDictionary2.Add("전체", 0);
            subDictionary2.Add("강서구", 1);
            subDictionary2.Add("금정구", 2);
            subDictionary2.Add("기장군", 3);
            subDictionary2.Add("남구", 4);
            subDictionary2.Add("동구", 5);
            subDictionary2.Add("동래구", 6);
            subDictionary2.Add("부산진구", 7);
            subDictionary2.Add("북구", 8);
            subDictionary2.Add("사상구", 9);
            subDictionary2.Add("사하구", 10);
            subDictionary2.Add("서구", 11);
            subDictionary2.Add("수영구", 12);
            subDictionary2.Add("연제구", 13);
            subDictionary2.Add("영도구", 14);
            subDictionary2.Add("중구", 15);
            subDictionary2.Add("해운대구", 16);
            subDictionary3.Add("전체", 0);
            subDictionary3.Add("남구", 1);
            subDictionary3.Add("동구", 2);
            subDictionary3.Add("북구", 3);
            subDictionary3.Add("울주군", 4);
            subDictionary3.Add("중구", 5);
            subDictionary4.Add("전체", 0);
            subDictionary4.Add("강화군", 1);
            subDictionary4.Add("계양구", 2);
            subDictionary4.Add("남동구", 3);
            subDictionary4.Add("동구", 4);
            subDictionary4.Add("미추홀구", 5);
            subDictionary4.Add("부평구", 6);
            subDictionary4.Add("서구", 7);
            subDictionary4.Add("연수구", 8);
            subDictionary4.Add("옹진군", 9);
            subDictionary4.Add("중구", 10);
            subDictionary5.Add("전체", 0);
            subDictionary5.Add("남구", 1);
            subDictionary5.Add("달서구", 2);
            subDictionary5.Add("달성군", 3);
            subDictionary5.Add("동구", 4);
            subDictionary5.Add("북구", 5);
            subDictionary5.Add("서구", 6);
            subDictionary5.Add("수성구", 7);
            subDictionary5.Add("중구", 8);
            subDictionary6.Add("전체", 0);
            subDictionary6.Add("대덕구", 1);
            subDictionary6.Add("동구", 2);
            subDictionary6.Add("서구", 3);
            subDictionary6.Add("유성구", 4);
            subDictionary6.Add("중구", 5);
            subDictionary7.Add("전체", 0);
            subDictionary7.Add("광산구", 1);
            subDictionary7.Add("남구", 2);
            subDictionary7.Add("동구", 3);
            subDictionary7.Add("북구", 4);
            subDictionary7.Add("서구", 5);
            subDictionary8.Add("전체", 0);
            subDictionary8.Add("가평군", 1);
            subDictionary8.Add("고양시", 2);
            subDictionary8.Add("과천시", 3);
            subDictionary8.Add("광명시", 4);
            subDictionary8.Add("광주시", 5);
            subDictionary8.Add("구리시", 6);
            subDictionary8.Add("군포시", 7);
            subDictionary8.Add("김포시", 8);
            subDictionary8.Add("남양주시", 9);
            subDictionary8.Add("동두천시", 10);
            subDictionary8.Add("부천시", 11);
            subDictionary8.Add("성남시", 12);
            subDictionary8.Add("수원시", 13);
            subDictionary8.Add("시흥시", 14);
            subDictionary8.Add("안산시", 15);
            subDictionary8.Add("안성시", 16);
            subDictionary8.Add("안양시", 17);
            subDictionary8.Add("양주시", 18);
            subDictionary8.Add("양평군", 19);
            subDictionary8.Add("여주시", 20);
            subDictionary8.Add("연천군", 21);
            subDictionary8.Add("오산시", 22);
            subDictionary8.Add("용인시", 23);
            subDictionary8.Add("의왕시", 24);
            subDictionary8.Add("의정부시", 25);
            subDictionary8.Add("이천시", 26);
            subDictionary8.Add("파주시", 27);
            subDictionary8.Add("평택시", 28);
            subDictionary8.Add("포천시", 29);
            subDictionary8.Add("하남시", 30);
            subDictionary8.Add("화성시", 31);
            subDictionary9.Add("전체", 0);
            subDictionary9.Add("강릉시", 1);
            subDictionary9.Add("고성군", 2);
            subDictionary9.Add("동해시", 3);
            subDictionary9.Add("삼척시", 4);
            subDictionary9.Add("속초시", 5);
            subDictionary9.Add("양구군", 6);
            subDictionary9.Add("양양군", 7);
            subDictionary9.Add("영월군", 8);
            subDictionary9.Add("원주시", 9);
            subDictionary9.Add("인제군", 10);
            subDictionary9.Add("정선군", 11);
            subDictionary9.Add("철원군", 12);
            subDictionary9.Add("춘천시", 13);
            subDictionary9.Add("태백시", 14);
            subDictionary9.Add("평창군", 15);
            subDictionary9.Add("홍천군", 16);
            subDictionary9.Add("화천군", 17);
            subDictionary9.Add("횡성군", 18);
            subDictionary10.Add("전체", 0);
            subDictionary10.Add("괴산군", 1);
            subDictionary10.Add("단양군", 2);
            subDictionary10.Add("보은군", 3);
            subDictionary10.Add("영동군", 4);
            subDictionary10.Add("옥천군", 5);
            subDictionary10.Add("음성군", 6);
            subDictionary10.Add("제천시", 7);
            subDictionary10.Add("증평군", 8);
            subDictionary10.Add("진천군", 9);
            subDictionary10.Add("청주시", 10);
            subDictionary10.Add("충주시", 11);
            subDictionary11.Add("전체", 0);
            subDictionary11.Add("계룡시", 1);
            subDictionary11.Add("공주시", 2);
            subDictionary11.Add("금산군", 3);
            subDictionary11.Add("논산시", 4);
            subDictionary11.Add("당진시", 5);
            subDictionary11.Add("보령시", 6);
            subDictionary11.Add("부여군", 7);
            subDictionary11.Add("서산시", 8);
            subDictionary11.Add("서천군", 9);
            subDictionary11.Add("아산시", 10);
            subDictionary11.Add("예산군", 11);
            subDictionary11.Add("천안시", 12);
            subDictionary11.Add("청양군", 13);
            subDictionary11.Add("태안군", 14);
            subDictionary11.Add("홍성군", 15);
            subDictionary12.Add("전체", 0);
            subDictionary12.Add("경산시", 1);
            subDictionary12.Add("경주시", 2);
            subDictionary12.Add("고령군", 3);
            subDictionary12.Add("구미시", 4);
            subDictionary12.Add("군위군", 5);
            subDictionary12.Add("김천시", 6);
            subDictionary12.Add("문경시", 7);
            subDictionary12.Add("봉화군", 8);
            subDictionary12.Add("상주시", 9);
            subDictionary12.Add("성주군", 10);
            subDictionary12.Add("안동시", 11);
            subDictionary12.Add("영덕군", 12);
            subDictionary12.Add("영양군", 13);
            subDictionary12.Add("영주시", 14);
            subDictionary12.Add("영천시", 15);
            subDictionary12.Add("예천군", 16);
            subDictionary12.Add("울릉군", 17);
            subDictionary12.Add("울진군", 18);
            subDictionary12.Add("의성군", 19);
            subDictionary12.Add("청도군", 20);
            subDictionary12.Add("청송군", 21);
            subDictionary12.Add("칠곡군", 22);
            subDictionary12.Add("포항시", 23);
            subDictionary13.Add("전체", 0);
            subDictionary13.Add("거제시", 1);
            subDictionary13.Add("거창군", 2);
            subDictionary13.Add("고성군", 3);
            subDictionary13.Add("김해시", 4);
            subDictionary13.Add("남해군", 5);
            subDictionary13.Add("밀양시", 6);
            subDictionary13.Add("사천시", 7);
            subDictionary13.Add("산청군", 8);
            subDictionary13.Add("양산시", 9);
            subDictionary13.Add("의령군", 10);
            subDictionary13.Add("진주시", 11);
            subDictionary13.Add("창녕군", 12);
            subDictionary13.Add("창원시", 13);
            subDictionary13.Add("통영시", 14);
            subDictionary13.Add("하동군", 15);
            subDictionary13.Add("함안군", 16);
            subDictionary13.Add("함양군", 17);
            subDictionary13.Add("합천군", 18);
            subDictionary14.Add("전체", 0);
            subDictionary14.Add("고창군", 1);
            subDictionary14.Add("군산시", 2);
            subDictionary14.Add("김제시", 3);
            subDictionary14.Add("남원시", 4);
            subDictionary14.Add("무주군", 5);
            subDictionary14.Add("부안군", 6);
            subDictionary14.Add("순창군", 7);
            subDictionary14.Add("완주군", 8);
            subDictionary14.Add("익산시", 9);
            subDictionary14.Add("임실군", 10);
            subDictionary14.Add("장수군", 11);
            subDictionary14.Add("전주시", 12);
            subDictionary14.Add("정읍시", 13);
            subDictionary14.Add("진안군", 14);
            subDictionary15.Add("전체", 0);
            subDictionary15.Add("강진군", 1);
            subDictionary15.Add("고흥군", 2);
            subDictionary15.Add("곡성군", 3);
            subDictionary15.Add("광양시", 4);
            subDictionary15.Add("구례군", 5);
            subDictionary15.Add("나주시", 6);
            subDictionary15.Add("담양군", 7);
            subDictionary15.Add("목포시", 8);
            subDictionary15.Add("무안군", 9);
            subDictionary15.Add("보성군", 10);
            subDictionary15.Add("순천시", 11);
            subDictionary15.Add("신안군", 12);
            subDictionary15.Add("여수시", 13);
            subDictionary15.Add("영광군", 14);
            subDictionary15.Add("영암군", 15);
            subDictionary15.Add("완도군", 16);
            subDictionary15.Add("장성군", 17);
            subDictionary15.Add("장흥군", 18);
            subDictionary15.Add("진도군", 19);
            subDictionary15.Add("함평군", 20);
            subDictionary15.Add("해남군", 21);
            subDictionary15.Add("화순군", 22);
            subDictionary16.Add("전체", 0);
            subDictionary16.Add("서귀포시", 1);
            subDictionary16.Add("제주시", 2);
            subDictionary17.Add("전체", 0);
            TypeDictionary.Add("전체", false);
            TypeDictionary.Add("건어물", true);
            TypeDictionary.Add("마트", true);
            TypeDictionary.Add("문구", true);
            TypeDictionary.Add("미용", true);
            TypeDictionary.Add("병원", true);
            TypeDictionary.Add("부동산", true);
            TypeDictionary.Add("분식", true);
            TypeDictionary.Add("생화", true);
            TypeDictionary.Add("생활용품", true);
            TypeDictionary.Add("수산물", true);
            TypeDictionary.Add("슈퍼", true);
            TypeDictionary.Add("식육", true);
            TypeDictionary.Add("안경", true);
            TypeDictionary.Add("약국", true);
            TypeDictionary.Add("양식", true);
            TypeDictionary.Add("어린이집", true);
            TypeDictionary.Add("유통", true);
            TypeDictionary.Add("음식", true);
            TypeDictionary.Add("의류", true);
            TypeDictionary.Add("일식", true);
            TypeDictionary.Add("잡화", true);
            TypeDictionary.Add("전자상거래", true);
            TypeDictionary.Add("정비", true);
            TypeDictionary.Add("제과", true);
            TypeDictionary.Add("주류", true);
            TypeDictionary.Add("주유", true);
            TypeDictionary.Add("중식", true);
            TypeDictionary.Add("치과", true);
            TypeDictionary.Add("치킨", true);
            TypeDictionary.Add("카페", true);
            TypeDictionary.Add("커피", true);
            TypeDictionary.Add("편의점", true);
            TypeDictionary.Add("피자", true);
            TypeDictionary.Add("학원", true);
            TypeDictionary.Add("한식", true);
            TypeDictionary.Add("한의원", true);
            TypeDictionary.Add("호프", true);
            TypeDictionary.Add("화장품", true);
            TypeDictionary.Add("휴게음식", true);
        }
        
    }
}