using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueActivatorConfigDataManager : BaseSQLDataManager, IQueueActivatorConfigDataManager
    {
        public QueueActivatorConfigDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {
        }
        public List<QueueActivatorConfig> GetAllQueueActivatorConfig()
        {
            //return GetItemsSP("[queue].[sp_QueueActivatorConfig_GetAll]", null);
            return new List<QueueActivatorConfig>();
        }

    }
}
