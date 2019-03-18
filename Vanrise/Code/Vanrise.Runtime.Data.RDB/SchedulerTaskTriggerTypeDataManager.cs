using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class SchedulerTaskTriggerTypeDataManager : ISchedulerTaskTriggerTypeDataManager
    {
        static string TABLE_NAME = "runtime_SchedulerTaskTriggerType";
        static string TABLE_ALIAS = "STTriggerType";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        internal const string COL_TriggerTypeInfo = "TriggerTypeInfo";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static SchedulerTaskTriggerTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_TriggerTypeInfo, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "SchedulerTaskTriggerType",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RuntimeConfig", "RuntimeConfigDBConnStringKey", "RuntimeConfigDBConnString");
        }

        public bool AreSchedulerTaskTriggerTypesUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        public List<SchedulerTaskTriggerType> GetAll()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);

            return queryContext.GetItems(SchedulerTaskTriggerTypeMapper);
        }

        #region Private Methods

        private SchedulerTaskTriggerType SchedulerTaskTriggerTypeMapper(IRDBDataReader reader)
        {
            return new SchedulerTaskTriggerType
            {
                TriggerTypeId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Info = Serializer.Deserialize<TriggerTypeInfo>(reader.GetString(COL_TriggerTypeInfo))
            };
        }

        internal void JoinScheduleTaskTriggerType(RDBJoinContext joinContext, string schedulerTaskTriggerTypeTableALias, string otherTableAlias, string OtherTableColumnName, RDBJoinType rDBJoinType = RDBJoinType.Inner)
        {
            joinContext.JoinOnEqualOtherTableColumn(rDBJoinType, TABLE_NAME, schedulerTaskTriggerTypeTableALias, COL_ID, otherTableAlias, OtherTableColumnName);
        }
        #endregion
    }
}
