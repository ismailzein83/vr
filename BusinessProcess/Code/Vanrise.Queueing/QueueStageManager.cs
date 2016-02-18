using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueStageManager
    {
      
        #region Public Methods
        public IEnumerable<QueueStageNameInfo> GetStageNamesInfo(QueueStageFilter filter)
        {
            QueueInstanceManager manager = new QueueInstanceManager();
            IEnumerable<string> filteredStageNames = manager.GetAllQueueInstances().Select(x => x.StageName).Distinct().ToList();
            return filteredStageNames.MapRecords(QueueStageInfoMapper, null);

        }

        #endregion

        #region Mappers

        private QueueStageNameInfo QueueStageInfoMapper(string queueStageName)
        {
            QueueStageNameInfo queueStageNameInfo = new QueueStageNameInfo();
            queueStageNameInfo.Name = queueStageName;
            return queueStageNameInfo;
        }

        #endregion

    }
    
}
