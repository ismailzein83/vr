using System;
using System.Data;
using Vanrise.Data.SQL;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Entities;
using System.Collections.Generic;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class ReceivedPricelistManager : BaseSQLDataManager, IReceivedPricelistManager
    {
        public ReceivedPricelistManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #region Public Methods

        public bool InsertReceivedPricelist(ReceivedPricelist receivedPricelist, out int insertedObjectId)
        {
            object receivedPricelistId;

            int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_Insert", out receivedPricelistId, receivedPricelist.SupplierId, receivedPricelist.FileId, receivedPricelist.ReceivedDate, receivedPricelist.PricelistType, receivedPricelist.Status, receivedPricelist.PricelistId, receivedPricelist.ProcessInstanceId);
            insertedObjectId = (affectedRows > 0) ? (int)receivedPricelistId : -1;

            return (affectedRows > 0);
        }

        public bool InsertReceivedPricelist(int supplierId, long fileId, DateTime receivedDate, SupplierPriceListType pricelistType, ReceivedPricelistStatus status, int? pricelistId, long? processInstanceId, out int insertedObjectId)
        {
            object receivedPricelistId;

            int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_Insert", out receivedPricelistId, supplierId, fileId, receivedDate, pricelistType, status, pricelistId, processInstanceId);
            insertedObjectId = (affectedRows > 0) ? (int)receivedPricelistId : -1;

            return (affectedRows > 0);
        }

        public void UpdateReceivedPricelistStatus(int receivedPricelistId, ReceivedPricelistStatus status)
        {
            ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_UpdateStatus", receivedPricelistId, status);
        }

        public void UpdateReceivedPricelistInfos(int receivedPricelistId, ReceivedPricelistStatus status, int pricelistId, long processInstanceId)
        {
            ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_UpdatePrcielistInfos", receivedPricelistId, status, pricelistId, processInstanceId);
        }

        public List<ReceivedPricelist> GetFilteredReceivedPricelists(ReceivedPricelistQuery input)
        {
            List<int> supplierIds = input.SupplierIds;
            List<int> status = input.Status;

            string strSupplierIds = null;
            if (supplierIds != null && supplierIds.Count > 0)
                strSupplierIds = string.Join(",", supplierIds);

            string strStatus = null;
            if (status != null && status.Count > 0)
                strStatus = string.Join(",", status);

            return GetItemsSP("[TOneWhS_SPL].[sp_ReceivedSupplierPricelist_GetFiltered]", ReceivedPricelistMapper, strSupplierIds, strStatus, input.Top);
        }

        #endregion

        #region Mappers
        private ReceivedPricelist ReceivedPricelistMapper(IDataReader reader)
        {
            return new ReceivedPricelist()
            {
                //Id = GetReaderValue<int>(reader, "Id"),
                SupplierId =GetReaderValue<int>(reader, "SupplierId"),
                FileId = GetReaderValue<long?>(reader, "FileId"),
                ReceivedDate = (DateTime)reader["ReceivedDate"],
                PricelistType = (SupplierPriceListType?)GetReaderValue<int?>(reader, "PricelistType"),
                Status = (ReceivedPricelistStatus)GetReaderValue<int>(reader, "Status"),
                PricelistId = GetReaderValue<int?>(reader, "PricelistId"),
                ProcessInstanceId = GetReaderValue<long?>(reader, "ProcessInstanceId"),
                StartProcessingDate = GetReaderValue<DateTime?>(reader, "StartProcessingDate"),
            };
        }

        #endregion
    }
}
