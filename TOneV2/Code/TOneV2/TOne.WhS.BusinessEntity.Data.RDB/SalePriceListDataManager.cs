using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePriceListDataManager : ISalePriceListDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sp";
        static string TABLE_NAME = "TOneWhS_BE_SalePriceList";
        const string COL_ID = "ID";
        const string COL_OwnerType = "OwnerType";
        const string COL_OwnerID = "OwnerID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_EffectiveOn = "EffectiveOn";
        const string COL_PriceListType = "PriceListType";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_FileID = "FileID";
        const string COL_IsSent = "IsSent";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_UserID = "UserID";
        const string COL_Description = "Description";
        const string COL_PricelistStateBackupID = "PricelistStateBackupID";
        const string COL_PricelistSource = "PricelistSource";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SalePriceListDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_OwnerType, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_OwnerID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CurrencyID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_EffectiveOn, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_PriceListType, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_ProcessInstanceID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_FileID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_IsSent, new RDBTableColumnDefinition {DataType = RDBDataType.Boolean}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_UserID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Description, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_PricelistStateBackupID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_PricelistSource, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePriceList",
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

        #region ISalePriceListDataManager Members
        public List<SalePriceList> GetPriceLists()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SalePriceListMapper);
        }

        public bool ArGetSalePriceListsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool SetCustomerPricelistsAsSent(IEnumerable<int> customerIds, int? priceListId)
        {
            if (customerIds == null || !customerIds.Any())
                return false;

            if (!priceListId.HasValue)
                return false;

            int recordsEffected = 0;

            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(priceListId.Value);
            whereContext.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, customerIds);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public void SavePriceListsToDb(IEnumerable<NewPriceList> salePriceLists)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (var salePriceList in salePriceLists)
            {
                WriteRecordToStream(salePriceList, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }

        #endregion
    
        #region Not Used Function
        public bool Update(SalePriceList salePriceList)
        {
            throw new NotImplementedException();
        }

        public bool Insert(SalePriceList salePriceList)
        {
            throw new NotImplementedException();
        }
        public void SavePriceListsToDb(List<SalePriceList> salePriceLists)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Mappers
        SalePriceList SalePriceListMapper(IRDBDataReader reader)
        {
            SalePriceList salePriceList = new SalePriceList
            {
                OwnerId = reader.GetInt("OwnerID"),
                CurrencyId = reader.GetInt("CurrencyID"),
                PriceListId = reader.GetInt("ID"),
                OwnerType = (SalePriceListOwnerType)reader.GetInt("OwnerType"),
                EffectiveOn = reader.GetDateTimeWithNullHandling("EffectiveOn"),
                PriceListType = (SalePriceListType?)reader.GetIntWithNullHandling("PriceListType"),
                ProcessInstanceId = reader.GetLongWithNullHandling("ProcessInstanceID"),
                FileId = reader.GetLongWithNullHandling("FileID"),
                CreatedTime = reader.GetDateTimeWithNullHandling("CreatedTime"),
                IsSent = reader.GetBooleanWithNullHandling("IsSent"),
                SourceId = reader.GetString("SourceID"),
                UserId = reader.GetIntWithNullHandling("UserID"),
                Description = reader.GetString("Description"),
                PricelistStateBackupId = reader.GetNullableLong("PricelistStateBackupID"),
                PricelistSource = (SalePricelistSource?)reader.GetIntWithNullHandling("PricelistSource")
            };
            return salePriceList;
        }

        #endregion

        #region Bulk 

        private object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_ID, COL_OwnerType, COL_OwnerID, COL_CurrencyID, COL_EffectiveOn, COL_PriceListType, COL_ProcessInstanceID, COL_FileID, COL_UserID, COL_Description);
            return streamForBulkInsert;
        }
        private void WriteRecordToStream(NewPriceList record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(record.PriceListId);
            recordContext.Value((int)record.OwnerType);
            recordContext.Value(record.OwnerId);
            recordContext.Value(record.CurrencyId);
            recordContext.Value(record.EffectiveOn);

            if (record.PriceListType.HasValue)
                recordContext.Value((int)record.PriceListType);
            else
                recordContext.Value(string.Empty);
            
            recordContext.Value(record.ProcessInstanceId);
            recordContext.Value(record.FileId);
            recordContext.Value(record.UserId);
            recordContext.Value(record.Description);
        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        private void ApplyForDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();

        }
        #endregion

        #region StateBackup

        // TODO : when RDB support insertion from database into another database

        #endregion

    }
}
