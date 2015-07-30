using System;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Web.Models
{
    public class QueueInstanceModel
    {
        public int QueueInstanceId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public QueueInstanceStatus Status { get; set; }

        public int ItemTypeId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}