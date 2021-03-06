﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace rtypeapi
{
    class Listener
    {
        HttpListener listener;
        Config config;
        DataBase db;
        Web web;

        public Listener(DataBase db)
        {
            config = Config.GetConfig();
            web = new Web();
            this.db = db;
            listener = new HttpListener();
            listener.Prefixes.Add(config.prefix);
            listener.Start();
            Console.WriteLine("Port\n", config.port);
            Console.WriteLine("Listening on {0}", config.prefix);
        }

        public void Listen()
        {
            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;

                    string ip = "";                    
                    try
                    {
                        IEnumerable<string> headerValues = request.Headers.GetValues("X-Real-Ip");
                        ip = headerValues.FirstOrDefault();
                    }
                    catch
                    {
                        ip = request.RemoteEndPoint.ToString();
                        ip = ip.Split(':')[0];
                    }
                    string requesText = request.RawUrl;
                    string requestBody = GetRequestPostData(request);

                    if (requesText != "/favicon.ico")
                    {
                        int count = db.GetCountGameForIP(ip);
                        Console.WriteLine("------------------------------------------");
                        string msg = String.Format("time = {0}", DateTime.Now.ToString()) + "\n" +
                            String.Format("ip = {0}", ip) + "\n" +
                            String.Format("requesText = {0}", requesText) + "\n" +
                            String.Format("requestBody = {0}", requestBody);
                        if (requestBody.Contains("init"))
                        {
                            msg += "\n\n";
                            msg += count == 0 ? "NEW USER" : "count init = " + count.ToString();
                        }

                        Console.WriteLine(msg);

                        db.SaveRequestAndIP(requesText, requestBody, ip);

                        web.SendMessage(msg);
                    }

                    string result = "true";
                    
                    if (requestBody.Contains("init"))
                    {
                        result = db.CheckName(ip);
                    }
                    else if (requestBody.Contains("name"))
                    {
                        Dictionary<string, string> name = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);
                        result = db.SaveNameForIp(ip, name["name"].Trim());
                    }
                    
                    byte[] body = Encoding.UTF8.GetBytes("{\"result\": \"" + result + "\"}");
                    
                    var response = context.Response;                                       
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "application/json; charset=utf-8";
                    response.OutputStream.Write(body, 0, body.Length);                    
                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                }
            }
        }

        public static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            using (Stream body = request.InputStream)
            {
                using (StreamReader reader = new StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}