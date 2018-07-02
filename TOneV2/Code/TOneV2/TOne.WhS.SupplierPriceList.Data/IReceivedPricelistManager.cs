using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
	public interface IReceivedPricelistManager : IDataManager
	{
		bool InsertReceivedPricelist(ReceivedPricelist receivedPricelist, out int insertedObjectId);
		bool InsertReceivedPricelist(int supplierId, long? fileId, DateTime receivedDate, SupplierPricelistType? pricelistType, ReceivedPricelistStatus status, IEnumerable<SPLImportErrorDetail> errors, out int recordId);
		void SetReceivedPricelistAsStarted(int receivedPricelistRecordId, ReceivedPricelistStatus status, long processInstanceId, DateTime startProcessingTime);
		void SetReceivedPricelistAsCompleted(int receivedPricelistRecordId, ReceivedPricelistStatus status, int pricelistId);
		void UpdateReceivedPricelistStatus(int receivedPricelistRecordId, ReceivedPricelistStatus status);
		bool SetReceivedPricelistAsCompletedManualy(int receivedPricelistRecordId, ReceivedPricelistStatus status);
		void UpdateReceivedPricelistStatus(int receivedPricelistRecordId, ReceivedPricelistStatus status, IEnumerable<SPLImportErrorDetail> errors);
		ReceivedPricelist GetReceivedPricelistById(int id);
		List<ReceivedPricelist> GetFilteredReceivedPricelists(ReceivedPricelistQuery input);
        void UpdateSentToSupplierStatus(int receivedPricelistRecordId, bool status);
	}
}
