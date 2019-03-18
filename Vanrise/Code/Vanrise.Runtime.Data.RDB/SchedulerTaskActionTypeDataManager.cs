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
    public class SchedulerTaskActionTypeDataManager : ISchedulerTaskActionTypeDataManager
    {
        static string TABLE_NAME = "runtime_SchedulerTaskActionType";
        static string TABLE_ALIAS = "STActionT";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        internal const string COL_ActionTypeInfo = "ActionTypeInfo";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static SchedulerTaskActionTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_ActionTypeInfo, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "SchedulerTaskActionType",
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

        public bool AreSchedulerTaskActionTypesUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        public List<SchedulerTaskActionType> GetAll()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);

            return queryContext.GetItems(SchedulerTaskActionTypeMapper);
        }

        #region Private Methods

        private SchedulerTaskActionType SchedulerTaskActionTypeMapper(IRDBDataReader reader)
        {
            return new SchedulerTaskActionType
            {
                ActionTypeId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Info = Serializer.Deserialize<ActionTypeInfo>(reader.GetString(COL_ActionTypeInfo))
            };
        }

        internal void JoinScheduleTaskActionType(RDBJoinContext joinContext, string schedulerTaskActionTypeTableALias, string otherTableAlias, string otherTableColumn, RDBJoinType rDBJoinType = RDBJoinType.Inner)
        {
            joinContext.JoinOnEqualOtherTableColumn(rDBJoinType, TABLE_NAME, schedulerTaskActionTypeTableALias, COL_ID, otherTableAlias, otherTableColumn);
        }

        #endregion
    }
}
