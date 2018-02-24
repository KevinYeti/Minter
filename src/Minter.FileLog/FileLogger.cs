using Minter.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Minter.FileLog
{
    public class FileLogger : ILogger
    {
        private static string GetFile(DateTime date)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Logs\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + date.ToString("yyyyMMdd") + ".log";
        }
        public int Count(DateTime date)
        {
            return Read(date).Length;
        }

        public string[] Read(DateTime date)
        {
            return File.ReadAllLines(FileLogger.GetFile(date));
        }

        public void Write(string log)
        {
            int retry = 0;
            while (retry < 3)
            {
                try
                {
                    string file = FileLogger.GetFile(DateTime.Today);
                    File.AppendAllLines(file, new string[] { DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff   ") + log });
                    break;
                }
                catch(Exception ex)
                {
                    retry++;
                }
            }
        }
    }
}
