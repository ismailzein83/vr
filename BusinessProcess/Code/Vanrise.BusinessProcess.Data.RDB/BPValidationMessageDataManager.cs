using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPValidationMessageDataManager : IBPValidationMessageDataManager
    {

        static string TABLE_NAME = "bp_BPValidationMessage";
        static string TABLE_ALIAS = "ValidationMsg";

        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_ParentProcessID = "ParentProcessID";
        const string COL_TargetKey = "TargetKey";
        const string COL_TargetType = "TargetType";
        const string COL_Severity = "Severity";
        const string COL_Message = "Message";


        static BPValidationMessageDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ParentProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_TargetKey, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 900 });
            columns.Add(COL_TargetType, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_Severity, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Message, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPValidationMessage",
                Columns = columns
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcessTracking", "BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString");
        }

        #region Public Methods
        public List<BPValidationMessage> GetBeforeId(BPValidationMessageBeforeIdInput input)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, input.NbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.LessThanCondition(COL_ID).Value(input.LessThanID);
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(input.BPInstanceID);

            var sortContext = selectQuery.Sort();
            sortContext.ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(BPValidationMessageMapper);
        }
        public IEnumerable<BPValidationMessage> GetFilteredBPValidationMessage(BPValidationMessageQuery query)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            if (query.Severities != null && query.Severities.Count > 0)
                whereContext.ListCondition(COL_Severity, RDBListConditionOperator.IN, query.Severities.Select(itm => (int)itm));
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(query.ProcessInstanceId);

            return queryContext.GetItems(BPValidationMessageMapper);
        }
        public List<BPValidationMessage> GetUpdated(BPValidationMessageUpdateInput input)
        {
            List<BPValidationMessage> bpValidationMessages = new List<BPValidationMessage>();

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, input.NbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(input.BPInstanceID);

            if (input.GreaterThanID == default(long))
            {
                var sortContext = selectQuery.Sort();
                sortContext.ByColumn(COL_ID, RDBSortDirection.DESC);
            }
            else
            {
                whereContext.GreaterThanCondition(COL_ID).Value(input.GreaterThanID);

                var sortContext = selectQuery.Sort();
                sortContext.ByColumn(COL_ID, RDBSortDirection.ASC);
            }

            return queryContext.GetItems(BPValidationMessageMapper);
        }
        public void Insert(IEnumerable<Entities.BPValidationMessage> messages)
        {
            var queryContext = new RDBQueryContext(GetDataProvider(), true);
            var multipleInsertQuery = queryContext.AddInsertMultipleRowsQuery();
            multipleInsertQuery.IntoTable(TABLE_NAME);
            foreach (BPValidationMessage msg in messages)
            {
                var rowInsertContext = multipleInsertQuery.AddRow();
                rowInsertContext.Column(COL_ProcessInstanceID).Value(msg.ProcessInstanceId);
                if (msg.ParentProcessId.HasValue)
                    rowInsertContext.Column(COL_ParentProcessID).Value(msg.ParentProcessId.Value);
                if (msg.TargetKey != null)
                    rowInsertContext.Column(COL_TargetKey).Value(msg.TargetKey.ToString());
                rowInsertContext.Column(COL_TargetType).Value(msg.TargetType);
                rowInsertContext.Column(COL_Severity).Value((int)msg.Severity);
                rowInsertContext.Column(COL_Message).Value(msg.Message);
            }
            queryContext.ExecuteNonQuery();
        }
        #endregion
        #region Mappers
        private BPValidationMessage BPValidationMessageMapper(IRDBDataReader reader)
        {
            BPValidationMessage bpValidationMessage = new BPValidationMessage()
            {
                ValidationMessageId = reader.GetLong(COL_ID),
                ProcessInstanceId = reader.GetLong(COL_ProcessInstanceID),
                ParentProcessId = reader.GetNullableLong(COL_ParentProcessID),
                TargetKey = reader.GetString(COL_TargetKey),
                TargetType = reader.GetString(COL_TargetType),
                Severity = (ActionSeverity)reader.GetInt(COL_Severity),
                Message = reader.GetString(COL_Message)
            };
            return bpValidationMessage;
        }
        #endregion
    }
}
