using System;
using System.Threading;

namespace WhiteboardServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();

            Console.WriteLine();
            Console.WriteLine("Press 'Q' to quit server...");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Q)
                    {
                        server.Stop();
                        break;
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
