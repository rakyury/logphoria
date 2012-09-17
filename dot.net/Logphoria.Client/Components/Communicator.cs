using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Logphoria.Driver.Responses;

namespace Logphoria.Driver.Components
{
    public class Communicator
    {
        public const string POST = "POST";
        public const string GET = "GET";

        private readonly IRequestContext _context;

        public Communicator(IRequestContext context)
        {
            _context = context;
        }

        public void SendPayload(string method, string endPoint, string message, Action<Response> callback)
        {
            var request = CreateRequest(method, endPoint, false);
            var state = new RequestState { Request = request, Payload = message == null ? null : Encoding.UTF8.GetBytes(message), Callback = callback };
            try
            {
                request.BeginGetRequestStream(GetRequestStream, state);
            }catch(Exception ex)
            {
                
            }
        }

        public T GetPayload<T>(string endPoint, IDictionary<string, object> parameters)
        {
            var pathAndQuery = BuildPathAndQuery(endPoint, parameters);
            var request = CreateRequest(GET, pathAndQuery, true);
            try
            {
                using (var response = request.GetResponse())
                {
                    var js = new JavaScriptSerializer();
                    return js.Deserialize<T>(GetResponseBody(response));
                }
            }
            catch (WebException ex)
            {
                throw new LogphoriaException(GetResponseBody(ex.Response), ex);
            }
            catch (Exception ex)
            {
                throw new LogphoriaException(ex.Message, ex);
            }

        }

        private static string BuildPathAndQuery(string endPoint, IDictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return endPoint;
            }
            var sb = new StringBuilder(100);
            sb.Append(endPoint);
            sb.Append('?');
            foreach (var kvp in parameters)
            {
                if (kvp.Value == null) { continue; }
                sb.Append(kvp.Key);
                sb.Append('=');
                sb.Append(HttpUtility.UrlEncode(kvp.Value.ToString()));
                sb.Append("&");
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        private HttpWebRequest CreateRequest(string method, string endPoint, bool withCredentials)
        {
            var data = LogphoriaConfiguration.Data;
            var url = _context.Url;
            var request = (HttpWebRequest)WebRequest.Create(string.Concat(url, "/", endPoint));
            request.Method = method;
            request.Timeout = data.Timeout;
            request.ReadWriteTimeout = data.Timeout;
            request.UserAgent = "logphoria-csharp";
            request.KeepAlive = false;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            if (withCredentials) { request.Credentials = data.Credentials; }
            return request;
        }

        private static void GetRequestStream(IAsyncResult result)
        {
            var state = (RequestState)result.AsyncState;
            if (state.Payload != null)
            {
                try
                {
                    using (var requestStream = state.Request.EndGetRequestStream(result))
                    {
                        requestStream.Write(state.Payload, 0, state.Payload.Length);
                        requestStream.Flush();
                        requestStream.Close();
                    }
                }
                catch (Exception ex)
                {

                    if (state.Callback != null)
                    {
                        state.Callback(Response.CreateError(HandleException(ex)));
                        return;
                    }
                }
                
            }
            state.Request.BeginGetResponse(GetResponseStream, state);
        }

        private static void GetResponseStream(IAsyncResult result)
        {
            var state = (ResponseState)result.AsyncState;
            try
            {
                using (var response = (HttpWebResponse)state.Request.EndGetResponse(result))
                {
                    if (state.Callback != null)
                    {
                        state.Callback(Response.CreateSuccess(GetResponseBody(response)));
                    }
                }
            }
            catch (Exception ex)
            {
                if (state.Callback != null)
                {
                    state.Callback(Response.CreateError(HandleException(ex)));
                }
            }
        }

        private static string GetResponseBody(WebResponse response)
        {
            if (response == null) { return null; }
            using (var stream = response.GetResponseStream())
            {
                var sb = new StringBuilder();
                var read = 0;
                do
                {
                    var buffer = new byte[2048];
                    read = stream.Read(buffer, 0, buffer.Length);
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, read));
                } while (read > 0);
                return sb.ToString();
            }
        }
        private static ErrorMessage HandleException(Exception exception)
        {
            if (exception is WebException)
            {
                var body = GetResponseBody(((WebException)exception).Response);
                return new ErrorMessage { Error = body, InnerException = exception };
            }
            return new ErrorMessage { Error = "Unknown Error", InnerException = exception };
        }

        #region Nested type: RequestState

        private class RequestState : ResponseState
        {
            public byte[] Payload { get; set; }
        }

        #endregion

        #region Nested type: ResponseState

        private class ResponseState
        {
            public HttpWebRequest Request { get; set; }
            public Action<Response> Callback { get; set; }
        }

        #endregion
    }
}