namespace PAYMAP_BACKEND
{
    public class DatabaseManager
    {
        private static DatabaseManager _instance;

        public static int CountDatabaseAPI;
        
        public static bool IsModuleRunning = false;
        public static bool IsModuleHealthy = false;
        
        private DatabaseManager()
        {
            
        }

        public static DatabaseManager GetInstance()
        {
            return _instance ?? (_instance = new DatabaseManager());
        }


    }
}