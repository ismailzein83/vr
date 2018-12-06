using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class ExcludedItemsDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IExcludedItemsDataManager
    {
        private readonly string[] _ExcludedItems =
        {
            "ItemID","ItemType","ItemName","Description","ParentId","ProcessInstanceId"
        };
        #region Public Methods
        public ExcludedItemsDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public IEnumerable<ExcludedItem> GetAllExcludedItems(ExcludedItemsQuery query)
        {
            return GetItemsSP("TOneWhS_Sales.sp_ExcludedItems_GetAll", ExcludedItemMapper, query.ProcessInstanceId);
        }

        public IEnumerable<long> GetExcludedItemsProcessInstanceIds(IEnumerable<long> subscribersProcessInstanceIds)
        {
            string processInstanceIds = null;
            if (subscribersProcessInstanceIds != null)
                processInstanceIds = string.Join(",", subscribersProcessInstanceIds);
            return GetItemsSP("TOneWhS_Sales.sp_ExcludedItems_GetProcessInstanceIds", reader =>
            {
                return (long)reader["ProcessInstanceId"];
            }
            , processInstanceIds);
        }
        public void BulkInsertExcludedItems(List<ExcludedItem> excludedItems)
        {
            if (excludedItems == null || !excludedItems.Any())
                return;

            object dbApplyStrem = InitialiazeStreamForDBApply();
            foreach (var excludedItem in excludedItems)
                WriteRecordToStream(excludedItem, dbApplyStrem);
            object preparedSnapshot = FinishDBApplyStream(dbApplyStrem, "TOneWhS_Sales.RP_ExcludedItems", _ExcludedItems);
            ApplyChangesToDataBase(preparedSnapshot);
        }

        #endregion

        #region Private Methods
        private void WriteRecordToStream(ExcludedItem record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            if (streamForBulkInsert != null)
                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}",
                    record.ItemId,
                 (int)record.ItemType,
                  record.ItemName,
                  record.Reason,
                  record.ParentId,
            record.ProcessInstanceId);
        }

        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private object FinishDBApplyStream(object dbApplyStream, string tableName, string[] columnNames)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = tableName,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columnNames
            };

        }
        private void ApplyChangesToDataBase(object preparedObject)
        {
            InsertBulkToTable(preparedObject as BaseBulkInsertInfo);
        }
        #endregion

        #region Mappers
        private ExcludedItem ExcludedItemMapper(IDataReader reader)
        {
            ExcludedItem excludedItem = new ExcludedItem
            {
                ItemId = reader["ItemID"] as string,
                ItemType = (ExcludedItemTypeEnum)reader["ItemType"],
                ItemName = reader["ItemName"] as string,
                Reason = reader["Description"] as string,
                ParentId = GetReaderValue<int>(reader, "ParentId"),
                ProcessInstanceId = GetReaderValue<long>(reader, "ProcessInstanceId")
            };
            return excludedItem;
        }
        #endregion

    }
}
