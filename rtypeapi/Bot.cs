using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace rtypeapi
{
    public class Bot
    {
        private Config config;
        private bool getUpdate;
        private BackgroundWorker backgroundWorker;
        DataBase db;
        Statistics statistics;

        public Bot(DataBase dataBase)
        {
            config = Config.GetConfig();
            db = dataBase;
            statistics = new Statistics(db);
            backgroundWorker = new BackgroundWorker();
        }

        public void StartGettingUpdates()
        {
            getUpdate = true;
            backgroundWorker.DoWork += GetUpdates;
            backgroundWorker.RunWorkerAsync();
            Logger.Info("Start bot");
        }

        private void GetUpdates(object sender, DoWorkEventArgs e)
        {
            long offset = 0;
            string json = "";
            Update update = null;
            bool res = false;
            string url = config.host + "getUpdates?offset=";
            while (getUpdate)
            {
                json = Web.DownloadData(url + offset.ToString());
                update = JsonConvert.DeserializeObject<Update>(json);
                foreach (Result result in update.result)
                {
                    res = ProcessingResult(result);
                    offset = result.update_id + 1;
                }
            }
        }

        private bool ProcessingResult(Result result)
        {
            bool res = false;
            Message message = result.message;
            if (message != null)
            {
                if (message.chat.Type == ChatType.Private && message.text == "/online")
                {
                    Logger.Info("received a request for the number of online users");
                    int count = statistics.GetCountOnlineUsers();

                    Message msg = new Message
                    {
                        chat = new Chat { id = message.chat.id },
                        text = "count online users = " + count.ToString()
                    };
                    SendMessage(msg);
                }
            }
            else
            {
                res = true;
            }

            return res;
        }

        private bool SendMessage(Message message, string parseMode = null)
        {
            try
            {
                string url = config.host + "sendMessage?chat_id=" + message.chat.id + "&text=" + message.text;
                if (!String.IsNullOrEmpty(parseMode))
                {
                    url += "&parse_mode=" + parseMode;
                }
                string json = Web.DownloadData(url);
                //Update update = JsonConvert.DeserializeObject<Update>(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void FinishGettingUpdates()
        {
            getUpdate = false;
            backgroundWorker.CancelAsync();
            backgroundWorker.Dispose();
        }
    }
}