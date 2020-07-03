using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;

namespace PAYMAP_BACKEND
{
    public class ServerManager
    {
        private static ServerManager _instance;

        public static bool IsModuleRunning = false;
        public static bool IsModuleHealthy = false;

        private WebServer webServerGeocode;
        private WebServer webServerZeroPay;
        
        private ServerManager()
        {
            
        }

        public static ServerManager GetInstance()
        {
            return _instance ?? (_instance = new ServerManager());
        }

        public void StartWebServer()
        {
           webServerGeocode = new WebServer(GeocodeResponse, "http://devx.kr:9991/geocode/");
           webServerGeocode.Run();
           webServerZeroPay = new WebServer(ZeroPayResponse, "http://devx.kr:9991/zeropay/");
           webServerZeroPay.Run();
        }

        public void StopWebServer()
        {
           webServerGeocode?.Stop();
           webServerZeroPay?.Stop();
        }
        
        public static string GeocodeResponse(HttpListenerRequest request)
        {
           NameValueCollection queryCollection = HttpUtility.ParseQueryString(request.Url.Query);
           string inputId = queryCollection.Get("id");
           if (inputId == null || inputId.Equals(String.Empty)) inputId = Constants.NAVER_SB_ID;
           string inputKey = queryCollection.Get("key");
           if (inputKey == null || inputKey.Equals(String.Empty)) inputKey = Constants.NAVER_SB_KEY;
           string[] inputAddressArray;
           string inputAddress = queryCollection.Get("address");
           if (inputAddress == null || inputAddress.Equals(String.Empty)) inputAddress = "";
           inputAddressArray = inputAddress.Split('|');
           string inputCoordinate = queryCollection.Get("coordinate");
           if (inputCoordinate == null || inputCoordinate.Equals(String.Empty)) inputCoordinate = "";

           string[] headerId;
           string[] headerKey;
           string[] headerAddress;
           string[] headerCoordinate;
           headerId = request.Headers.GetValues("id");
           if (headerId != null && headerId.Length > 0)
           {
              inputId = headerId[0];
           }
           headerKey = request.Headers.GetValues("key");
           if (headerKey != null && headerKey.Length > 0)
           {
              inputKey = headerKey[0];
           }
           headerAddress = request.Headers.GetValues("address");
           if (headerAddress != null && headerAddress.Length > 0)
           {
              inputAddressArray = headerAddress;
           }
           headerCoordinate = request.Headers.GetValues("coordinate");
           if (headerCoordinate != null && headerCoordinate.Length > 0)
           {
              inputCoordinate = headerCoordinate[0];
           }

           JArray naverGeocodeResultArray = new JArray();
           WebClient naverGeocodeClient = new WebClient {Encoding = Encoding.UTF8};
           naverGeocodeClient.Headers.Add("X-NCP-APIGW-API-KEY-ID", inputId);
           naverGeocodeClient.Headers.Add("X-NCP-APIGW-API-KEY", inputKey);

           for (int i = 0; i < inputAddressArray.Length; i ++)
           {
              string requestURL = $"https://naveropenapi.apigw.ntruss.com/map-geocode/v2/geocode?query={inputAddressArray[i]}&coordinate={inputCoordinate}";
              JObject resultObject = JObject.Parse(naverGeocodeClient.DownloadString(requestURL));
              naverGeocodeResultArray.Add(resultObject);
           }

           return naverGeocodeResultArray.ToString();
        }
        
        public static string ZeroPayResponse(HttpListenerRequest request)
        {
           NameValueCollection queryCollection = HttpUtility.ParseQueryString(request.Url.Query);
           string inputSido = queryCollection.Get("sido");
           if (inputSido == null || inputSido.Equals(String.Empty)) inputSido = "0";
           string inputSigungu = queryCollection.Get("sigungu");
           if (inputSigungu == null || inputSigungu.Equals(String.Empty)) inputSigungu = "0";
           string inputAddress = queryCollection.Get("address");
           string inputName = queryCollection.Get("name");
           string inputType = queryCollection.Get("type");
           string inputMax = queryCollection.Get("max");
           if (inputMax == null || inputMax.Equals(String.Empty)) inputMax = "1000";

           JArray zeroResult = DatabaseManager.GetInstance().SearchShop(int.Parse(inputSido), int.Parse(inputSigungu), inputAddress, inputName, inputType, int.Parse(inputMax));
           
           return zeroResult.ToString();
        }

    }
    
    public class WebServer
   {
      private readonly HttpListener _listener = new HttpListener();
      private readonly Func<HttpListenerRequest, string> _responderMethod;
 
      public WebServer(IReadOnlyCollection<string> prefixes, Func<HttpListenerRequest, string> method)
      {
         if (!HttpListener.IsSupported)
         {
            throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");
         }
             
         if (prefixes == null || prefixes.Count == 0)
         {
            throw new ArgumentException("URI prefixes are required");
         }
         
         if (method == null)
         {
            throw new ArgumentException("responder method required");
         }
 
         foreach (var s in prefixes)
         {
            _listener.Prefixes.Add(s);
         }
 
         _responderMethod = method;
         _listener.Start();
      }
 
      public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
         : this(prefixes, method)
      {
      }
 
      public void Run()
      {
         ThreadPool.QueueUserWorkItem(o =>
         {
            LogManager.NewLog(LogType.ServerManager, LogLevel.Info, "WebServer:Run", "WebServer Started");
            try
            {
               while (_listener.IsListening)
               {
                  ThreadPool.QueueUserWorkItem(c =>
                  {
                     var ctx = c as HttpListenerContext;
                     try
                     {
                        if (ctx == null)
                        {
                           return;
                        }
                        
                        var rstr = _responderMethod(ctx.Request);
                        var buf = Encoding.UTF8.GetBytes(rstr);
                        ctx.Response.Headers["Access-Control-Allow-Origin"] = "*";
                        ctx.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
                        ctx.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS";
                        ctx.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                        ctx.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
                        ctx.Response.ContentLength64 = buf.Length;
                        ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                        ctx.Response.ContentEncoding = Encoding.UTF8;
                     }
                     catch
                     {
                        // ignored
                     }
                     finally
                     {
                        // always close the stream
                        if (ctx != null)
                        {
                           ctx.Response.OutputStream.Close();
                        }
                     }
                  }, _listener.GetContext());
               }
            }
            catch (Exception ex)
            {
               // ignored
            }
         });
      }
 
      public void Stop()
      {
         _listener.Stop();
         _listener.Close();
      }
   }
}