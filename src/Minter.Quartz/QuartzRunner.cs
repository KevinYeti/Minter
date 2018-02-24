using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Minter.Interface;
using Minter.Quartz.Model;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Minter.Quartz
{
    public static class QuartzRunner
    {
        private static bool _exit = false;
        private static Thread _thread = null;
        private static Hashtable _plugins = null;
        private static IScheduler _scheduler = null;
        private static FileSystemWatcher _fsw = null;
        private static FileSystemWatcher _fswCfg = null;
        private static object _locker = new object();
        
        public static void Run()
        {
            _exit = false;
            //初始化调度器
            if (_scheduler == null)
            {
                _scheduler = QuartzRunner.CreateScheduler();
                _scheduler.Start();
            }
            //
            if (_plugins == null)
            {
                _plugins = new Hashtable();
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Plugins"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Plugins");

                var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Plugins", "*.dll", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var plugin = PluginLoader.Load(file);
                    if (plugin != null && !_plugins.ContainsKey(plugin.File))
                    {
                        _plugins.Add(plugin.File, plugin);
                        WriteLog("New plugin found: " + plugin.File);
                    }
                }
            }

            if (_thread == null)
            {
                _thread = new Thread(new ThreadStart(RunScheduler)) { IsBackground = true };
                _thread.Start();
            }

            if (_fsw == null)
            {
                _fsw = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory + "Plugins", "*.dll");
                _fsw.IncludeSubdirectories = true;
                _fsw.Created += _fsw_Created;
                _fsw.Deleted += _fsw_Deleted;
                _fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                _fsw.EnableRaisingEvents = true;
            }
            if (_fswCfg == null)
            {
                _fswCfg = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory + "Plugins", "*.cfg");
                _fswCfg.IncludeSubdirectories = true;
                _fswCfg.Changed += _fswCfg_Changed;
                _fswCfg.NotifyFilter = NotifyFilters.LastWrite;
                _fswCfg.EnableRaisingEvents = true;
            }
            WriteLog("Scheduler running...");
        }

        public static void WriteLog(string log)
        {
            var logger = Global.Container.Resolve<ILogger>();
            logger.Write(log);
        }
        private static void _fswCfg_Changed(object sender, FileSystemEventArgs e)
        {
            lock (_locker)
            {
                try
                {
                    string file = e.FullPath;
                    string plugin = file.Replace(".cfg", ".dll");
                    if (_plugins.ContainsKey(plugin))
                    {
                        string cron = ConfigReader.Read(file);
                        if (string.IsNullOrEmpty(cron))
                            return;

                        var info = _plugins[plugin] as PluginInfo;
                        if (info.Cron != cron)
                        {
                            info.Cron = cron;
                            var trigger = _scheduler.GetTriggersOfJob(info.Plugin.Job.Key).Result.FirstOrDefault();
                            var triggerNew = TriggerBuilder.Create()
                                            .WithIdentity("Trigger-" + info.Plugin.Name + DateTime.Now.ToString("yyyyMMddHHmmssfff"), info.Plugin.Group)
                                            .ForJob(info.Plugin.Job)
                                            .StartNow()
                                            .WithCronSchedule(cron)
                                            .Build();
                            _scheduler.RescheduleJob(trigger.Key, triggerNew);
                            WriteLog("Job config changed: " + file);
                        }
                    }
                }
                catch(Exception ex)
                {
                    WriteLog(ex.Message);
                }
            }
        }

        private static void _fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (_locker)
            {
                string file = e.FullPath;
                if (_plugins.ContainsKey(file))
                {
                    var plugin = _plugins[file] as PluginInfo;
                    PluginLoader.Unload(plugin, _scheduler);

                    _plugins.Remove(file);

                    WriteLog("Job file offline: " + file);
                }
            }
        }

        private static void _fsw_Created(object sender, FileSystemEventArgs e)
        {
            lock (_locker)
            {
                string file = e.FullPath;
                var plugin = PluginLoader.Load(file);
                if (plugin != null && !_plugins.ContainsKey(plugin.File))
                {
                    _plugins.Add(plugin.File, plugin);
                    WriteLog("New plugin found: " + file);
                }
            }
        }

        public static void Stop()
        {
            _exit = true;
            WriteLog("Scheduler stopped.");
        }

        private static void RunScheduler()
        {
            while (!_exit)
            {
                try
                {
                    //读取所有没有部署上线的插件
                    var plugins = _plugins.Values.OfType<PluginInfo>().Where(p => p.Deployed == false);

                    //遍历这些插件，逐个上线
                    foreach (var plugin in plugins)
                    {
                        //生成trigger
                        var trigger = TriggerBuilder.Create()
                            .WithIdentity("Trigger-" + plugin.Plugin.Name + DateTime.Now.ToString("yyyyMMddHHmmssfff"), plugin.Plugin.Group)
                            .ForJob(plugin.Plugin.Job)
                            .StartNow()
                            .WithCronSchedule(plugin.Cron)
                            .Build();

                        //
                        _scheduler.ScheduleJob(plugin.Plugin.Job, trigger);
                        plugin.Deployed = true;

                        WriteLog("Job file online: " + plugin.File);
                    }
                }
                catch (Exception ex)
                {
                    //log
                    WriteLog(ex.Message);
                }

                //
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 创建一个调度器
        /// </summary>
        /// <returns></returns>
        private static IScheduler CreateScheduler()
        {
            //TODO:应该重构成从数据库读
            var properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "Minter Scheduler";
            properties["quartz.scheduler.instanceId"] = "Minter Scheduler";

            // set thread pool info
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "4";                  //db
            properties["quartz.threadPool.threadPriority"] = "Normal";          //db

            // set remoting expoter
            properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
            properties["quartz.scheduler.exporter.port"] = "555";                   //conf
            properties["quartz.scheduler.exporter.bindName"] = "MinterQuartzScheduler";   //conf
            properties["quartz.scheduler.exporter.channelType"] = "tcp";              //conf


            var schedulerFactory = new StdSchedulerFactory(properties);
            var scheduler = schedulerFactory.GetScheduler().Result;

            return scheduler;
        }
    }
}
