using System;
using System.Collections;
using System.Data.SQLite;

namespace rtypeapi
{
    public class DataBase
    {
        private SQLiteConnection connection;
        Config config;

        public DataBase()
        {
            config = Config.GetConfig();
            connection = new SQLiteConnection(string.Format("Data Source={0};", config.db_name));
            //Logger.Debug("create connection");
        }

        public int GetCountGameForIP(string ip)
        {
            int count = 0;
            Hashtable parameters = new Hashtable
            {
                { "@ip", ip },
            };
            string cmd = "select count(*) from request where body like '%init%' and ip = @ip";
          
            UniversalQuery(cmd, parameters, (SQLiteDataReader reader) =>
            {
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
            });

            return count;
        }

        public int GetCountOnlineUsers()
        {
            int count = 0;
            string date = DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss");

            string cmd = @"SELECT count (distinct ip) FROM request where body not like '%start_game%' and body not like '%end_game%' and body not like '%new_game%' and body not like '%init%' and body not null and (datetime(""date"") >= datetime('" + date + "'))";

            UniversalQuery(cmd, null, (SQLiteDataReader reader) =>
                {
                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                });

            return count;
        }

        private bool UniversalQuery(string sql, Hashtable parameters, Action<SQLiteDataReader> processor)
        {
            bool result = false;
            //Logger.Info("connect to base");
            //Logger.Info(sql);
            try
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);

                if (parameters != null)
                {
                    var parametersName = parameters.Keys;
                    foreach (var name in parametersName)
                    {
                        cmd.Parameters.AddWithValue(name.ToString(), parameters[name]);
                    }
                }

                if (processor == null)
                {
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        processor(reader);
                    }
                }
                result = true;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                result = false;
            }
            finally
            {
                connection.Close();                
            }

            return result;
        }

        public bool SaveRequestAndIP(string request, string body, string ip)
        {
            //Logger.Info("save request, ip in db");
            Hashtable issues = new Hashtable();
            Hashtable parameters = new Hashtable
            {
                { "@request_text", request },
                { "@ip", ip },
                { "@body", body }
            };

            bool result = UniversalQuery("insert into request (request_text, body, ip, date) " +
                "select @request_text, @body, @ip, datetime('now', 'localtime')", parameters, null);

            return result;
        }
    }
}