namespace PAYMAP_BACKEND
{
    public class CrawlManager
    {
        private static CrawlManager _instance;

        public static bool IsModuleRunning = false;
        public static bool IsModuleHealthy = false;
        
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
            public LogType Result { get; set; }
        }
        
        private CrawlManager()
        {
            
        }

        public static CrawlManager GetInstance()
        {
            return _instance ?? (_instance = new CrawlManager());
        }

    }
}