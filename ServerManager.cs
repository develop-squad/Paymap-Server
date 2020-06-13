using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

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
           webServerGeocode = new WebServer(GeocodeResponse, "http://localhost:9991/geocode/");
           webServerGeocode.Run();
           webServerZeroPay = new WebServer(ZeroPayResponse, "http://localhost:9991/zeropay/");
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
           if (inputId == null || inputId.Equals(String.Empty)) inputId = "i0xvdqsm01";
           string inputKey = queryCollection.Get("key");
           if (inputKey == null || inputKey.Equals(String.Empty)) inputKey = "SE6qmbIMlBhDIG8HhzdLqL0e4OfxFtqSB3NrUnvM";
           string inputAddress = queryCollection.Get("address");
           string inputCoordinate = queryCollection.Get("coordinate");
           string requestURL = $"https://naveropenapi.apigw.ntruss.com/map-geocode/v2/geocode?query={inputAddress}&coordinate={inputCoordinate}";
           WebClient naverGeocodeClient = new WebClient {Encoding = Encoding.UTF8};
           naverGeocodeClient.Headers.Add("X-NCP-APIGW-API-KEY-ID", inputId);
           naverGeocodeClient.Headers.Add("X-NCP-APIGW-API-KEY", inputKey);
           String naverGeocodeResult = naverGeocodeClient.DownloadString(requestURL);
           return naverGeocodeResult;
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
           
           return "";
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
                        var buf = Encoding.Default.GetBytes(rstr);
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