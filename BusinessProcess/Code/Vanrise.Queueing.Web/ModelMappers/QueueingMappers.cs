using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public static List<QueueInstanceModel> MapQueueInstances(List<QueueInstance> instances)
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