using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Jazz.Data;
using TOne.WhS.Jazz.Entities;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Jazz.Data.RDB
{
    public class DraftReportDataManager : IDraftReportDataManager
    {
        #region Local Variables
        static string TABLE_NAME = "Jazz_ERP_ERPDraftReport";
        static string TABLE_ALIAS = "draftReport";
        const string COL_ID = "ID";
        const string COL_ReportDefinitionID = "ReportDefinitionID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_SheetName = "SheetName";
        const string COL_TransactionTypeID = "TransactionTypeID";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_IsTaxTransaction = "IsTaxTransaction";
        #endregion

        #region Constructors
        static DraftReportDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ReportDefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_SheetName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_TransactionTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_IsTaxTransaction, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Jazz_ERP",
                DBTableName = "ERPDraftReport",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        #endregion

        #region Public Methods

        public void Insert(JazzTransactionsReport transactionsReport,long processInstanceId, out long insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            insertQuery.Column(COL_ReportDefinitionID).Value(transactionsReport.ReportDefinitionId);
            insertQuery.Column(COL_SheetName).Value(transactionsReport.SheetName);
            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceId);
            if (transactionsReport.TransactionTypeId.HasValue)
                insertQuery.Column(COL_TransactionTypeID).Value(transactionsReport.TransactionTypeId.Value);
            insertQuery.Column(COL_IsTaxTransaction).Value(transactionsReport.IsTaxTransaction);

            insertedId = queryContext.ExecuteScalar().LongValue;
        }

        public List<ERPDraftReport> GetTransactionsReports(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            return queryContext.GetItems(DraftReportMapper);
        }

        public void Delete(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();

            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            queryContext.ExecuteNonQuery();
        }
        internal void AddJoinToDraftReport(RDBJoinContext joinContext,string originalTableAlias,string draftReportTableAlias,string originalTableReportIdColumnName)
        {
            joinContext.JoinOnEqualOtherTableColumn(TABLE_NAME, draftReportTableAlias, COL_ID, originalTableAlias, originalTableReportIdColumnName);
        }
        internal void AddProcessInstanceIdCondition(RDBConditionContext conditionContext,string draftReportTableAlias, long processInstanceIdValue)
        {
            conditionContext.EqualsCondition(draftReportTableAlias, COL_ProcessInstanceID).Value(processInstanceIdValue);
        }
  
        #endregion

        #region Private Methods

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("WhS_Jazz", "WhSJAZZERPIntegTransactionDBConnStringKey", "WhSJAZZERPIntegTransactionDBConnString");
        }
        #endregion

        #region Mappers
        ERPDraftReport DraftReportMapper(IRDBDataReader reader)
        {

            return new ERPDraftReport()
            {
                ReportDefinitionId = reader.GetGuid(COL_ReportDefinitionID),
                ReportId = reader.GetLong(COL_ID),
                SheetName = reader.GetString(COL_SheetName),
                TransactionTypeId = reader.GetNullableGuid(COL_TransactionTypeID)
            };
        }

        #endregion
    }
}