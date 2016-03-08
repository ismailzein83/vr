using System;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Business
{
    public class UserTaskFilter : ISchedulerTaskFilter
    {
        public int UserId { get; set; }
        public bool IsMatched(SchedulerTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            if (task.OwnerId != UserId)
                return false;

            return true;
        }
    }
}
