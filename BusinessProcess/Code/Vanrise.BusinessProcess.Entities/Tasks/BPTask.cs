using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public enum BPTaskStatus { New = 0, Started = 10, Completed = 50, Cancelled = 60 }

    public class BPTask
    {
        public static string GetTaskWFBookmark(long taskId)
        {
            return String.Format("BPTask_Bookmark_{0}", taskId);
        }

        public long BPTaskId { get; set; }

        public long ProcessInstanceId { get; set; }

        public Guid TypeId { get; set; }

        public string Title { get; set; }

        public List<int> AssignedUsers { get; set; }

        public string AssignedUsersDescription { get; set; }

        public int? ExecutedById { get; set; }

        public BPTaskStatus Status { get; set; }

        public BPTaskData TaskData { get; set; }

        public BPTaskExecutionInformation TaskExecutionInformation { get; set; }
        
        public string Notes { get; set; }

        public string Decision { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdatedTime { get; set; }
    }
}
