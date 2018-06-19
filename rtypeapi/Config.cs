using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rtypeapi
{
    public class Config
    {
        public string url;// = "http://+";//"http://127.0.0.1";
        public string port;// = "5030";
        public string prefix;
        public string db_name;

        public static Config config;

        public static Config GetConfig()
        {
            if (config == null)
            {
                CreateConfig();
            }
            return config;
        }

        public static void CreateConfig()
        {
            string filename = @"./config_rtype.json";
            //Logger.Info("read config");

            config = new Config();
            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(json);
            }
            config.prefix = String.Format("{0}:{1}/", config.url, config.port);         
        }
    }
}
