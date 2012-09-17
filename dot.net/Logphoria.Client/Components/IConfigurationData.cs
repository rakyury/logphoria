using System.Net;

namespace Logphoria.Driver.Components
{
    public interface IConfigurationData
    {
        int Timeout { get; }
        bool Https { get; }
        string ForcedUrl { get; }
        NetworkCredential Credentials { get; }
    }

    internal class ConfigurationData : IConfigurationData
    {
        private bool _https = false;
        private int _timeout = 10000;

        public NetworkCredential Credentials { get; set; }

        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public bool Https
        {
            get { return _https; }
            set { _https = value; }
        }

        public string ForcedUrl { get; set; }
    }
}