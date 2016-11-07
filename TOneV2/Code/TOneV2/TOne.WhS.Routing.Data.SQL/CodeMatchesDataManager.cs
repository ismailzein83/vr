using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
	public class CodeMatchesDataManager : RoutingDataManager, ICodeMatchesDataManager
	{
		private readonly string[] codeMatchColumns = { "CodePrefix", "Code", "Content" };
		private readonly string[] codeSaleZoneMatchColumns = { "Code", "SellingNumberPlanID", "SaleZoneID" };
		private readonly string[] codeSupplierZoneMatchColumns = { "Code", "SupplierID", "SupplierZoneID" };

		public bool ShouldApplyCodeZoneMatch { get; set; }

		public void ApplyCodeMatchesForDB(object preparedData)
		{
			var codeMatchBulkInsertInfo = preparedData as CodeMatchBulkInsertInfo;
			int count = ShouldApplyCodeZoneMatch ? 3 : 1;

			Parallel.For(0, count, (i) =>
			{
				switch (i)
				{
					case 0: base.InsertBulkToTable(codeMatchBulkInsertInfo.CodeMatchesBulkInsertInfo); break;
					case 1: base.InsertBulkToTable(codeMatchBulkInsertInfo.CodeSaleZoneMatchBulkInsertInfo); break;
					case 2: base.InsertBulkToTable(codeMatchBulkInsertInfo.CodeSupplierZoneMatchBulkInsertInfo); break;
				}
			});
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			var codeMatchBulkInsertInfo = new CodeMatchBulkInsertInfo();
			var codeMatchBulkInsert = dbApplyStream as CodeMatchBulkInsert;

			codeMatchBulkInsert.CodeMatchStream.Close();
			codeMatchBulkInsertInfo.CodeMatchesBulkInsertInfo = new StreamBulkInsertInfo()
			{
				TableName = "[dbo].[CodeMatch]",
				Stream = codeMatchBulkInsert.CodeMatchStream,
				TabLock = true,
				KeepIdentity = false,
				FieldSeparator = '^',
				ColumnNames = codeMatchColumns
			};

			if (ShouldApplyCodeZoneMatch)
			{
				codeMatchBulkInsert.CodeSaleZoneMatchStream.Close();
				codeMatchBulkInsertInfo.CodeSaleZoneMatchBulkInsertInfo = new StreamBulkInsertInfo()
				{
					TableName = "[dbo].[CodeSaleZoneMatch]",
					Stream = codeMatchBulkInsert.CodeSaleZoneMatchStream,
					TabLock = true,
					KeepIdentity = false,
					FieldSeparator = '^',
					ColumnNames = codeSaleZoneMatchColumns
				};

				codeMatchBulkInsert.CodeSupplierZoneMatchStream.Close();
				codeMatchBulkInsertInfo.CodeSupplierZoneMatchBulkInsertInfo = new StreamBulkInsertInfo()
				{
					TableName = "[dbo].[CodeSupplierZoneMatch]",
					Stream = codeMatchBulkInsert.CodeSupplierZoneMatchStream,
					TabLock = true,
					KeepIdentity = false,
					FieldSeparator = '^',
					ColumnNames = codeSupplierZoneMatchColumns
				};
			}

			return codeMatchBulkInsertInfo;
		}

		public object InitialiazeStreamForDBApply()
        {
			var codeMatchBulkInsert = new CodeMatchBulkInsert()
			{
				CodeMatchStream = base.InitializeStreamForBulkInsert()
			};
			if (ShouldApplyCodeZoneMatch)
			{
				codeMatchBulkInsert.CodeSaleZoneMatchStream = base.InitializeStreamForBulkInsert();
				codeMatchBulkInsert.CodeSupplierZoneMatchStream = base.InitializeStreamForBulkInsert();
			}
			return codeMatchBulkInsert;
        }

		public void WriteRecordToStream(Entities.CodeMatches record, object dbApplyStream)
		{
			var codeMatchBulkInsert = dbApplyStream as CodeMatchBulkInsert;

			codeMatchBulkInsert.CodeMatchStream.WriteRecord("{0}^{1}^{2}", record.CodePrefix, record.Code, Vanrise.Common.Serializer.Serialize(record.SupplierCodeMatches, true));

			if (ShouldApplyCodeZoneMatch)
			{
				if (record.SaleCodeMatches != null)
				{
					foreach (SaleCodeMatch saleCodeMatch in record.SaleCodeMatches)
						codeMatchBulkInsert.CodeSaleZoneMatchStream.WriteRecord("{0}^{1}^{2}", record.Code, saleCodeMatch.SellingNumberPlanId, saleCodeMatch.SaleZoneId);
				}
				if (record.SupplierCodeMatches != null)
				{
					foreach (SupplierCodeMatchWithRate supplierCodeMatch in record.SupplierCodeMatches)
						codeMatchBulkInsert.CodeSupplierZoneMatchStream.WriteRecord("{0}^{1}^{2}", record.Code, supplierCodeMatch.CodeMatch.SupplierId, supplierCodeMatch.CodeMatch.SupplierZoneId);
				}
			}
		}

		#region Queries

		const string query_GetCodeMatchesByZone = @"                                                       
                                          SELECT  cm.Code, 
                                                  cm.Content, 
                                                  sz.SaleZoneID
                                          FROM    [dbo].[CodeMatch] cm with(nolock)
                                          join    CodeSaleZone sz on sz.code = cm.code 
                                          where   sz.SaleZoneId between @FromZoneId and @ToZoneId";

		#endregion

		public IEnumerable<RPCodeMatches> GetCodeMatches(long fromZoneId, long toZoneId)
		{
			return GetItemsText(query_GetCodeMatchesByZone, RPCodeMatchesMapper, (cmd) =>
			{

				var dtPrm = new SqlParameter("@FromZoneId", SqlDbType.BigInt);
				dtPrm.Value = fromZoneId;
				cmd.Parameters.Add(dtPrm);
				dtPrm = new SqlParameter("@ToZoneId", SqlDbType.BigInt);
				dtPrm.Value = toZoneId;
				cmd.Parameters.Add(dtPrm);
			});
		}
		RPCodeMatches RPCodeMatchesMapper(IDataReader reader)
		{
			return new RPCodeMatches()
			{
				Code = reader["Code"] as string,
				SupplierCodeMatches = reader["Content"] != null ? Vanrise.Common.Serializer.Deserialize<List<SupplierCodeMatchWithRate>>(reader["Content"].ToString()) : null,
				SaleZoneId = (long)reader["SaleZoneId"]
			};
		}

		#region Private Classes

		private class CodeMatchBulkInsert
		{
			public StreamForBulkInsert CodeMatchStream { get; set; }
			public StreamForBulkInsert CodeSaleZoneMatchStream { get; set; }
			public StreamForBulkInsert CodeSupplierZoneMatchStream { get; set; }
		}

		private class CodeMatchBulkInsertInfo
		{
			public StreamBulkInsertInfo CodeMatchesBulkInsertInfo { get; set; }
			public StreamBulkInsertInfo CodeSaleZoneMatchBulkInsertInfo { get; set; }
			public StreamBulkInsertInfo CodeSupplierZoneMatchBulkInsertInfo { get; set; }
		}

		#endregion
	}
}
