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
    public class QueueItemHeaderDataManager : BaseSQLDataManager, IQueueItemHeaderDataManager
    {
        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();

        public QueueItemHeaderDataManager()
            : base(GetConnectionStringName("QueueItemDBConnStringKey", "QueueItemDBConnString"))
        {
        }

        #region ctor/Local Variables



        static QueueItemHeaderDataManager()
        {
             _mapper.Add("Entity.ItemId", "ItemID");
             _mapper.Add("Entity.QueueId", "QueueId");
            _mapper.Add("Entity.ExecutionFlowTriggerItemId", "ExecutionFlowTriggerItemID");
            _mapper.Add("Entity.SourceItemId", "SourceItemID");
            _mapper.Add("Entity.Description", "Description");
            _mapper.Add("Entity.Status", "Status");
            _mapper.Add("Entity.RetryCount", "RetryCount");
            _mapper.Add("Entity.ErrorMessage", "ErrorMessage");
            _mapper.Add("Entity.CreatedTime", "CreatedTime");
            _mapper.Add("Entity.LastUpdatedTime", "LastUpdatedTime");
        }


        #endregion


        public Vanrise.Entities.BigResult<Entities.QueueItemHeader> GetQueueItemHeaderFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.QueueItemHeaderQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string queueIds = null;
                if (input.Query.QueueIds != null && input.Query.QueueIds.Count() > 0)
                    queueIds = string.Join<int>(",", input.Query.QueueIds);


                ExecuteNonQuerySP("[queue].[sp_QueueItemHeader_CreateTempByFiltered2]", tempTableName, input.Query.ExecutionFlowIds, queueIds, input.Query.QueueStatusIds, input.Query.CreatedTimeFrom, input.Query.CreatedTimeTo);
            };

            return RetrieveData(input, createTempTableAction, QueueItemHeaderMapper, _mapper);
        }


        #region Mappers

        private QueueItemHeader QueueItemHeaderMapper(IDataReader reader)
        {
            QueueItemHeader queueItemHeader = new QueueItemHeader();
            queueItemHeader.ItemId = (int)reader["ItemID"];
            queueItemHeader.QueueId = (int)reader["QueueID"];
            queueItemHeader.ExecutionFlowTriggerItemId = (int)reader["ExecutionFlowTriggerItemID"];
            queueItemHeader.SourceItemId = (int)reader["SourceItemID"];
            queueItemHeader.Description = reader["Description"] as string;
            queueItemHeader.Status = (QueueItemStatus)reader["Status"];
            queueItemHeader.RetryCount = (int)reader["RetryCount"];
            queueItemHeader.ErrorMessage = reader["ErrorMessage"] as string;
            queueItemHeader.CreatedTime = (DateTime)reader["CreatedTime"];
            queueItemHeader.LastUpdatedTime = (DateTime)reader["LastUpdatedTime"];
            return queueItemHeader;
        }


        #endregion


        

    }
}
