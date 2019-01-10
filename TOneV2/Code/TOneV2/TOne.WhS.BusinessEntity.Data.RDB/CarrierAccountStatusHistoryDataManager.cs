using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class CarrierAccountStatusHistoryDataManager : ICarrierAccountStatusHistoryDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "cash";
        static string TABLE_NAME = "TOneWhS_BE_CarrierAccountStatusHistory";
        const string COL_ID = "ID";
        const string COL_CarrierAccountID = "CarrierAccountID";
        const string COL_StatusID = "StatusID";
        const string COL_PreviousStatusID = "PreviousStatusID";
        const string COL_StatusChangedDate = "StatusChangedDate";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";


        static CarrierAccountStatusHistoryDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CarrierAccountID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_StatusID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_PreviousStatusID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_StatusChangedDate, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CarrierAccountStatusHistory",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region ICarrierAccountStatusHistoryDataManager Members
        public void Insert(int carrierAccountId, ActivationStatus status, ActivationStatus? previousStatus)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_CreatedTime).Value(carrierAccountId);
            notExistsCondition.EqualsCondition(COL_StatusID).Value((int)status);

            if (previousStatus.HasValue)
                notExistsCondition.EqualsCondition(COL_PreviousStatusID).Value((int)previousStatus.Value);
            
            queryContext.ExecuteNonQuery();
        }
        #endregion
    }
}
