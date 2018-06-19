using System;
using System.Collections;
using System.Data.SQLite;

namespace rtypeapi
{
    internal class DataBase
    {
        private SQLiteConnection connection;
        Config config;

        internal DataBase()
        {
            config = Config.GetConfig();
            connection = new SQLiteConnection(string.Format("Data Source={0};", config.db_name));
            //Logger.Debug("create connection");
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
                "select @request_text, @body, @ip, datetime('now')", parameters, null);

            return result;
        }
    }
}