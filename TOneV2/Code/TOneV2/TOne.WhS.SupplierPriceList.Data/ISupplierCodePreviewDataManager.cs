using System;
using System.Collections.Generic;
using Vanrise.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
	public interface ISupplierCodePreviewDataManager : IDataManager, IBulkApplyDataManager<CodePreview>
	{
		long ProcessInstanceId { set; }

		void ApplyPreviewCodesToDB(object preparedCodes);

		IEnumerable<CodePreview> GetFilteredCodePreview(SPLPreviewQuery query);
	}
	public interface IReceivedPricelistManagerTemp : IDataManager
	{
		bool InsertReceivedPricelist(int supplierId, long? fileId, DateTime receivedDate, SupplierPriceListType? pricelistType, ReceivedPricelistStatus status, IEnumerable<SPLImportErrorDetail> errors, out int recordId);
		void SetReceivedPricelistAsStarted(int receivedPricelistRecordId, ReceivedPricelistStatus status, long processInstanceId, DateTime startProcessingTime);
		void SetReceivedPricelistAsCompleted(int receivedPricelistRecordId, ReceivedPricelistStatus status, int pricelistId);
		void UpdateReceivePricelistStatus(int receivedPricelistRecordId, ReceivedPricelistStatus status);
	}
}