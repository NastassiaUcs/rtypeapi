using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace rtypeapi
{
    internal static class Logger
    {
        internal const string TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        internal const string DATE_FORMAT = "yyyy_MM_dd";
        private const string PATH_FILE_LOG = "./log/";
        private const string FILE_LOG_NAME = "log_name_{0}.txt";

        internal static void Info(string text, params object[] options)
        {
            WriteToLog(text, ConsoleColor.White, "info", options);
        }

        internal static void Success(string text, params object[] options)
        {
            WriteToLog(text, ConsoleColor.DarkGreen, "success", options);
        }

        internal static void Error(string text, params object[] options)
        {
            StackTrace stackTrace = new StackTrace();
            WriteToLog(text, ConsoleColor.DarkRed, "error][" + stackTrace.GetFrame(1).GetMethod().Name, options);
        }

        internal static void Warn(string text, params object[] options)
        {
            WriteToLog(text, ConsoleColor.White, "warn", options);
        }

        internal static void Debug(string text, params object[] options)
        {
            WriteToLog(text, ConsoleColor.Cyan, "debug", options);
        }

        private static void WriteToLog(string textLog, ConsoleColor color, string methodName, params object[] options)
        {
            string text = CreateTextForLog(textLog, options);

            if (!String.IsNullOrEmpty(text))
            {
                DateTime currentDate = DateTime.Now;
                string currentDateTimeString = currentDate.ToString(TIME_FORMAT);
                string logMessage = String.Format("{0} [{1}] {2}", currentDateTimeString, methodName, text);

                Console.ForegroundColor = color;
                Console.WriteLine(logMessage);
                WriteLogToFile(logMessage, currentDate);
                Console.ResetColor();
            }
            else
            {
                return;
            }
        }

        private static string CreateTextForLog(string textLog, params object[] options)
        {
            string text = "";
            if (!String.IsNullOrEmpty(textLog))
            {
                try
                {
                    if (options.Length > 0)
                        text = String.Format(textLog, options);
                    else
                        text = textLog;
                }
                catch
                {
                    text = textLog + " ";
                    foreach (string s in options)
                    {
                        text += options;
                    }
                }
                return text;
            }
            else
            {
                return null;
            }
        }

        private static void WriteLogToFile(string logText, DateTime currentDateTime)
        {
            string currentDateString = currentDateTime.ToString(DATE_FORMAT);
            string logFile = String.Format(PATH_FILE_LOG + FILE_LOG_NAME, currentDateString);
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            byte[] logData = unicodeEncoding.GetBytes(String.Format("{0}{1}", logText, Environment.NewLine));

            try
            {
                using (FileStream logStream = new FileStream(logFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    logStream.Seek(0, SeekOrigin.End);
                    logStream.Write(logData, 0, logData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}