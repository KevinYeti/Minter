using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minter.Monitor.Models
{
    public class JobView
    {
        public List<TriggerView> Triggers = new List<TriggerView>();

        public JobKey Key { get; set; }

        public string Name { get; set; }

    }
}