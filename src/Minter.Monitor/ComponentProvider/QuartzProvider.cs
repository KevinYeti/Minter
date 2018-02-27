using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Minter.Monitor.ComponentProvider
{
    public static class QuartzProvider
    {
        private static IScheduler _scheduler = null;
        public static void Init()
        {
            if (_scheduler == null)
            {
                var properties = new NameValueCollection();
                properties["quartz.scheduler.proxy"] = "true";
                properties["quartz.scheduler.proxy.address"] = "tcp://localhost:555/MinterQuartzScheduler";
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
                _scheduler = schedulerFactory.GetScheduler().Result; 
            }
        }

        public static string GetSchedulerInstanceId()
        {
            return _scheduler.SchedulerInstanceId;
        }
        public static string GetSchedulerState()
        {
            if (_scheduler.IsShutdown)
                return "Shutdown";
            else if (_scheduler.InStandbyMode)
                return "Standby";
            else if (_scheduler.IsStarted)
                return "Started";
            else
                return "Unknown";
        }
        public static string GetSchedulerName()
        {
            return _scheduler.SchedulerName;
        }

        public static void StandbyScheduler()
        {
            if (_scheduler.IsStarted && !_scheduler.IsShutdown)
                _scheduler.Standby();
        }

        public static void ResumeScheduler()
        {
            if (!_scheduler.IsShutdown)
                _scheduler.Start();
        }

        public static string[] GetGroups()
        {
            return _scheduler.GetJobGroupNames().Result.ToArray();
        }

        public static List<JobKey> GetJobs(string group)
        {
            return _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)).Result.ToList();
        }

        public static List<ITrigger> GetTriggers(JobKey key)
        {
            return _scheduler.GetTriggersOfJob(key).Result.ToList();
        }

        public static TriggerState GetTriggerState(TriggerKey key)
        {
            return _scheduler.GetTriggerState(key).Result;
        }
    }
}