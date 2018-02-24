using Minter.Quartz;
using System;
using System.Threading.Tasks;

namespace Minter.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            new TaskFactory().StartNew(() => QuartzRunner.Run());
            Console.WriteLine("Press QUIT to exit...");
            while (true)
            {
                var input = Console.ReadLine().Trim();
                switch (input)
                {
                    case "RESTART":
                        QuartzRunner.Stop();
                        new TaskFactory().StartNew(() => QuartzRunner.Run());
                        break;
                    case "QUIT":
                    case "EXIT":
                        QuartzRunner.Stop();
                        break;
                    case "HELP":
                        break;
                    case "STATUS":
                        break;
                    default:
                        break;
                }
            }
            
        }
    }
}
