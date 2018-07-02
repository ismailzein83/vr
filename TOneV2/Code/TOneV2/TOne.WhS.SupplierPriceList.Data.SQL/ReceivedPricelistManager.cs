using System;
using System.Data;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using System.Linq;

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

			int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_Insert", out receivedPricelistId, receivedPricelist.SupplierId, receivedPricelist.FileId, receivedPricelist.ReceivedDateTime, receivedPricelist.PricelistType, receivedPricelist.Status, receivedPricelist.PricelistId, receivedPricelist.ProcessInstanceId);
			insertedObjectId = (affectedRows > 0) ? (int)receivedPricelistId : -1;

			return (affectedRows > 0);
		}

		public bool InsertReceivedPricelist(int supplierId, long? fileId, DateTime receivedDate, SupplierPricelistType? pricelistType, ReceivedPricelistStatus status, IEnumerable<SPLImportErrorDetail> errors, out int recordId)
		{
			object receivedPricelistRecordId;
			var serializedErrors = (errors != null && errors.Any()) ? Vanrise.Common.Serializer.Serialize(errors) : null;
			int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_Insert", out receivedPricelistRecordId, supplierId, fileId, receivedDate, pricelistType, status, null, null, serializedErrors);
			recordId = (affectedRows > 0) ? (int)receivedPricelistRecordId : -1;

			return (affectedRows > 0);
		}

		public void SetReceivedPricelistAsStarted(int receivedPricelistRecordId, ReceivedPricelistStatus status, long processInstanceId, DateTime startProcessingTime)
		{
			ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_SetAsStarted", receivedPricelistRecordId, status, processInstanceId, startProcessingTime);
		}

		public void SetReceivedPricelistAsCompleted(int receivedPricelistRecordId, ReceivedPricelistStatus status, int pricelistId)
		{
			ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_SetAsCompleted", receivedPricelistRecordId, status, pricelistId);
		}

		public void UpdateReceivedPricelistStatus(int receivedPricelistRecordId, ReceivedPricelistStatus status)
		{
			ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_UpdateStatus", receivedPricelistRecordId, status);
		}
        public void UpdateSentToSupplierStatus (int receivedPricelistRecordId, bool status)
        {
            ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_SetSentToSupplierStatus", receivedPricelistRecordId, status);
        }

		public bool SetReceivedPricelistAsCompletedManualy(int receivedPricelistRecordId, ReceivedPricelistStatus status)
		{
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_SetAsCompletedManualy", receivedPricelistRecordId, status);
            return (recordsEffected > 0);
		}

		public void UpdateReceivedPricelistStatus(int receivedPricelistRecordId, ReceivedPricelistStatus status, IEnumerable<SPLImportErrorDetail> errors)
		{
			var serializedErrors = (errors != null && errors.Any()) ? Vanrise.Common.Serializer.Serialize(errors) : null;
			ExecuteNonQuerySP("[TOneWhS_SPL].sp_ReceivedSupplierPricelist_UpdateStatusWithErrors", receivedPricelistRecordId, status, serializedErrors);
		}

		public ReceivedPricelist GetReceivedPricelistById(int id)
		{
			return GetItemSP("[TOneWhS_SPL].[sp_ReceivedSupplierPricelist_GetReceivedPricelistById]", ReceivedPricelistMapper, id);
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
				Id = GetReaderValue<int>(reader, "ID"),
				SupplierId = GetReaderValue<int>(reader, "SupplierId"),
				FileId = GetReaderValue<long?>(reader, "FileId"),
				ReceivedDateTime = (DateTime)reader["ReceivedDate"],
				PricelistType = (SupplierPricelistType?)GetReaderValue<int?>(reader, "PricelistType"),
				Status = (ReceivedPricelistStatus)GetReaderValue<int>(reader, "Status"),
				PricelistId = GetReaderValue<int?>(reader, "PricelistId"),
				ProcessInstanceId = GetReaderValue<long?>(reader, "ProcessInstanceId"),
				StartProcessingDateTime = GetReaderValue<DateTime?>(reader, "StartProcessingDate"),
				ErrorDetails = string.IsNullOrEmpty(GetReaderValue<string>(reader, "ErrorDetails")) ? null : Serializer.Deserialize<List<SPLImportErrorDetail>>(GetReaderValue<string>(reader, "ErrorDetails")),
                SentToSupplier = GetReaderValue<bool>(reader, "SentToSupplier")
			};
		}

		#endregion
	}
}
