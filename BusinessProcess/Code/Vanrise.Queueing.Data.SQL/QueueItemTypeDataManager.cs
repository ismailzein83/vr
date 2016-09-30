using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{

    public class QueueItemTypeDataManager : BaseSQLDataManager, IQueueItemTypeDataManager
    {


        #region ctor

        public QueueItemTypeDataManager()
            : base(GetConnectionStringName("QueueingConfigDBConnStringKey", "QueueingConfigDBConnString"))
        {
        }

        #endregion


        #region Public Methods
        public IEnumerable<QueueItemType> GetQueueItemTypes()
        {
            return GetItemsSP("queue.sp_QueueItemTypes_GetAll", QueueItemTypeMapper);
        }


        public bool AreItemTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[queue].[QueueItemType]", ref updateHandle);

        }

        #endregion

        #region Mappers
        private QueueItemType QueueItemTypeMapper(IDataReader reader)
        {
            return new QueueItemType
            {
                Id = (int)reader["ID"],
                ItemFQTN = reader["ItemFQTN"] as string,
                Title = reader["Title"] as string,
                CreatedTime = (DateTime)reader["CreatedTime"],
                DefaultQueueSettings = reader["DefaultQueueSettings"] as string
            };
        }

        #endregion

    }
}
