using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace rtypeapi
{
    class Listener
    {
        HttpListener listener;
        Config config;
        DataBase dataBase;

        public Listener()
        {
            config = Config.GetConfig();
            dataBase = new DataBase();
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
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                ShowRequestData(request);

                var ip = request.RemoteEndPoint.ToString();
                string requesText = request.RawUrl;

                string requestBody = GetRequestPostData(request);

                Console.WriteLine("ip = {0}", ip);

                dataBase.SaveRequestAndIP(requesText, requestBody, ip);

                var response = context.Response;

                Console.WriteLine("StatusCode - {0}", HttpStatusCode.OK.ToString());
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/json; charset=utf-8";

                //Console.WriteLine("start select json");
                //string json = Answers.GetJson(lastId);
                var body = Encoding.UTF8.GetBytes("200");
                //Console.WriteLine("end select json");

                response.OutputStream.Write(body, 0, body.Length);
                Console.WriteLine("flush");
                response.OutputStream.Flush();
                Console.WriteLine("close");
                response.OutputStream.Close();

                //Console.WriteLine("{0} request was caught: {1}", request.HttpMethod, request.Url);
            }
        }

        public static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            using (System.IO.Stream body = request.InputStream) // here we have data
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static void ShowRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return;
            }

            Stream body = request.InputStream;
            Encoding encoding = request.ContentEncoding;

            using (StreamReader reader = new StreamReader(body, encoding))
            {
                if (request.ContentType != null)
                {
                    Console.WriteLine("Client data content type {0}", request.ContentType);
                }

                Console.WriteLine("Client data content length {0}", request.ContentLength64);
                Console.WriteLine("Start of client data:");

                string s = reader.ReadToEnd();

                Console.WriteLine(s);
                Console.WriteLine("End of client data:");
                body.Close();
            }
        }
    }
}