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
            return GetItemsSP("[queue].[sp_QueueActivatorConfig_GetAll]", QueueActivatorConfigMapper);
        }


        #region Mappers

        QueueActivatorConfig QueueActivatorConfigMapper(IDataReader reader)
        {
            QueueActivatorConfig queueActivatorConfig = Vanrise.Common.Serializer.Deserialize<QueueActivatorConfig>(reader["Details"] as string);
            if (queueActivatorConfig != null)
            {
                queueActivatorConfig.QueueActivatorConfigId = Convert.ToInt32(reader["ID"]);
                queueActivatorConfig.Name = reader["Name"] as string;
            }
            return queueActivatorConfig;
        }

        #endregion

    }
}
