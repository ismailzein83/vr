using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Deal.Data.SQL
{
	public class DealZoneRateDataManager : BaseSQLDataManager, IDealZoneRateDataManager
	{
		#region Constructors/Cloumns

		public DealZoneRateDataManager()
			: base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
		{

		}

		readonly string[] _columns = { "DealId", "ZoneGroupNb", "IsSale", "TierNb", "ZoneId", "Rate", "CurrencyId", "BED", "EED" };

		#endregion

		#region Public Methods

		public IEnumerable<DealZoneRate> GetDealZoneRatesByDate(bool isSale, DateTime fromDate, DateTime toDate)
		{
			return GetItemsSP("TOneWhS_Deal.sp_DealZoneRate_GetDealZoneRatesByDate", DealZoneRateMapper, isSale, fromDate, toDate);
		}

		public IEnumerable<DealZoneRate> GetDealZoneRatesByDealIds(bool isSale, IEnumerable<int> dealIds)
		{
			string strDealIds = null;
			if (dealIds != null && dealIds.Count() > 0)
				strDealIds = string.Join(",", dealIds);
			return GetItemsSP("TOneWhS_Deal.sp_DealZoneRate_GetDealZoneRatesByDealIds", DealZoneRateMapper, isSale, strDealIds);
		}

		#region BCP
		public object FinishDBApplyStream(object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new StreamBulkInsertInfo
			{
				ColumnNames = _columns,
				TableName = "TOneWhS_Deal.DealZoneRate_Temp",
				Stream = streamForBulkInsert,
				TabLock = false,
				KeepIdentity = false,
				FieldSeparator = '^',
			};
		}

		public object InitialiazeStreamForDBApply()
		{
			return base.InitializeStreamForBulkInsert();
		}

		public void WriteRecordToStream(DealZoneRate record, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}",
				record.DealId,
				record.ZoneGroupNb,
				record.IsSale,
				record.TierNb,
				record.ZoneId,
				record.Rate,
				record.CurrencyId,
				record.BED,
				record.EED);
		}
		public void ApplyNewDealZoneRatesToDB(object preparedRates)
		{
			InsertBulkToTable(preparedRates as BaseBulkInsertInfo);
		}

		public bool AreDealZoneRateUpdated(ref object updateHandle)
		{
			return base.IsDataUpdated("TOneWhS_Deal.DealZoneRate", ref updateHandle);
		}

		#endregion

		#region TempTableFunctions

		public void InitializeDealZoneRateInsert(IEnumerable<int> dealIdsToKeep)
		{
			var queryString = query_CreateTempTables.Replace("#UniqueId#", Guid.NewGuid().ToString());
			ExecuteNonQueryText(queryString, null);

			if (dealIdsToKeep != null || dealIdsToKeep.Count() == 0)
				return;

			string dealIdsToKeepAsString = string.Format(" ({0}) ", string.Join(",", dealIdsToKeep));
			var query = syncExistingRates.Replace("#DealIdsToKeep#", dealIdsToKeepAsString);

			ExecuteNonQueryText(query, null);
		}

		public void FinalizeDealZoneRateInsert()
		{
			ExecuteNonQueryText(query_SwapTables, null);
		}

		#endregion

		#region Create/Swap/Insert queries
		const string query_SwapTables = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[DealZoneRate]') AND s.type in (N'U'))
		                                        begin
			                                        DROP TABLE [dbo].[DealZoneRate]
		                                        end
	                                        EXEC sp_rename 'DealZoneRate_Temp', 'DealZoneRate'";



		const string query_CreateTempTables = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[DealZoneRate_temp]') AND s.type in (N'U'))
                                                        begin
                                                            DROP TABLE [dbo].[DealZoneRate_Temp]
                                                        end
												CREATE TABLE [TOneWhS_Deal].[DealZoneRate_Temp](
													[ID] [bigint] NOT NULL,
													[DealId] [int] NOT NULL,
													[ZoneGroupNb] [int] NOT NULL,
													[IsSale] [bit] NOT NULL,
													[TierNb] [int] NOT NULL,
													[ZoneId] [bigint] NOT NULL,
													[Rate] [decimal](20, 8) NOT NULL,
													[CurrencyId] [int] NOT NULL,
													[BED] [datetime] NOT NULL,
													[EED] [datetime] NULL,
													[timestamp] [timestamp] NULL,
												 CONSTRAINT [PK_DealZoneRate_#UniqueId#] PRIMARY KEY CLUSTERED 
												(
													[ID] ASC
												)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
												) ON [PRIMARY]
												GO

												CREATE NONCLUSTERED INDEX [IX_DealZoneRate_BED_#UniqueId#] ON [TOneWhS_Deal].[DealZoneRate_Temp]
												(
													[BED] ASC
												)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
												GO

												CREATE NONCLUSTERED INDEX [IX_DealZoneRate_DealId_#UniqueId#] ON [TOneWhS_Deal].[DealZoneRate_Temp]
												(
													[DealId] ASC
												)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
												GO

												CREATE NONCLUSTERED INDEX [IX_DealZoneRate_EED_#UniqueId#] ON [TOneWhS_Deal].[DealZoneRate_Temp]
												(
													[EED] ASC
												)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
												GO

												CREATE NONCLUSTERED INDEX [IX_DealZoneRate_IsSale_#UniqueId#] ON [TOneWhS_Deal].[DealZoneRate_Temp]
												(
													[IsSale] ASC
												)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
												GO";

		const string syncExistingRates = @"IF  (
												EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[DealZoneRate]') AND s.type in (N'U'))
		                                        and EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[DealZoneRate_Temp]') AND s.type in (N'U'))
											   )
												begin
												INSERT INTO dbo.[DealZoneRate_Temp] ([ID],[DealId],[ZoneGroupNb],[IsSale],[TierNb],[ZoneId],[Rate],[CurrencyId],[BED],[EED])
												SELECT [ID],[DealId],[ZoneGroupNb],[IsSale],[TierNb],[ZoneId],[Rate],[CurrencyId],[BED],[EED] from dbo.[DealZoneRate] dzr
												WHERE dzr.[DealId] IN #DealIdsToKeep#
		                                        end";

		#endregion

		#endregion


		#region  Mappers

		private DealZoneRate DealZoneRateMapper(IDataReader reader)
		{
			DealZoneRate dealZoneRate = new DealZoneRate
			{
				DealZoneRateId = (int)reader["ID"],
				DealId = (int)reader["DealId"],
				ZoneGroupNb = (int)reader["ZoneGroupNb"],
				IsSale = (bool)reader["IsSale"],
				TierNb = (int)reader["TierNb"],
				ZoneId = (int)reader["ZoneId"],
				Rate = (decimal)reader["Rate"],
				CurrencyId = (int)reader["CurrencyId"],
				BED = (DateTime)reader["BED"],
				EED = GetReaderValue<DateTime?>(reader, "EED")
			};
			return dealZoneRate;
		}
		#endregion

	}
}