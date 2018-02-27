using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minter.Monitor.Models
{
    public class SchedulerView
    {
        public List<GroupView> Groups = new List<GroupView>();

        public string InstanceId { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
    }
}