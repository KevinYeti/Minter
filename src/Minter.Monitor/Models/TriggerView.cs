using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minter.Monitor.Models
{
    public class TriggerView
    {
        public string Name { get; set; }

        public TriggerKey Key { get; set; }

        public TriggerState State { get; set; }

        public DateTime PreviousFireTime { get; set; }
        public DateTime NextFireTime { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}