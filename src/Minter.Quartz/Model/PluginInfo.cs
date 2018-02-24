using Minter.Interface;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minter.Quartz.Model
{
    public class PluginInfo
    {
        public bool Deployed { get; set; }
        public IPlugin Plugin { get; set; }

        public string File { get; set; }

        public List<ITrigger> Triggers = new List<ITrigger>();

        public string Cron { get; set; }
    }
}
