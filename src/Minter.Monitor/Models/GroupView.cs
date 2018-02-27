using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minter.Monitor.Models
{
    public class GroupView
    {
        public List<JobView> Jobs = new List<JobView>();

        public string Name { get; set; }
    }
}