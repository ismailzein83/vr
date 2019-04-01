﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Jazz.Data;
using TOne.WhS.Jazz.Entities;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Jazz.Data.RDB
{
    public class DraftReportTransactionDataManager : IDraftReportTransactionDataManager
    {
        #region Local Variables
        static string TABLE_NAME = "Jazz_ERP_ERPDraftReportTransaction";
        static string TABLE_ALIAS = "draftReportTransaction";
        const string COL_ID = "ID";
        const string COL_ERPDraftReportID = "ERPDraftReportID";
        const string COL_TransactionCode = "TransactionCode";
        const string COL_TransactionDescription = "TransactionDescription";
        const string COL_Credit = "Credit";
        const string COL_Debit = "Debit";
        const string COL_CarriersNames = "CarriersNames";
        const string COL_CreatedTime = "CreatedTime";
        #endregion

        #region Constructors
        static DraftReportTransactionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ERPDraftReportID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_TransactionCode, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 900 });
            columns.Add(COL_TransactionDescription, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar});
            columns.Add(COL_Credit, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal,Size=26,Precision=6 });
            columns.Add(COL_Debit, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 26, Precision = 6 });
            columns.Add(COL_CarriersNames, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Jazz_ERP",
                DBTableName = "ERPDraftReportTransaction",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        #endregion

        #region Public Methods

        public Dictionary<long, List<ERPDraftReportTransaction>> GetTransactionsReportsData(List<long> reportsIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().ListCondition(COL_ERPDraftReportID, RDBListConditionOperator.IN, reportsIds);

            Dictionary<long, List<ERPDraftReportTransaction>> data = null;
            queryContext.ExecuteReader(
              (reader) =>
              {
                  while (reader.Read())
                  {
                      if (data == null)
                          data = new Dictionary<long, List<ERPDraftReportTransaction>>();

                      long draftReportId = reader.GetLong(COL_ERPDraftReportID);
                      ERPDraftReportTransaction transactionsReportData = new ERPDraftReportTransaction
                      {
                          TransactionCode = reader.GetString(COL_TransactionCode),
                          TransationDescription = reader.GetString(COL_TransactionDescription),
                          Credit = reader.GetDecimal(COL_Credit),
                          Debit = reader.GetDecimal(COL_Debit),
                      };
                      List<ERPDraftReportTransaction> transactionsReportsData = data.GetOrCreateItem(draftReportId);
                      transactionsReportsData.Add(transactionsReportData);
                  }
              });
            return data;
        }

        public void Insert(List<JazzTransactionsReportData> transactionsReportsData,long reportId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            if(transactionsReportsData !=null && transactionsReportsData.Count > 0)
            {
                var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
                insertMultipleRowsQuery.IntoTable(TABLE_NAME);
                foreach (var data in transactionsReportsData)
                {
                    var row = insertMultipleRowsQuery.AddRow();
                    row.Column(COL_ERPDraftReportID).Value(reportId);
                    row.Column(COL_TransactionCode).Value(data.TransactionCode);
                    row.Column(COL_TransactionDescription).Value(data.TransationDescription);
                    row.Column(COL_Credit).Value(data.Credit);
                    row.Column(COL_Debit).Value(data.Debit);
                    row.Column(COL_CarriersNames).Value(data.CarriersNames);
                }
            }
             queryContext.ExecuteNonQuery();
        }

        public void Delete(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            var joinContext = deleteQuery.Join(TABLE_ALIAS);
            var whereCondition = deleteQuery.Where();
            DraftReportDataManager draftReportDataManager = new DraftReportDataManager();
            draftReportDataManager.AddJoinToDraftReport(joinContext,TABLE_ALIAS, "draftReport", COL_ERPDraftReportID);
            draftReportDataManager.AddProcessInstanceIdCondition(whereCondition, "draftReport", processInstanceId);
            queryContext.ExecuteNonQuery();
        }

        internal void SetSelectQuery(RDBSelectQuery selectQuery,string tableAlias,long processInstanceId,string transactionCodeAlias,string transactionDescriptionAlias,string creditAlias,string debitAlias, string switchNameAlias, string directionAlias, string carriersNamesAlias, string transactionTypeAlias)
        {
            DraftReportDataManager draftReportDataManager = new DraftReportDataManager();

            selectQuery.From(TABLE_NAME, tableAlias);
            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_TransactionCode, transactionCodeAlias);
            selectColumns.Column(COL_TransactionDescription, transactionDescriptionAlias);
            selectColumns.Column(COL_Credit, creditAlias);
            selectColumns.Column(COL_Debit, debitAlias);
            selectColumns.Column(COL_CarriersNames, carriersNamesAlias);

            var joinContext = selectQuery.Join();
            draftReportDataManager.SetSelectQuery(selectQuery, selectColumns, tableAlias, "draftReport", COL_ERPDraftReportID, switchNameAlias, directionAlias, transactionTypeAlias, processInstanceId);

            var whereCondition = selectQuery.Where();
            draftReportDataManager.AddProcessInstanceIdCondition(whereCondition, "draftReport", processInstanceId);

        }
        #endregion

        #region Private Methods

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("WhS_Jazz", "WhSJAZZERPIntegTransactionDBConnStringKey", "WhSJAZZERPIntegTransactionDBConnString");
        }
        #endregion

        #region Mappers


        #endregion
    }
}