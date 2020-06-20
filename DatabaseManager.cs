using System;
using System.Collections;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
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
		
		public JArray SearchShop(int sido, int sigungu, string _address, string _name, string _type, int max)
		{
			try
			{
				lock (dbConnection)
				{
					using (dbConnection = new MySqlConnection("SERVER=devx.kr;DATABASE=paymap;UID=" + Constants.DB_ACCOUNT + ";PASSWORD=" + Constants.DB_PASSWORD + ";Charset=utf8"))
					{
						dbConnection.Open();
						using (MySqlCommand shopSearchCommand = dbConnection.CreateCommand())
						{
							if (max > 1000) max = 1000;
							string address = "", name = "", type = "";
							JArray shopArray = new JArray();
							if (!string.IsNullOrEmpty(_address)) address = MySqlHelper.EscapeString(_address);
							if (!string.IsNullOrEmpty(_name)) name = MySqlHelper.EscapeString(_name);
							if (!string.IsNullOrEmpty(_type)) type = MySqlHelper.EscapeString(_type);
							shopSearchCommand.CommandText = $"SELECT * FROM `shop` WHERE `shop_sido` = {sido} AND `shop_sigungu` = {sigungu} AND `shop_address` LIKE '{address}%' AND `shop_name` LIKE '%{name}%' AND `shop_type` LIKE '%{type}%' LIMIT {max}";
							using (MySqlDataReader dataReader = shopSearchCommand.ExecuteReader())
							{
								while (dataReader.Read())
								{
									JObject shopObject = new JObject();
									int shop_index = dataReader.GetInt32(dataReader.GetOrdinal("shop_index"));
									string shop_name = dataReader.GetString(dataReader.GetOrdinal("shop_name"));
									int shop_sido = dataReader.GetInt32(dataReader.GetOrdinal("shop_sido"));
									int shop_sigungu = dataReader.GetInt32(dataReader.GetOrdinal("shop_sigungu"));
									string shop_address = dataReader.GetString(dataReader.GetOrdinal("shop_address"));
									string shop_type = dataReader.GetString(dataReader.GetOrdinal("shop_type"));
									shopObject.Add("shop_index", shop_index);
									shopObject.Add("shop_name", shop_name);
									shopObject.Add("shop_sido", shop_sido);
									shopObject.Add("shop_sigungu", shop_sigungu);
									shopObject.Add("shop_address", shop_address);
									shopObject.Add("shop_type", shop_type);
									shopArray.Add(shopObject);
								}
								dataReader.Close();
							}

							return shopArray;
						}
					}
				}
				return null;
			}
			catch (Exception e)
			{
				LogManager.NewLog(LogType.DatabaseManager, LogLevel.Error, "SearchShop", e);
				return null;
			}
		}
        
    }
}