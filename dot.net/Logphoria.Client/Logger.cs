using System;
using System.Diagnostics;
using System.Threading;
using System.Web.Script.Serialization;
using Logphoria.Driver.Components;
using Logphoria.Driver.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Logphoria.Driver
{
    public class Logger : ILogger, IRequestContext
    {
        private readonly string _url;
        private readonly string _inputKey;

        public Logger(string url, string inputKey)
        {
            _url = url;
            _inputKey = inputKey;
        }

        public LogResponse Log(string message)
        {
            Trace.WriteLine("Log");
            var synchronizer = new AutoResetEvent(false);
            LogResponse response = null;
            LogAsync(message, r =>
            {
                response = r;
                synchronizer.Set();
            });
            synchronizer.WaitOne();
            return response;

        }

        public void LogAsync(string message)
        {
            LogAsync(message, null);
        }

        public LogResponse Log(object obj)
        {
            var message = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            return Log(message);
        }

        public void LogAsync(string message, Action<LogResponse> callback)
        {
            var communicator = new Communicator(this);
            var callbackWrapper = callback == null ? (Action<Response>)null : r =>
            {
                if (r.Success)
                {
                    var js = new JavaScriptSerializer();
                    callback(js.Deserialize<LogResponse>(r.Raw));
                }
            };

            try{
                communicator.SendPayload(Communicator.POST, string.Format(Routes.Log, _inputKey), message, callbackWrapper);
            }catch(Exception ex)
            {
                // yes I know it's bad)
            }
        }

        public string Url
        {
            get { return _url; }
        }
    }
}