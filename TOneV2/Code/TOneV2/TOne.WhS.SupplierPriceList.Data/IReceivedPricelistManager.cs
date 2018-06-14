using System;
using System.Collections.Generic;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
	public interface IReceivedPricelistManager : IDataManager
	{
		bool InsertReceivedPricelist(ReceivedPricelist receivedPricelist, out int insertedObjectId);
		bool InsertReceivedPricelist(int supplierId, long fileId, DateTime receivedDate, SupplierPriceListType pricelistType, ReceivedPricelistStatus status, int? pricelistId, long? processInstanceId, out int insertedObjectId);
		void UpdateReceivedPricelistStatus(int receivedPricelistId, ReceivedPricelistStatus status);
		void UpdateReceivedPricelistInfos(int receivedPricelistId, ReceivedPricelistStatus status, int pricelistId, long processInstanceId);
        List<ReceivedPricelist> GetFilteredReceivedPricelists(ReceivedPricelistQuery input);
	}
}
