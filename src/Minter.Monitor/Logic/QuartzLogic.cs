using Minter.Monitor.ComponentProvider;
using Minter.Monitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minter.Monitor.Logic
{
    public static class QuartzLogic
    {
        public static SchedulerView GetSchedulerInfo()
        {
            SchedulerView scheduler = new SchedulerView();
            scheduler.InstanceId = QuartzProvider.GetSchedulerInstanceId();
            scheduler.Name = QuartzProvider.GetSchedulerName();
            scheduler.State = QuartzProvider.GetSchedulerState();

            var groups = QuartzProvider.GetGroups();
            foreach (var group in groups)
            {
                //group
                GroupView gv = new GroupView();
                gv.Name = group;

                //
                var jobs = QuartzProvider.GetJobs(group);
                foreach (var job in jobs)
                {
                    JobView jv = new JobView();
                    jv.Key = job;
                    jv.Name = job.Name;
                    var triggers = QuartzProvider.GetTriggers(jv.Key);
                    foreach (var trigger in triggers)
                    {
                        TriggerView tv = new TriggerView();
                        tv.Key = trigger.Key;
                        tv.Name = trigger.Key.Name;
                        tv.State = QuartzProvider.GetTriggerState(trigger.Key);
                        tv.PreviousFireTime = trigger.GetPreviousFireTimeUtc().HasValue ? trigger.GetPreviousFireTimeUtc().Value.LocalDateTime : DateTime.MinValue;
                        tv.NextFireTime = trigger.GetNextFireTimeUtc().HasValue ? trigger.GetNextFireTimeUtc().Value.LocalDateTime : DateTime.MaxValue;
                        tv.StartTime = trigger.StartTimeUtc.LocalDateTime;
                        tv.EndTime = trigger.EndTimeUtc.HasValue ? trigger.EndTimeUtc.Value.LocalDateTime : DateTime.MaxValue;

                        jv.Triggers.Add(tv);
                    }

                    gv.Jobs.Add(jv);
                }

                scheduler.Groups.Add(gv);
            }

            return scheduler;
        }
    }
}