using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Logphoria.Driver
{
    public class JsonSerializer
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
        }
    }
}