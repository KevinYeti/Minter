using Microsoft.AspNetCore.Mvc;
using Minter.Monitor.Logic;
using Minter.Monitor.Models;

namespace Minter.Monitor.Api
{
    public class SchedulerController : Controller
    {
        public SchedulerView Get()
        {
            return QuartzLogic.GetSchedulerInfo();
        }
    }
}