using System;
using System.Data;
using Vanrise.Data.SQL;
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

		#endregion

		#region Mappers
		private ReceivedPricelist SupplierPriceListTemplateMapper(IDataReader reader)
		{
			return new ReceivedPricelist()
			{
				SupplierId = (int)reader["SupplierId"],
				FileId = (long)reader["FileId"],
				ReceivedDate = (DateTime)reader["ReceivedDate"],
				PricelistType = (SupplierPriceListType)reader["ReceivedDate"],
				Status = (ReceivedPricelistStatus)reader["Status"],
				PricelistId = GetReaderValue<int?>(reader, "PricelistId"),
				ProcessInstanceId = GetReaderValue<long?>(reader, "ProcessInstanceId")
			};
		}
		#endregion
	}
}
