using System;
using System.Net.Http;

namespace rtypeapi
{
    class Web
    {
        static HttpClient client;
        Config config;

        static Web()
        {            
            client = new HttpClient();
        }
        public Web()
        {
            config = Config.GetConfig();
        }

        public static string DownloadData(string url)
        {
            string data = "";
            try
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        data = content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception e)
            {
                data = "";
                Logger.Error("Error: {0}", e.Message);
            }
            return data;
        }

        public bool SendMessage(string text)
        {
            foreach (long id in config.ids)
            {
                try
                {
                    string url = config.host + "sendMessage?chat_id=" + id.ToString() + "&text=" + text;
                    DownloadData(url);
                }
                catch
                {

                }
            }
            return true;          
        }
    }
}