namespace PAYMAP_BACKEND
{
    public class NetworkManager
    {
        private static NetworkManager _instance;

        public static bool IsModuleRunning = false;
        public static bool IsModuleHealthy = false;
        
        public static int CountNetworkAPI;

        private NetworkManager()
        {
            
        }

        public static NetworkManager GetInstance()
        {
            return _instance ?? (_instance = new NetworkManager());
        }
    }
}