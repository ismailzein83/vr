using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class AccountManagerCarrierDataManager : IAccountManagerCarrierDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "am";
        static string TABLE_NAME = "TOneWhS_BE_AccountManager";
        const string COL_UserId = "UserId";
        const string COL_CarrierAccountId = "CarrierAccountId";
        const string COL_RelationType = "RelationType";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static AccountManagerCarrierDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_UserId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CarrierAccountId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_RelationType, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "AccountManager",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region IAccountManagerCarrierDataManager Members

        public IEnumerable<AssignedCarrier> GetAssignedCarriers()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(AssignedCarrierMapper);
        }

        public bool AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            return false;
            //TODO check it 
            //var queryContext = new RDBQueryContext(GetDataProvider());
            //var tempTableQueryToInsert = CreateTempTable(queryContext, updatedCarriers, true);

            //var insertQuery = queryContext.AddInsertQuery();
            //insertQuery.IntoTable(TABLE_NAME);

            //var fromSelect = insertQuery.FromSelect();
            //fromSelect.From(TABLE_NAME, TABLE_ALIAS, null, true);
            //var joinContext = fromSelect.Join();
            //var joinStatement = joinContext.Join(tempTableQueryToInsert, "amToInsert");
            //joinStatement.JoinType(RDBJoinType.Left);
            //var joinCondition = joinStatement.On();
            //joinCondition.EqualsCondition(TABLE_ALIAS, COL_CarrierAccountId, "amToInsert", COL_CarrierAccountId);
            //joinCondition.EqualsCondition(TABLE_ALIAS, COL_RelationType, "amToInsert", COL_RelationType);
            //fromSelect.Where().NullCondition(COL_CarrierAccountId);

            //var tempTableQueryToDelete = CreateTempTable(queryContext, updatedCarriers, true);

            //var deleteQuery = queryContext.AddDeleteQuery();
            //deleteQuery.FromTable(TABLE_NAME);
            //var deleteJoinContext = deleteQuery.Join(TABLE_ALIAS);
            //var deleteJoinStatement = deleteJoinContext.Join(tempTableQueryToDelete, "amToDelete");
            //deleteJoinStatement.JoinType(rdb);

        }
        public bool AreAssignedCarriersUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }
        #endregion

        #region  Mappers

        AssignedCarrier AssignedCarrierMapper(IRDBDataReader reader)
        {
            return new AssignedCarrier
            {
                CarrierAccountId = reader.GetInt(COL_CarrierAccountId),
                UserId = reader.GetIntWithNullHandling(COL_UserId),
                RelationType = (CarrierAccountType)reader.GetInt(COL_RelationType)
            };
        }

        #endregion
        #region Private Methods

        private RDBTempTableQuery CreateTempTable(RDBQueryContext queryContext, UpdatedAccountManagerCarrier[] updatedCarriers, bool status)
        {
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_CarrierAccountId, RDBDataType.NVarchar, true);
            tempTableQuery.AddColumn(COL_UserId, RDBDataType.Int, false);
            tempTableQuery.AddColumn(COL_RelationType, RDBDataType.Int, false);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            var assignedAccountManagersToInsert = updatedCarriers.ToList().Where(it => it.Status == status);
            foreach (var queryItem in assignedAccountManagersToInsert)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_CarrierAccountId).Value(queryItem.CarrierAccountId);
                rowContext.Column(COL_UserId).Value(queryItem.UserId);
                rowContext.Column(COL_RelationType).Value(queryItem.RelationType);
            }
            return tempTableQuery;
        }
        private void SetInsertQuery(RDBQueryContext queryContext, UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            var tempTableQuery = CreateTempTable(queryContext, updatedCarriers, true);
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var fromSelect = insertQuery.FromSelect();
            fromSelect.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var joinContext = fromSelect.Join();
            var joinStatement = joinContext.Join(tempTableQuery, "amToInsert");
            joinStatement.JoinType(RDBJoinType.Left);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_CarrierAccountId, "amToInsert", COL_CarrierAccountId);
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_RelationType, "amToInsert", COL_RelationType);

            fromSelect.Where().NullCondition(COL_CarrierAccountId);
        }
        #endregion
    }
}
