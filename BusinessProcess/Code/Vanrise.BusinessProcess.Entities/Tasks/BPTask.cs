using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public enum BPTaskStatus
    {
        [BPTaskStatus(IsClosed = false)]
        New = 0,
        [BPTaskStatus(IsClosed = false)]
        Started = 10,
        [BPTaskStatus(IsClosed = true)]
        Completed = 50,
        [BPTaskStatus(IsClosed = true)]
        Cancelled = 60
    }

    public class BPTaskStatusAttribute : Attribute
    {
        static Dictionary<BPTaskStatus, BPTaskStatusAttribute> _cachedAttributes;
        static BPTaskStatusAttribute()
        {
            _cachedAttributes = new Dictionary<BPTaskStatus, BPTaskStatusAttribute>();
            foreach (var member in typeof(BPTaskStatus).GetFields())
            {
                BPTaskStatusAttribute mbrAttribute = member.GetCustomAttributes(typeof(BPTaskStatusAttribute), true).FirstOrDefault() as BPTaskStatusAttribute;
                if (mbrAttribute != null)
                    _cachedAttributes.Add((BPTaskStatus)Enum.Parse(typeof(BPTaskStatus), member.Name), mbrAttribute);
            }
        }

        public bool IsClosed { get; set; }

        public static BPTaskStatusAttribute GetAttribute(BPTaskStatus status)
        {
            return _cachedAttributes[status];
        }

        public static List<BPTaskStatus> GetClosedStatuses()
        {
            List<BPTaskStatus> rslt = new List<BPTaskStatus>();
            foreach (var statusEnum in Enum.GetValues(typeof(BPTaskStatus)))
            {
                BPTaskStatus status = (BPTaskStatus)statusEnum;
                if (GetAttribute(status).IsClosed)
                    rslt.Add(status);
            }
            return rslt;
        }
    }


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
        public string ExecutedByIdDescription { get; set; }

        public BPTaskStatus Status { get; set; }

        public BPTaskData TaskData { get; set; }

        public BPTaskExecutionInformation TaskExecutionInformation { get; set; }

        public string Notes { get; set; }

        public string Decision { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdatedTime { get; set; }
        public int? TakenBy { get; set; }
        public string TakenByDescription { get; set; }
    }
}
