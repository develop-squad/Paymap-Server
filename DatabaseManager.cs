using System;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;

namespace PAYMAP_BACKEND
{
    public class DatabaseManager
    {
        private static DatabaseManager _instance;

        public static int CountDatabaseAPI;
        
        public static bool IsModuleRunning = false;
        public static bool IsModuleHealthy = false;

        private Thread databaseThread;
        private MySqlConnection dbConnection;
        
        public bool CURRENT_DB_CONNECTION = false;
        private bool MODULE_STOP_FLAG;
        
        private DatabaseManager()
        {
            
        }

        public static DatabaseManager GetInstance()
        {
            return _instance ?? (_instance = new DatabaseManager());
        }
        
        public void StartDatabase()
		{
			MODULE_STOP_FLAG = false;
			CURRENT_DB_CONNECTION = true;
			//serverApplication.logManager.NewLog(LogManager.LOG_LEVEL.LOG_NORMAL, LogManager.LOG_TARGET.LOG_SYSTEM, "DatabaseManager", "DATABASE 시작");
			try
			{
				if (dbConnection == null || dbConnection.State == ConnectionState.Closed || dbConnection.State == ConnectionState.Broken || dbConnection.Ping() == false)
				{
					dbConnection = null;
					dbConnection = new MySqlConnection("SERVER=localhost;DATABASE=paymap;UID=" + Constants.DB_ACCOUNT + ";PASSWORD=" + Constants.DB_PASSWORD + ";Charset=utf8");
					dbConnection.Open();
				}
			}
			catch (Exception e)
			{
				dbConnection?.Close();
				dbConnection = null;
				//serverApplication.graphicalManager.OnDatabaseModuleStatusChanged(true, false);
				//serverApplication.logManager.NewLog(LogManager.LOG_LEVEL.LOG_CRITICAL, LogManager.LOG_TARGET.LOG_SYSTEM, "DatabaseManager", "DATABASE 연결 끊김");
				//serverApplication.logManager.NewLog(LogManager.LOG_LEVEL.LOG_CRITICAL, LogManager.LOG_TARGET.LOG_SYSTEM, "DatabaseManager", e.Message);
				return;
			}
			if (databaseThread == null || !databaseThread.IsAlive)
			{
				databaseThread = new Thread(DoDatabase);
                databaseThread.Priority = ThreadPriority.Lowest;
			}
			else if (databaseThread.IsAlive)
			{
				return;
			}
			databaseThread.Start();
		}

		public void StopDatabase()
		{
			MODULE_STOP_FLAG = true;
			CURRENT_DB_CONNECTION = false;
			if (dbConnection != null)
			{
				dbConnection.Close();
				dbConnection = null;
			}
		}

		private void DoDatabase()
		{
			//serverApplication.graphicalManager.OnDatabaseModuleStatusChanged(true, true);
			while (true)
			{
				if (MODULE_STOP_FLAG)
				{
					break;
				}

				CURRENT_DB_CONNECTION = true;
				using (dbConnection = new MySqlConnection("SERVER=localhost;DATABASE=paymap;UID=" + Constants.DB_ACCOUNT + ";PASSWORD=" + Constants.DB_PASSWORD + ";Charset=utf8"))
				{
					dbConnection.Open();
					if (dbConnection.Ping())
					{
						//serverApplication.graphicalManager.OnDatabaseModuleStatusChanged(true, true);
					}
					else
					{
						CURRENT_DB_CONNECTION = false;
						//serverApplication.graphicalManager.OnDatabaseModuleStatusChanged(true, false);
						//serverApplication.logManager.NewLog(LogManager.LOG_LEVEL.LOG_CRITICAL, LogManager.LOG_TARGET.LOG_SYSTEM, "DatabaseManager", "DATABASE 연결 끊김");
					}
				}
				
				try
				{
					Thread.Sleep(1000);
				} catch (Exception e)
				{
					//serverApplication.graphicalManager.OnDatabaseModuleStatusChanged(true, false);
					//serverApplication.logManager.NewLog(LogManager.LOG_LEVEL.LOG_CRITICAL, LogManager.LOG_TARGET.LOG_SYSTEM, "DatabaseManager", "DATABASE 에러");
					//serverApplication.logManager.NewLog(LogManager.LOG_LEVEL.LOG_CRITICAL, LogManager.LOG_TARGET.LOG_SYSTEM, "DatabaseManager", e.ToString());
					return;
				}
			}
			
			try
			{
				//serverApplication.logManager.NewLog(LogManager.LOG_LEVEL.LOG_NORMAL, LogManager.LOG_TARGET.LOG_SYSTEM, "DatabaseManager", "DATABASE 종료");
				//serverApplication.graphicalManager.OnDatabaseModuleStatusChanged(false, false);
			}
			catch { }
		}
        
    }
}