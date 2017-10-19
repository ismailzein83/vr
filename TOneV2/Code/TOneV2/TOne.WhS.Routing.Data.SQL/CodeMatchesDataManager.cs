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
        private readonly string[] codeSaleZoneMatchColumns = { "Code", "SellingNumberPlanID", "SaleZoneID", "CodeMatch" };
        private readonly string[] codeSupplierZoneMatchColumns = { "Code", "SupplierID", "SupplierZoneID", "CodeMatch" };

        const char SupplierCodeMatchesWithRateSeparator = '|';
        const char SupplierCodeMatchWithRatePropertiesSeparator = '~';
        const char SupplierCodeMatchPropertiesSeparator = '$';
        const char SupplierServicesSeparator = '#';
        const string SupplierServicesSeparatorAsString = "#";
        const char ExactSupplierServicesSeparator = '#';
        const string ExactSupplierServicesSeparatorAsString = "#";

        #region Public Methods

        public bool ShouldApplyCodeZoneMatch { get; set; }

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
            string supplierCodeMatchesWithRate = this.SerializeSupplierCodeMatches(record.SupplierCodeMatches);

            var codeMatchBulkInsert = dbApplyStream as CodeMatchBulkInsert;
            codeMatchBulkInsert.CodeMatchStream.WriteRecord("{0}^{1}^{2}", record.CodePrefix, record.Code, supplierCodeMatchesWithRate);

            if (ShouldApplyCodeZoneMatch)
            {
                if (record.SaleCodeMatches != null)
                {
                    foreach (SaleCodeMatch saleCodeMatch in record.SaleCodeMatches)
                        codeMatchBulkInsert.CodeSaleZoneMatchStream.WriteRecord("{0}^{1}^{2}^{3}", record.Code, saleCodeMatch.SellingNumberPlanId, saleCodeMatch.SaleZoneId, saleCodeMatch.SaleCode);
                }
                if (record.SupplierCodeMatches != null)
                {
                    foreach (SupplierCodeMatchWithRate supplierCodeMatch in record.SupplierCodeMatches)
                        codeMatchBulkInsert.CodeSupplierZoneMatchStream.WriteRecord("{0}^{1}^{2}^{3}", record.Code, supplierCodeMatch.CodeMatch.SupplierId, supplierCodeMatch.CodeMatch.SupplierZoneId, supplierCodeMatch.CodeMatch.SupplierCode);
                }
            }
        }

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

        #endregion

        #region Private Methods

        private RPCodeMatches RPCodeMatchesMapper(IDataReader reader)
        {
            string supplierCodeMatchesWithRate = reader["Content"] as string;

            return new RPCodeMatches()
            {
                Code = reader["Code"] as string,
                SupplierCodeMatches = this.DeserializeSupplierCodeMatches(supplierCodeMatchesWithRate),
                SaleZoneId = (long)reader["SaleZoneId"]
            };
        }

        /// <summary>
        /// CM~RateValue~ServiceId1#...#ServiceIdn~ExactServiceId1#...#ExactServiceIdn~...~SupplierRateEED|CM~RateValue~ServiceId1#...#ServiceIdn~ExactServiceId1#...#ExactServiceIdn~...~SupplierRateEED
        /// CM --> SupplierId$SupplierZoneId$SupplierCode$SupplierCodeSourceId
        /// </summary>
        /// <param name="supplierCodeMatchesWithRate"></param>
        /// <returns></returns>
        private string SerializeSupplierCodeMatches(List<SupplierCodeMatchWithRate> supplierCodeMatchesWithRate)
        {
            StringBuilder str = new StringBuilder();

            foreach (var item in supplierCodeMatchesWithRate)
            {
                if (str.Length > 0)
                    str.Append(SupplierCodeMatchesWithRateSeparator);

                SupplierCodeMatch supplierCodeMatch = item.CodeMatch;
                string serializedSupplierCodeMatch = string.Format("{1}{0}{2}{0}{3}{0}{4}", SupplierCodeMatchPropertiesSeparator, supplierCodeMatch.SupplierId,
                                                        supplierCodeMatch.SupplierZoneId, supplierCodeMatch.SupplierCode, supplierCodeMatch.SupplierCodeSourceId);

                string serializedSupplierServiceIds = string.Empty;
                if (item.SupplierServiceIds != null)
                    serializedSupplierServiceIds = string.Join(SupplierServicesSeparatorAsString, item.SupplierServiceIds);

                string serializedExactSupplierServiceIds = string.Empty;
                if (item.ExactSupplierServiceIds != null)
                    serializedExactSupplierServiceIds = string.Join(ExactSupplierServicesSeparatorAsString, item.ExactSupplierServiceIds);

                str.AppendFormat("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", SupplierCodeMatchWithRatePropertiesSeparator, serializedSupplierCodeMatch, item.RateValue,
                                    serializedSupplierServiceIds, serializedExactSupplierServiceIds, item.SupplierServiceWeight, item.SupplierRateId, item.SupplierRateEED);
            }
            return str.ToString();
        }

        public List<SupplierCodeMatchWithRate> DeserializeSupplierCodeMatches(string serializedSupplierCodeMatches)
        {
            if (string.IsNullOrEmpty(serializedSupplierCodeMatches))
                return null;

            List<SupplierCodeMatchWithRate> supplierCodeMatches = new List<SupplierCodeMatchWithRate>();

            string[] lines = serializedSupplierCodeMatches.Split(SupplierCodeMatchesWithRateSeparator);

            foreach (var line in lines)
            {
                string[] parts = line.Split(SupplierCodeMatchWithRatePropertiesSeparator);

                var supplierCodeMatchWithRate = new SupplierCodeMatchWithRate();
                supplierCodeMatchWithRate.RateValue = decimal.Parse(parts[1]);
                supplierCodeMatchWithRate.SupplierServiceWeight = int.Parse(parts[4]);
                supplierCodeMatchWithRate.SupplierRateId = long.Parse(parts[5]);
                supplierCodeMatchWithRate.SupplierRateEED = !string.IsNullOrEmpty(parts[6]) ? DateTime.Parse(parts[6]) : default(DateTime?);

                string supplierCodeMatchAsString = parts[0];
                if (!string.IsNullOrEmpty(supplierCodeMatchAsString))
                {
                    string[] supplierCodeMatchZones = supplierCodeMatchAsString.Split(SupplierCodeMatchPropertiesSeparator);

                    SupplierCodeMatch supplierCodeMatch = new SupplierCodeMatch();
                    supplierCodeMatch.SupplierId = int.Parse(supplierCodeMatchZones[0]);
                    supplierCodeMatch.SupplierZoneId = long.Parse(supplierCodeMatchZones[1]);
                    supplierCodeMatch.SupplierCode = supplierCodeMatchZones[2] as string;
                    supplierCodeMatch.SupplierCodeSourceId = supplierCodeMatchZones[3] as string;

                    supplierCodeMatchWithRate.CodeMatch = supplierCodeMatch;
                }

                if (!string.IsNullOrEmpty(parts[2]))
                    supplierCodeMatchWithRate.SupplierServiceIds = new HashSet<int>(parts[2].Split(SupplierServicesSeparator).Select(itm => int.Parse(itm)));

                if (!string.IsNullOrEmpty(parts[3]))
                    supplierCodeMatchWithRate.ExactSupplierServiceIds = new HashSet<int>(parts[3].Split(ExactSupplierServicesSeparator).Select(itm => int.Parse(itm)));

                supplierCodeMatches.Add(supplierCodeMatchWithRate);
            }

            return supplierCodeMatches;
        }

        #endregion

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

        #region Queries

        const string query_GetCodeMatchesByZone = @"SELECT  cm.Code, 
                                                            cm.Content, 
                                                            sz.SaleZoneID
                                                    FROM    [dbo].[CodeMatch] cm with(nolock)
                                                    join    CodeSaleZone sz on sz.code = cm.code 
                                                    where   sz.SaleZoneId between @FromZoneId and @ToZoneId";

        #endregion
    }
}
