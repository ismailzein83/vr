﻿using System;
using System.Data;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;

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
			var serializedErrors = Vanrise.Common.Serializer.Serialize(errors);
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
				//Id = GetReaderValue<int>(reader, "Id"),
				SupplierId = GetReaderValue<int>(reader, "SupplierId"),
				FileId = GetReaderValue<long?>(reader, "FileId"),
				ReceivedDateTime = (DateTime)reader["ReceivedDate"],
				PricelistType = (SupplierPricelistType?)GetReaderValue<int?>(reader, "PricelistType"),
				Status = (ReceivedPricelistStatus)GetReaderValue<int>(reader, "Status"),
				PricelistId = GetReaderValue<int?>(reader, "PricelistId"),
				ProcessInstanceId = GetReaderValue<long?>(reader, "ProcessInstanceId"),
				StartProcessingDateTime = GetReaderValue<DateTime?>(reader, "StartProcessingDate"),
				ErrorDetails = string.IsNullOrEmpty(GetReaderValue<string>(reader, "ErrorDetails")) ? null : Serializer.Deserialize<List<SPLImportErrorDetail>>(GetReaderValue<string>(reader, "ErrorDetails")),
			};
		}

		#endregion
	}
}
