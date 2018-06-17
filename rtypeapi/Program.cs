using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rtypeapi
{
    class Program
    {
        static Listener listener;

        static void Main(string[] args)
        {
            listener = new Listener();
            listener.Listen();
            Console.ReadKey();
        }
    }
}