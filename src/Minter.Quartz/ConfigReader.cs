using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minter.Quartz
{
    public static class ConfigReader
    {
        public static string Read(string file)
        {
            string config = string.Empty;
            if (File.Exists(file))
            {
                int retry = 0;
                while (retry < 10)
                {
                    try
                    {
                        config = File.ReadAllText(file);
                        break;
                    }
                    catch 
                    {
                        retry++;
                        Thread.Sleep(200);
                    }
                }
            }

            return config;
        }
    }
}
