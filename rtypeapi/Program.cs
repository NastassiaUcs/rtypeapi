using System;

namespace rtypeapi
{
    class Program
    {
        static Listener listener;

        static void Main(string[] args)
        {
            DataBase dataBase = new DataBase();            
            listener = new Listener(dataBase);
            Bot bot = new Bot(dataBase);
            bot.StartGettingUpdates();
            listener.Listen();            
            
            while (true)
            {
                var exit = Console.ReadLine();
                if (exit == "exit")
                {
                    bot.FinishGettingUpdates();
                    Environment.Exit(0);
                }
            }
        }
    }
}