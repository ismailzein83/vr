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
    public class FinalTransactionDataManager : IFinalTransactionDataManager
    {
        #region Local Variables
        static string TABLE_NAME = "Jazz_ERP_ERPFinalTransaction";
        static string TABLE_ALIAS = "finalTransaction";
        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_FromDate = "FromDate";
        const string COL_ToDate = "ToDate";
        const string COL_TransactionCode = "TransactionCode";
        const string COL_TransactionDescription = "TransactionDescription";
        const string COL_Credit = "Credit";
        const string COL_Debit = "Debit";
        const string COL_CreatedTime = "CreatedTime";

        #endregion

        #region Constructors
        static FinalTransactionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_FromDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ToDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_TransactionCode, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 900 });
            columns.Add(COL_TransactionDescription, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Credit, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 26, Precision = 6 });
            columns.Add(COL_Debit, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 26, Precision = 6 });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Jazz_ERP",
                DBTableName = "ERPFinalTransaction",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        #endregion

        #region Public Methods

        public void Insert(DateTime fromDate, DateTime toDate, long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceId);
            insertQuery.Column(COL_FromDate).Value(fromDate);
            insertQuery.Column(COL_ToDate).Value(toDate);

            var fromSelect = insertQuery.FromSelect();

            DraftReportTransactionDataManager draftReportTransactionDataManager = new DraftReportTransactionDataManager();
            draftReportTransactionDataManager.SetSelectQuery(fromSelect, TABLE_ALIAS, processInstanceId, COL_TransactionCode, COL_TransactionDescription, COL_Credit, COL_Debit);
            queryContext.ExecuteNonQuery();
        }


        #endregion

        #region Private Methods

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("WhS_Jazz", "WhSJAZZERPIntegTransactionDBConnString", "WhSJAZZERPIntegTransactionDBConnString");
        }
        #endregion

        #region Mappers


        #endregion
    }
}