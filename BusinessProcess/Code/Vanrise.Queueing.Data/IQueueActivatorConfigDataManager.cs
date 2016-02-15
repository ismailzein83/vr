using System;
using System.Collections.Generic;
using Vanrise.Queueing.Entities;
namespace Vanrise.Queueing.Data
{
    public interface IQueueActivatorConfigDataManager : IDataManager
    {
        List<QueueActivatorConfig> GetAllQueueActivatorConfig();

        bool AreQueueActivatorConfigUpdated(ref object _updateHandle);
    }
}
