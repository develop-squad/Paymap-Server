using System;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using PAYMAP_BACKEND.Data;

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
			LogManager.NewLog(LogType.DatabaseManager, LogLevel.Info, "StartDatabase", "Database Module Started");
			try
			{
				if (dbConnection == null || dbConnection.State == ConnectionState.Closed || dbConnection.State == ConnectionState.Broken || dbConnection.Ping() == false)
				{
					dbConnection = null;
					dbConnection = new MySqlConnection("SERVER=devx.kr;DATABASE=paymap;UID=" + Constants.DB_ACCOUNT + ";PASSWORD=" + Constants.DB_PASSWORD + ";Charset=utf8");
					dbConnection.Open();
				}
			}
			catch (Exception e)
			{
				dbConnection?.Close();
				dbConnection = null;
				//serverApplication.graphicalManager.OnDatabaseModuleStatusChanged(true, false);
				LogManager.NewLog(LogType.DatabaseManager, LogLevel.Error, "StartDatabase", e);
				return;
			}
			/*
			if (databaseThread == null || !databaseThread.IsAlive)
			{
				databaseThread = new Thread(DoDatabase)
				{
					Priority = ThreadPriority.Lowest
				};
			}
			else if (databaseThread.IsAlive)
			{
				return;
			}
			databaseThread.Start();
			*/
		}

		public void StopDatabase()
		{
			MODULE_STOP_FLAG = true;
			CURRENT_DB_CONNECTION = false;
			LogManager.NewLog(LogType.DatabaseManager, LogLevel.Info, "StopDatabase", "Database Module Stopped");
			if (dbConnection != null)
			{
				dbConnection.Close();
				dbConnection = null;
			}
		}

		private void DoDatabase()
		{
			//serverApplication.graphicalManager.OnDatabaseModuleStatusChanged(true, true);
			IsModuleRunning = true;
			while (true)
			{
				if (MODULE_STOP_FLAG)
				{
					break;
				}

				CURRENT_DB_CONNECTION = true;
				using (dbConnection = new MySqlConnection("SERVER=devx.kr;DATABASE=paymap;UID=" + Constants.DB_ACCOUNT + ";PASSWORD=" + Constants.DB_PASSWORD + ";Charset=utf8"))
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
						LogManager.NewLog(LogType.DatabaseManager, LogLevel.Warn, "DoDatabase", "Database Ping Failed");
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
			
			IsModuleRunning = false;
			
			try
			{
				LogManager.NewLog(LogType.DatabaseManager, LogLevel.Info, "DoDatabase", "Database Thread Finished");
				//serverApplication.graphicalManager.OnDatabaseModuleStatusChanged(false, false);
			}
			catch (Exception e)
			{
				return;
			}
		}

		public bool InsertShop(CrawlData crawlData)
		{
			Shop shop = new Shop(crawlData);
			return InsertShop(shop);
		}
		
		public bool InsertShop(Shop shop)
		{
			try
			{
				lock (dbConnection)
				{
					using (dbConnection = new MySqlConnection("SERVER=devx.kr;DATABASE=paymap;UID=" + Constants.DB_ACCOUNT + ";PASSWORD=" + Constants.DB_PASSWORD + ";Charset=utf8"))
					{
						dbConnection.Open();
						using (MySqlCommand shopInsertCommand = dbConnection.CreateCommand())
						{
							string name = MySqlHelper.EscapeString(shop.Name);
							string address = MySqlHelper.EscapeString(shop.Address);
							string type = MySqlHelper.EscapeString(shop.Type);
							shopInsertCommand.CommandText = $"INSERT INTO `shop` (`shop_name`, `shop_sido`, `shop_sigungu`, `shop_address`, `shop_type`) VALUES ('{name}', '{shop.Sido}', '{shop.Sigungu}', '{address}', '{type}')";
							LogManager.NewLog(LogType.DatabaseManager, LogLevel.Info, "InsertShop", "Data (" + shop.Name + ") Inserted to DB");
							int inserted = shopInsertCommand.ExecuteNonQuery();
							if (inserted != 1)
							{
								return false;
							}
						}
					}
				}
				return true;
			}
			catch (Exception e)
			{
				LogManager.NewLog(LogType.DatabaseManager, LogLevel.Error, "InsertShop", e);
				return false;
			}
		}
        
    }
}