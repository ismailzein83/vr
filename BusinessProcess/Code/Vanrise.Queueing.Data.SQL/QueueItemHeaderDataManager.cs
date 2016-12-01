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


        #region ctor/Local Variables
      
        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();

        public QueueItemHeaderDataManager()
            : base(GetConnectionStringName("QueueItemDBConnStringKey", "QueueItemDBConnString"))
        {
        }


        static QueueItemHeaderDataManager()
        {
             _mapper.Add("Entity.ItemId", "ItemID");
             _mapper.Add("ExecutionFlowName", "ExecutionFlowTriggerItemID");
            _mapper.Add("Entity.SourceItemId", "SourceItemID");
            _mapper.Add("Entity.Description", "Description");
            _mapper.Add("Entity.RetryCount", "RetryCount");
            _mapper.Add("Entity.CreatedTime", "CreatedTime");
            _mapper.Add("Entity.LastUpdatedTime", "LastUpdatedTime");
            _mapper.Add("Entity.QueueId", "QueueId");
        }


        #endregion

        #region Public Methods
        public Vanrise.Entities.BigResult<Entities.QueueItemHeader> GetFilteredQueueItemHeader(Vanrise.Entities.DataRetrievalInput<Entities.QueueItemHeaderQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string queueIds = null;
                string queueStatusIds = null;
                if (input.Query.QueueIds != null && input.Query.QueueIds.Count() > 0)
                    queueIds = string.Join<int>(",", input.Query.QueueIds);

                if (input.Query.QueueStatusIds != null && input.Query.QueueStatusIds.Count() > 0)
                    queueStatusIds = string.Join<int>(",", input.Query.QueueStatusIds.Select(itm => (int)itm));
                ExecuteNonQuerySP("[queue].[sp_QueueItemHeader_CreateTempByFiltered2]", tempTableName, queueIds, queueStatusIds, input.Query.CreatedTimeFrom, input.Query.CreatedTimeTo);
            };

            return RetrieveData(input, createTempTableAction, QueueItemHeaderMapper, _mapper);
        }

        public List<QueueItemStatusSummary> GetItemStatusSummary()
        {
            return GetItemsSP("queue.sp_QueueItemHeader_GetItemStatusSummary", QueueItemStatusSummaryMapper);
        }
        #endregion

        #region Mappers

        private QueueItemHeader QueueItemHeaderMapper(IDataReader reader)
        {
            return new QueueItemHeader
            {
                ItemId = (long)reader["ItemID"],
                QueueId = (int)reader["QueueID"],
                ExecutionFlowTriggerItemId = GetReaderValue<long>(reader, "ExecutionFlowTriggerItemID"),
                SourceItemId = GetReaderValue<long>(reader, "SourceItemID"),
                Description = reader["Description"] as string,
                Status = (QueueItemStatus)reader["Status"],
                RetryCount = GetReaderValue<int>(reader, "RetryCount"),
                ErrorMessage = reader["ErrorMessage"] as string,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                LastUpdatedTime = GetReaderValue<DateTime>(reader, "LastUpdatedTime")
            };
        }


        private QueueItemStatusSummary QueueItemStatusSummaryMapper(IDataReader reader)
        {
            return new QueueItemStatusSummary
            {
                QueueId = (int)reader["QueueId"],
                Status = (QueueItemStatus)reader["Status"],
                Count = (int)reader["Count"]
            };
        }


        #endregion


        

    }
}
