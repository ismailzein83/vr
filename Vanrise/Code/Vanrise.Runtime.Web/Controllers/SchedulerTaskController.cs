using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Web.Controllers
{
    public class SchedulerTaskController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<SchedulerTask> GetFilteredTasks(int fromRow, int toRow, string name)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetFilteredTasks(fromRow, toRow, name);
        }
    }
}