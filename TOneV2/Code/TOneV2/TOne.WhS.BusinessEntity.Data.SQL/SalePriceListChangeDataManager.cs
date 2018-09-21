using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
	public class SalePriceListChangeDataManager : BaseSQLDataManager, ISalePriceListChangeDataManager
	{
		private readonly string[] _salePricelistCodeChangeColumns =
		{
			"Code","RecentZoneName","ZoneName","ZoneID","Change","BatchID","BED","EED","CountryID"
		};
		private readonly string[] _salePricelistRateChangeColumns =
		{
			"PricelistId","Rate","RateTypeId","RecentCurrencyId","RecentRate","RecentRateConverted","CountryID","ZoneName","ZoneID","Change","ProcessInstanceID","BED","EED","RoutingProductID","CurrencyID"
		};
		private readonly string[] _salePricelistCustomerChangeColumns =
		{
		   "BatchID","PricelistID","CountryID","CustomerID"
		};
		private readonly string[] _salePricelistRPChangeColumns =
		{
			"ZoneName","ZoneID","RoutingProductId","RecentRoutingProductId","BED","EED","PriceListId","CountryId","ProcessInstanceID","CustomerId"
		};
		private readonly string[] _salePriceListSnapshot =
		{
			"PriceListID","SnapShotDetail"
		};
		public SalePriceListChangeDataManager()
			: base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
		{

		}
		#region public Methods
		public List<SalePricelistCodeChange> GetFilteredSalePricelistCodeChanges(int pricelistId, List<int> countryIds)
		{
			string strcountryIds = null;
			if (countryIds != null && countryIds.Count > 0)
				strcountryIds = string.Join(",", countryIds);
			return GetItemsSP("TOneWhS_BE.sp_SalePricelistCodeChange_GetFiltered", SalePricelistCodeChangeMapper,
				pricelistId, strcountryIds);
		}
		public List<Entities.SalePriceListNew> GetTemporaryPriceLists(TemporarySalePriceListQuery query)
		{
			return GetItemsSP("[TOneWhS_BE].[sp_SalePriceListNew_GetAll]", TemporarySalePriceListMapper, query.ProcessInstanceId);
		}
		public bool DoCustomerTemporaryPricelistsExists(long processInstanceId)
		{
			//  return GetItemSP("[TOneWhS_BE].[sp_SalePriceListNew_Exists]", (reader) => { return GetReaderValue<bool>(reader, "PriceListExists"); }, processInstanceId);
			return (bool)ExecuteScalarSP("[TOneWhS_BE].[sp_SalePriceListNew_Exists]", processInstanceId);
		}
		public List<SalePricelistRateChange> GetFilteredSalePricelistRateChanges(int pricelistId, List<int> countryIds)
		{
			string strcountryIds = null;
			if (countryIds != null && countryIds.Count > 0)
				strcountryIds = string.Join(",", countryIds);
			return GetItemsSP("TOneWhS_BE.sp_SalePricelistRateChange_GetFiltered", SalePricelistRateChangeMapper,
				pricelistId, strcountryIds);
		}
		public List<SalePricelistRPChange> GetFilteredSalePriceListRPChanges(int pricelistId, List<int> countryIds)
		{
			string strcountryIds = null;
			if (countryIds != null && countryIds.Count > 0)
				strcountryIds = string.Join(",", countryIds);
			return GetItemsSP("TOneWhS_BE.sp_SalePricelistRPChange_GetFiltered", SalePricelistRPChangeMapper, pricelistId, strcountryIds);
		}
		public SalePriceListSnapShot GetSalePriceListSnapShot(int priceListId)
		{
			return GetItemSP("TOneWhS_BE.sp_SalePricelistSnapShot", SalePricelistSnapShotMapper, priceListId);
		}

		public IEnumerable<CustomerRatePreview> GetCustomerRatePreviews(CustomerRatePreviewQuery query)
		{
			string strcustomerIds = null;
			if (query.CustomerIds != null && query.CustomerIds.Any())
				strcustomerIds = string.Join(",", query.CustomerIds);
			return GetItemsSP("TOneWhS_BE.sp_SalePriceListRateChangeNew_GetCustomerRatePreviews", CustomerRatePreviewMapper, query.ProcessInstanceId, strcustomerIds);
		}

		public IEnumerable<RoutingProductPreview> GetRoutingProductPreviews(RoutingProductPreviewQuery query)
		{
			string strcustomerIds = null;
			if (query.CustomerIds != null && query.CustomerIds.Any())
				strcustomerIds = string.Join(",", query.CustomerIds);
			return GetItemsSP("TOneWhS_BE.sp_SalePriceListRPChangeNew_GetRoutingProductPreviews", RoutingProductPreviewMapper, query.ProcessInstanceId, strcustomerIds);
		}

		public List<SalePricelistCodeChange> GetNotSentCodechanges(IEnumerable<int> customerIds)
		{
			string strcustomerIds = null;
			if (customerIds != null && customerIds.Any())
				strcustomerIds = string.Join(",", customerIds);
			return GetItemsSP("TOneWhS_BE.sp_SalePriceListChange_GetNotSentCodeChanges", SalePricelistCodeChangeMapper, strcustomerIds);
		}

		public List<SalePricelistRateChange> GetNotSentRatechanges(IEnumerable<int> customerIds)
		{
			string strcustomerIds = null;
			if (customerIds != null && customerIds.Any())
				strcustomerIds = string.Join(",", customerIds);
			return GetItemsSP("TOneWhS_BE.sp_SalePriceListChange_GetNotSentRateChanges", SalePricelistRateChangeMapper, strcustomerIds);
		}
		public void SaveCustomerChangesToDb(IEnumerable<SalePriceListCustomerChange> salePriceLists)
		{
			if (salePriceLists == null || !salePriceLists.Any()) return;
			Object dbApplyStream = InitialiazeStreamForDBApply();
			foreach (SalePriceListCustomerChange salePriceList in salePriceLists)
				WriteRecordToStream(salePriceList, dbApplyStream);
			Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream, "TOneWhS_BE.SalePricelistCustomerChange_New", _salePricelistCustomerChangeColumns);
			ApplyChangesToDataBase(preparedSalePriceLists);
		}
		public void SaveCustomerCodeChangesToDb(IEnumerable<SalePricelistCodeChange> codeChanges)
		{
			if (codeChanges == null || !codeChanges.Any()) return;
			Object dbApplyStream = InitialiazeStreamForDBApply();
			foreach (SalePricelistCodeChange codeChange in codeChanges)
				WriteRecordToStream(codeChange, dbApplyStream);
			Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream, "TOneWhS_BE.SalePricelistCodeChange_New", _salePricelistCodeChangeColumns);
			ApplyChangesToDataBase(preparedSalePriceLists);
		}
		public void SaveCustomerRateChangesToDb(IEnumerable<SalePricelistRateChange> rateChanges, long processInstanceId)
		{
			if (rateChanges == null || !rateChanges.Any()) return;
			Object dbApplyStream = InitialiazeStreamForDBApply();
			foreach (SalePricelistRateChange rateChange in rateChanges)
				WriteRecordToStream(rateChange, dbApplyStream, processInstanceId);
			Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream, "TOneWhS_BE.SalePricelistRateChange_New", _salePricelistRateChangeColumns);
			ApplyChangesToDataBase(preparedSalePriceLists);
		}
		public void SaveCustomerRoutingProductChangesToDb(IEnumerable<SalePricelistRPChange> routingProductChanges, long processInstanceId)
		{
			if (routingProductChanges == null || !routingProductChanges.Any()) return;
			Object dbApplyStream = InitialiazeStreamForDBApply();
			foreach (SalePricelistRPChange routingProductChange in routingProductChanges)
				WriteRecordToStream(routingProductChange, dbApplyStream, processInstanceId, routingProductChange.CustomerId);
			Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream, "TOneWhS_BE.SalePricelistRPChange_New", _salePricelistRPChangeColumns);
			ApplyChangesToDataBase(preparedSalePriceLists);
		}

		public IEnumerable<int> GetAffectedCustomerIdsRPChangesByProcessInstanceId(long ProcessInstanceId)
		{
			List<int> lstAffectedCustomerIds = new List<int>();
			ExecuteReaderSP("TOneWhS_BE.SP_SalePricelistRPChangesNew_GetAffectedCustomerIds", (reader) =>
			{
				while (reader.Read())
				{
					lstAffectedCustomerIds.Add(GetReaderValue<int>(reader, "CustomerId"));
				}
			}, ProcessInstanceId);
			return lstAffectedCustomerIds;
		}
		public IEnumerable<int> GetAffectedCustomerIdsNewCountryChangesByProcessInstanceId(long ProcessInstanceId)
		{
			List<int> listAffectedCustomerIds = new List<int>();
			ExecuteReaderSP("TOneWhS_BE.SP_SalePricelistNewCountryChanges_GetAffectedCustomerIds", (reader) =>
			{
				while (reader.Read())
				{
					listAffectedCustomerIds.Add(GetReaderValue<int>(reader, "CustomerId"));
				}
			}, ProcessInstanceId);
			return listAffectedCustomerIds;
		}
		public IEnumerable<int> GetAffectedCustomerIdsChangedCountryChangesByProcessInstanceId(long ProcessInstanceId)
		{
			List<int> listAffectedCustomerIds = new List<int>();
			ExecuteReaderSP("TOneWhS_BE.SP_SalePricelistChangedCountryChanges_GetAffectedCustomerIds", (reader) =>
			{
				while (reader.Read())
				{
					listAffectedCustomerIds.Add(GetReaderValue<int>(reader, "CustomerId"));
				}
			}, ProcessInstanceId);
			return listAffectedCustomerIds;
		}
		public IEnumerable<int> GetAffectedCustomerIdsRateChangesByProcessInstanceId(long ProcessInstanceId)
		{
			List<int> lstAffectedCustomerIds = new List<int>();
			ExecuteReaderSP("TOneWhS_BE.SP_SalePricelistRateChangesNew_GetAffectedCustomerIds", (reader) =>
			{
				while (reader.Read())
				{
					lstAffectedCustomerIds.Add(GetReaderValue<int>(reader, "OwnerID"));
				}
			}, ProcessInstanceId);
			return lstAffectedCustomerIds;
		}

		public bool AreSalePriceListCodeSnapShotUpdated(ref object updateHandle)
		{
			return base.IsDataUpdated("TOneWhS_BE.SalePriceListSnapShot", ref updateHandle);

		}
		public void SaveSalePriceListSnapshotToDb(IEnumerable<SalePriceListSnapShot> salePriceListSaleCodeSnapshots)
		{
			if (salePriceListSaleCodeSnapshots == null || !salePriceListSaleCodeSnapshots.Any())
				return;

			object dbApplyStrem = InitialiazeStreamForDBApply();
			foreach (var salePriceListSaleCodeSnapshot in salePriceListSaleCodeSnapshots)
				WriteRecordToStream(salePriceListSaleCodeSnapshot, dbApplyStrem);
			object preparedSnapshot = FinishDBApplyStream(dbApplyStrem, "TOneWhS_BE.SalePriceListSnapShot", _salePriceListSnapshot);
			ApplyChangesToDataBase(preparedSnapshot);
		}
		#endregion
		#region Bulk Insert
		private void WriteRecordToStream(SalePriceListCustomerChange record, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			if (streamForBulkInsert != null)
				streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}",
					record.BatchId,
					record.PriceListId,
					record.CountryId,
					record.CustomerId
					);
		}
		private void WriteRecordToStream(SalePricelistCodeChange record, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			if (streamForBulkInsert != null)
				streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}",
					record.Code,
					record.RecentZoneName,
					record.ZoneName,
					record.ZoneId,
					(int)record.ChangeType,
					record.BatchId,
					GetDateTimeForBCP(record.BED),
					GetDateTimeForBCP(record.EED),
					record.CountryId);
		}
		private void WriteRecordToStream(SalePricelistRPChange record, object dbApplyStream, long processInstanceId, int customerId)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			if (streamForBulkInsert != null)
				streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}",
					record.ZoneName,
					record.ZoneId,
					record.RoutingProductId,
					record.RecentRoutingProductId,
					GetDateTimeForBCP(record.BED),
					GetDateTimeForBCP(record.EED),
					record.PriceListId,
					record.CountryId,
					processInstanceId,
					customerId);
		}
		private void WriteRecordToStream(SalePricelistRateChange record, object dbApplyStream, long processInstanceId)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			if (streamForBulkInsert != null)
				streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}",
					record.PricelistId,
					decimal.Round(record.Rate, 8),
					record.RateTypeId,
					record.RecentCurrencyId,
					record.RecentRate.HasValue ? decimal.Round(record.RecentRate.Value, 8) : record.RecentRate,
					record.RecentRateConverted.HasValue ? decimal.Round(record.RecentRateConverted.Value, 8) : record.RecentRateConverted,
					record.CountryId,
					record.ZoneName,
					record.ZoneId,
					(int)record.ChangeType,
					processInstanceId,
					GetDateTimeForBCP(record.BED),
					GetDateTimeForBCP(record.EED),
					record.RoutingProductId,
					record.CurrencyId);
		}
		private void WriteRecordToStream(SalePriceListSnapShot record, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			if (streamForBulkInsert != null)
				streamForBulkInsert.WriteRecord("{0}^{1}",
					record.PriceListId,
					Vanrise.Common.Serializer.Serialize(record.SnapShotDetail));
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
		private object InitialiazeStreamForDBApply()
		{
			return base.InitializeStreamForBulkInsert();
		}
		#endregion
		#region Mappers

		SalePricelistCodeChange SalePricelistCodeChangeMapper(IDataReader reader)
		{
			SalePricelistCodeChange salePricelistCodeChange = new SalePricelistCodeChange
			{
				PricelistId = GetReaderValue<int>(reader, "PricelistID"),
				Code = GetReaderValue<string>(reader, "Code"),
				CountryId = GetReaderValue<int>(reader, "CountryID"),
				RecentZoneName = GetReaderValue<string>(reader, "RecentZoneName"),
				ZoneName = GetReaderValue<string>(reader, "ZoneName"),
				ChangeType = (CodeChange)GetReaderValue<byte>(reader, "Change"),
				BED = GetReaderValue<DateTime>(reader, "BED"),
				EED = GetReaderValue<DateTime?>(reader, "EED"),
				ZoneId = GetReaderValue<long?>(reader, "ZoneID")
			};
			return salePricelistCodeChange;
		}
		SalePricelistRateChange SalePricelistRateChangeMapper(IDataReader reader)
		{
			return new SalePricelistRateChange
			{
				PricelistId = GetReaderValue<int>(reader, "PricelistID"),
				CountryId = GetReaderValue<int>(reader, "CountryID"),
				ZoneName = GetReaderValue<string>(reader, "ZoneName"),
				Rate = GetReaderValue<decimal>(reader, "Rate"),
				RateTypeId = GetReaderValue<int?>(reader, "RateTypeId"),
				RecentRate = GetReaderValue<decimal?>(reader, "RecentRate"),
				ChangeType = (RateChangeType)GetReaderValue<byte>(reader, "Change"),
				BED = GetReaderValue<DateTime>(reader, "BED"),
				EED = GetReaderValue<DateTime?>(reader, "EED"),
				RoutingProductId = GetReaderValue<int>(reader, "RoutingProductID"),
				CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
				ZoneId = GetReaderValue<long?>(reader, "ZoneID")
			};
		}
		SalePricelistRPChange SalePricelistRPChangeMapper(IDataReader reader)
		{
			SalePricelistRPChange salePricelistRpChange = new SalePricelistRPChange
			{
				ZoneName = GetReaderValue<string>(reader, "ZoneName"),
				ZoneId = GetReaderValue<long?>(reader, "ZoneID"),
				RoutingProductId = GetReaderValue<int>(reader, "RoutingProductId"),
				RecentRoutingProductId = GetReaderValue<int>(reader, "RecentRoutingProductId"),
				BED = GetReaderValue<DateTime>(reader, "BED"),
				EED = GetReaderValue<DateTime?>(reader, "EED"),
				CountryId = GetReaderValue<int>(reader, "CountryId"),
				CustomerId = GetReaderValue<int>(reader, "CustomerId")
			};
			return salePricelistRpChange;
		}
		RoutingProductPreview RoutingProductPreviewMapper(IDataReader reader)
		{
			RoutingProductPreview routingProductPreview = new RoutingProductPreview
			{
				ZoneName = GetReaderValue<string>(reader, "ZoneName"),
				ZoneId = GetReaderValue<long?>(reader, "ZoneID"),
				RoutingProductId = GetReaderValue<int>(reader, "RoutingProductId"),
				RecentRoutingProductId = GetReaderValue<int>(reader, "RecentRoutingProductId"),
				BED = GetReaderValue<DateTime>(reader, "BED"),
				EED = GetReaderValue<DateTime?>(reader, "EED"),
				CountryId = GetReaderValue<int>(reader, "CountryId"),
				CustomerId = GetReaderValue<int>(reader, "CustomerId")
			};
			return routingProductPreview;
		}
		SalePriceListSnapShot SalePricelistSnapShotMapper(IDataReader reader)
		{
			return new SalePriceListSnapShot
			{
				PriceListId = GetReaderValue<int>(reader, "PriceListID"),
				SnapShotDetail = Vanrise.Common.Serializer.Deserialize<SnapShotDetail>(reader["SnapShotDetail"] as string)
			};
		}
		private CustomerRatePreview CustomerRatePreviewMapper(IDataReader reader)
		{
			return new CustomerRatePreview()
			{
				ZoneName = reader["ZoneName"] as string,
				ZoneId = GetReaderValue<long?>(reader, "ZoneID"),
				RoutingProductId = GetReaderValue<int>(reader, "RoutingProductID"),
				CountryId = GetReaderValue<int>(reader, "CountryID"),
				RecentCurrencyId = GetReaderValue<int?>(reader, "RecentCurrencyId"),
				RecentRate = GetReaderValue<decimal?>(reader, "RecentRate"),
				RecentRateConverted = GetReaderValue<decimal?>(reader, "RecentRateConverted"),
				Rate = GetReaderValue<decimal>(reader, "Rate"),
				ChangeType = (RateChangeType)GetReaderValue<byte>(reader, "Change"),
				PricelistId = GetReaderValue<int>(reader, "PricelistId"),
				BED = GetReaderValue<DateTime>(reader, "BED"),
				EED = GetReaderValue<DateTime?>(reader, "EED"),
				CurrencyId = GetReaderValue<int?>(reader, "CurrencyID"),
				CustomerId = GetReaderValue<int>(reader, "OwnerID")
			};
		}
		SalePriceListNew TemporarySalePriceListMapper(IDataReader reader)
		{
			SalePriceListNew salePriceList = new SalePriceListNew
			{
				OwnerId = (int)reader["OwnerID"],
				CurrencyId = (int)reader["CurrencyID"],
				PriceListId = (int)reader["ID"],
				OwnerType = (Entities.SalePriceListOwnerType)GetReaderValue<int>(reader, "OwnerType"),
				EffectiveOn = GetReaderValue<DateTime>(reader, "EffectiveOn"),
				PriceListType = (SalePriceListType?)GetReaderValue<byte?>(reader, "PriceListType"),
				ProcessInstanceId = GetReaderValue<long>(reader, "ProcessInstanceID"),
				FileId = GetReaderValue<long>(reader, "FileID"),
				CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
				SourceId = GetReaderValue<string>(reader, "SourceID"),
				UserId = GetReaderValue<int>(reader, "UserID"),
				Description = GetReaderValue<string>(reader, "Description"),
				PricelistStateBackupId = GetReaderValue<long?>(reader, "PricelistStateBackupID"),
			};

			return salePriceList;
		}
		#endregion

	}
}
