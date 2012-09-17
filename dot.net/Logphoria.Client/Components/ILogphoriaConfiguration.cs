using System;
using System.Net;

namespace Logphoria.Driver.Components
{
    public interface ILogphoriaConfiguration
    {
        ILogphoriaConfiguration WithTimeout(int timeout);
        ILogphoriaConfiguration WithTimeout(TimeSpan timeout);
        ILogphoriaConfiguration DontUseHttps();
        ILogphoriaConfiguration AuthenticateWith(string username, string password);

        /// <summary>
        /// Used by the test library
        /// </summary>      
        ILogphoriaConfiguration ForceUrlTo(string url);
    }

    public class LogphoriaConfiguration : ILogphoriaConfiguration
    {
        private static ConfigurationData _data = new ConfigurationData();
        private static readonly LogphoriaConfiguration Configuration = new LogphoriaConfiguration();

        protected LogphoriaConfiguration()
        {
        }

        public static IConfigurationData Data
        {
            get { return _data; }
        }

        public ILogphoriaConfiguration WithTimeout(int timeout)
        {
            _data.Timeout = timeout;
            return this;
        }

        public ILogphoriaConfiguration WithTimeout(TimeSpan timeout)
        {
            return WithTimeout((int)timeout.TotalMilliseconds);
        }

        public ILogphoriaConfiguration DontUseHttps()
        {
            _data.Https = false;
            return this;
        }

        public ILogphoriaConfiguration AuthenticateWith(string username, string password)
        {
            _data.Credentials = new NetworkCredential(username, password);
            return this;
        }

        /// <summary>
        /// Used by the test library
        /// </summary>
        public ILogphoriaConfiguration ForceUrlTo(string url)
        {
            _data.ForcedUrl = url;
            return this;
        }

        public static void Configure(Action<ILogphoriaConfiguration> action)
        {
            action(Configuration);
        }

        /// <summary>
        /// Used by the test library
        /// </summary>
        public static void ResetToDefaults()
        {
            _data = new ConfigurationData();
        }
    }
}