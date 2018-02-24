using Minter.Interface;
using Minter.Quartz.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Minter.Quartz
{
    public static class PluginLoader
    {
        public static PluginInfo Load(string file)
        {
            PluginInfo plugin = null;
            if (File.Exists(file))
            {
                byte[] buffer = null;
                int retry = 0;
                while (retry < 100)
                {
                    retry++;
                    try
                    {
                        buffer = File.ReadAllBytes(file);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }

                try
                {
                    Assembly asm = Assembly.Load(buffer);
                    var types = asm.GetTypes();
                    foreach (var type in types)
                    {
                        if (!type.IsClass || type.IsNotPublic)
                            continue;

                        Type[] interfaces = type.GetInterfaces();//加载该类型接口

                        if (interfaces.Contains(typeof(IPlugin)))
                        {
                            plugin = new PluginInfo();
                            plugin.Deployed = false;
                            plugin.File = file;
                            plugin.Plugin = Activator.CreateInstance(type) as IPlugin;
                            if (File.Exists(file.Replace(".dll", ".cfg")))
                            {
                                plugin.Cron = ConfigReader.Read(file.Replace(".dll", ".cfg"));
                            }
                            else
                                plugin = null;
                        }
                    }
                }
                catch { }
            }

            return plugin;
        }

        public static void Unload(PluginInfo plugin, IScheduler scheduler)
        {
            var triggers = scheduler.GetTriggersOfJob(plugin.Plugin.Job.Key).Result;
            foreach (var trigger in triggers)
            {
                scheduler.UnscheduleJob(trigger.Key);
            }
            scheduler.DeleteJob(plugin.Plugin.Job.Key);
        }
    }
}
