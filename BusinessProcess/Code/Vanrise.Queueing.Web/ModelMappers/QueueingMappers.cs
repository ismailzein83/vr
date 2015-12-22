using System.Collections.Generic;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.ModelMappers
{
    public class QueueingMappers
    {
        private static QueueInstanceModel MapQueueInstance(QueueInstance instance)
        {
            return new QueueInstanceModel
            {
                Title = instance.Title,
                CreateTime = instance.CreateTime,
                ItemTypeId = instance.ItemTypeId,
                Name = instance.Name,
                QueueInstanceId = instance.QueueInstanceId,
                Status = instance.Status
            };
        }

        private static QueueItemHeaderModel MapQueueItemHeader(QueueItemHeader header)
        {
            return new QueueItemHeaderModel
            {
                ItemId = header.ItemId,
                QueueId = header.QueueId,
                SourceItemId = header.SourceItemId,
                Description = header.Description,
                Status = header.Status,
                StatusDescription = header.Status.ToString(),
                RetryCount = header.RetryCount,
                ErrorMessage = header.ErrorMessage,
                CreatedTime = header.CreatedTime,
                LastUpdatedTime = header.LastUpdatedTime,
            };
        }

        public static List<QueueItemHeaderModel> MapQueueItemHeaders(List<QueueItemHeader> headers)
        {
            List<QueueItemHeaderModel> models = new List<QueueItemHeaderModel>();
            if (headers != null)
                foreach (var h in headers)
                {
                    models.Add(MapQueueItemHeader(h));
                }
            return models;
        }

        public static IEnumerable<QueueInstanceModel> MapQueueInstances(IEnumerable<QueueInstance> instances)
        {
            List<QueueInstanceModel> models = new List<QueueInstanceModel>();
            if (instances != null)
                foreach (var ins in instances)
                {
                    models.Add(MapQueueInstance(ins));
                }
            return models;
        }
    }
}