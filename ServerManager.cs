namespace PAYMAP_BACKEND
{
    public class ServerManager
    {
        private static ServerManager _instance;

        public static bool IsModuleRunning = false;
        public static bool IsModuleHealthy = false;
        
        private ServerManager()
        {
            
        }

        public static ServerManager GetInstance()
        {
            return _instance ?? (_instance = new ServerManager());
        }

    }
}