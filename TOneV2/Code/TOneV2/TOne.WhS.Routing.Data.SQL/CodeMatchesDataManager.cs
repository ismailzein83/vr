using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;
using System.Globalization;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CodeMatchesDataManager : RoutingDataManager, ICodeMatchesDataManager
    {
        //private readonly string[] codeMatchColumns = { "CodePrefix", "Code", "Content" };
        private readonly string[] codeSaleZoneMatchColumns = { "Code", "SellingNumberPlanID", "SaleZoneID", "CodeMatch" };
        private readonly string[] codeSupplierZoneMatchColumns = { "Code", "SupplierID", "SupplierZoneID", "CodeMatch" };

        //const char SupplierCodeMatchesWithRateSeparator = '|';
        //const char SupplierCodeMatchWithRatePropertiesSeparator = '~';
        //const char SupplierCodeMatchPropertiesSeparator = '$';
        //const char SupplierServicesSeparator = '#';
        //const string SupplierServicesSeparatorAsString = "#";
        //const char ExactSupplierServicesSeparator = '#';
        //const string ExactSupplierServicesSeparatorAsString = "#";

        //const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        #region Public Methods

        public bool ShouldApplyCodeZoneMatch { get; set; }

        public object InitialiazeStreamForDBApply()
        {
            //var codeMatchBulkInsert = new CodeMatchBulkInsert()
            //{
            //    CodeMatchStream = base.InitializeStreamForBulkInsert()
            //};
            //if (ShouldApplyCodeZoneMatch)
            //{
            //    codeMatchBulkInsert.CodeSaleZoneMatchStream = base.InitializeStreamForBulkInsert();
            //    codeMatchBulkInsert.CodeSupplierZoneMatchStream = base.InitializeStreamForBulkInsert();
            //}

            var codeMatchBulkInsert = new CodeMatchBulkInsert()
            {
                CodeSaleZoneMatchStream = base.InitializeStreamForBulkInsert(),
                CodeSupplierZoneMatchStream = base.InitializeStreamForBulkInsert()
            };

            return codeMatchBulkInsert;
        }

        public void WriteRecordToStream(Entities.CodeMatches record, object dbApplyStream)
        {
            //string supplierCodeMatchesWithRate = this.SerializeSupplierCodeMatches(record.SupplierCodeMatches);

            var codeMatchBulkInsert = dbApplyStream as CodeMatchBulkInsert;
            //codeMatchBulkInsert.CodeMatchStream.WriteRecord("{0}^{1}^{2}", record.CodePrefix, record.Code, supplierCodeMatchesWithRate);

            //if (ShouldApplyCodeZoneMatch)
            //{
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
            //}
        }

        public void ApplyCodeMatchesForDB(object preparedData)
        {
            var codeMatchBulkInsertInfo = preparedData as CodeMatchBulkInsertInfo;
            int count = 2;

            Parallel.For(0, count, (i) =>
            {
                switch (i)
                {
                    //case 0: base.InsertBulkToTable(codeMatchBulkInsertInfo.CodeMatchesBulkInsertInfo); break;
                    //case 1: base.InsertBulkToTable(codeMatchBulkInsertInfo.CodeSaleZoneMatchBulkInsertInfo); break;
                    //case 2: base.InsertBulkToTable(codeMatchBulkInsertInfo.CodeSupplierZoneMatchBulkInsertInfo); break;

                    case 0: base.InsertBulkToTable(codeMatchBulkInsertInfo.CodeSaleZoneMatchBulkInsertInfo); break;
                    case 1: base.InsertBulkToTable(codeMatchBulkInsertInfo.CodeSupplierZoneMatchBulkInsertInfo); break;
                }
            });
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            var codeMatchBulkInsertInfo = new CodeMatchBulkInsertInfo();
            var codeMatchBulkInsert = dbApplyStream as CodeMatchBulkInsert;

            //codeMatchBulkInsert.CodeMatchStream.Close();
            //codeMatchBulkInsertInfo.CodeMatchesBulkInsertInfo = new StreamBulkInsertInfo()
            //{
            //    TableName = "[dbo].[CodeMatch]",
            //    Stream = codeMatchBulkInsert.CodeMatchStream,
            //    TabLock = true,
            //    KeepIdentity = false,
            //    FieldSeparator = '^',
            //    ColumnNames = codeMatchColumns
            //};

            //if (ShouldApplyCodeZoneMatch)
            //{
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
            //}

            return codeMatchBulkInsertInfo;
        }

        public IEnumerable<RPCodeMatches> GetRPCodeMatches(long fromZoneId, long toZoneId, Func<bool> shouldStop)
        {
            Dictionary<long, RPCodeMatches> result = new Dictionary<long, RPCodeMatches>();
            ExecuteReaderText(query_GetRPCodeMatchesByZone, (reader) =>
            {
                while (reader.Read())
                {
                    if (shouldStop != null && shouldStop())
                        break;

                    string code = reader["Code"] as string;
                    long saleZoneId = (long)reader["SaleZoneID"];
                    RPCodeMatches rpCodeMatches = result.GetOrCreateItem(saleZoneId, () =>
                    {
                        return new RPCodeMatches() { SaleZoneId = saleZoneId, Code = code, SupplierCodeMatches = new List<SupplierCodeMatchWithRate>() }; ;
                    });

                    SupplierCodeMatchWithRate supplierCodeMatchWithRate = SupplierCodeMatchWithRateMapper(reader);
                    rpCodeMatches.SupplierCodeMatches.Add(supplierCodeMatchWithRate);
                }
            }, (cmd) =>
            {
                var dtPrm = new SqlParameter("@FromZoneId", SqlDbType.BigInt);
                dtPrm.Value = fromZoneId;
                cmd.Parameters.Add(dtPrm);

                dtPrm = new SqlParameter("@ToZoneId", SqlDbType.BigInt);
                dtPrm.Value = toZoneId;
                cmd.Parameters.Add(dtPrm);
            });

            return result.Values;
        }

        public List<PartialCodeMatches> GetPartialCodeMatchesByRouteCodes(HashSet<string> routeCodes)
        {
            DataTable dtCodes = BuildCodesTable(routeCodes);
            Dictionary<string, PartialCodeMatches> result = new Dictionary<string, PartialCodeMatches>();
            ExecuteReaderText(query_GetCodeMatchesByCode.ToString(), (reader) =>
            {
                while (reader.Read())
                {
                    string code = reader["Code"] as string;
                    PartialCodeMatches partialCodeMatches = result.GetOrCreateItem(code, () =>
                    {
                        return new PartialCodeMatches() { Code = code, SupplierCodeMatches = new List<SupplierCodeMatchWithRate>(), SupplierCodeMatchesBySupplier = new SupplierCodeMatchWithRateBySupplier() }; ;
                    });

                    SupplierCodeMatchWithRate supplierCodeMatchWithRate = SupplierCodeMatchWithRateMapper(reader);
                    partialCodeMatches.SupplierCodeMatches.Add(supplierCodeMatchWithRate);
                    partialCodeMatches.SupplierCodeMatchesBySupplier.Add(supplierCodeMatchWithRate.CodeMatch.SupplierId, supplierCodeMatchWithRate);
                }
            }, (cmd) =>
            {
                var dtPrm = new SqlParameter("@Codes", SqlDbType.Structured);
                dtPrm.TypeName = "CodeType";
                dtPrm.Value = dtCodes;
                cmd.Parameters.Add(dtPrm);
            });
            return result.Values.ToList();
        }

        SupplierCodeMatchWithRate SupplierCodeMatchWithRateMapper(IDataReader reader)
        {
            string supplierServiceIds = reader["SupplierServiceIds"] as string;
            string exactSupplierServiceIds = reader["ExactSupplierServiceIds"] as string;
            return new SupplierCodeMatchWithRate()
             {
                 CodeMatch = new SupplierCodeMatch()
                 {
                     SupplierCode = reader["SupplierCode"] as string,
                     SupplierId = (int)reader["SupplierID"],
                     SupplierZoneId = (long)reader["SupplierZoneID"]
                 },
                 SupplierServiceIds = !string.IsNullOrEmpty(supplierServiceIds) ? supplierServiceIds.Split(',').Select(itm => int.Parse(itm)).ToHashSet() : null,
                 ExactSupplierServiceIds = !string.IsNullOrEmpty(exactSupplierServiceIds) ? exactSupplierServiceIds.Split(',').Select(itm => int.Parse(itm)).ToHashSet() : null,
                 RateValue = (decimal)reader["EffectiveRateValue"],
                 SupplierRateEED = GetReaderValue<DateTime?>(reader, "SupplierRateEED"),
                 SupplierRateId = (long)reader["SupplierRateId"],
                 SupplierServiceWeight = (int)reader["SupplierServiceWeight"]
             };
        }

        DataTable BuildCodesTable(HashSet<string> routeCodes)
        {
            DataTable dtCodes = new DataTable();
            dtCodes.Columns.Add("Code", typeof(string));
            dtCodes.BeginLoadData();
            foreach (var routeCode in routeCodes)
            {
                DataRow dr = dtCodes.NewRow();
                dr["Code"] = routeCode;
                dtCodes.Rows.Add(dr);
            }
            dtCodes.EndLoadData();
            return dtCodes;
        }

        #endregion

        #region Private Methods

        //private RPCodeMatches RPCodeMatchesMapper(IDataReader reader)
        //{
        //    string supplierCodeMatchesWithRate = reader["Content"] as string;

        //    return new RPCodeMatches()
        //    {
        //        Code = reader["Code"] as string,
        //        SupplierCodeMatches = this.DeserializeSupplierCodeMatches(supplierCodeMatchesWithRate),
        //        SaleZoneId = (long)reader["SaleZoneId"]
        //    };
        //}

        //private PartialCodeMatches PartialCodeMatchesMapper(IDataReader reader)
        //{
        //    PartialCodeMatches partialCodeMatches = new PartialCodeMatches()
        //    {
        //Code = reader["Code"] as string,
        //CodePrefix = reader["CodePrefix"] as string,
        //SupplierCodeMatches = this.DeserializeSupplierCodeMatches(reader["Content"] as string),
        //    };

        //    if (partialCodeMatches.SupplierCodeMatches != null)
        //    {
        //        partialCodeMatches.SupplierCodeMatchesBySupplier = new SupplierCodeMatchWithRateBySupplier();
        //        foreach (SupplierCodeMatchWithRate supplierCodeMatchWithRate in partialCodeMatches.SupplierCodeMatches)
        //        {
        //            partialCodeMatches.SupplierCodeMatchesBySupplier.Add(supplierCodeMatchWithRate.CodeMatch.SupplierId, supplierCodeMatchWithRate);
        //        }
        //    }

        //    return partialCodeMatches;
        //}


        /// <summary>
        /// CM~RateValue~ServiceId1#...#ServiceIdn~ExactServiceId1#...#ExactServiceIdn~...~SupplierRateEED|CM~RateValue~ServiceId1#...#ServiceIdn~ExactServiceId1#...#ExactServiceIdn~...~SupplierRateEED
        /// CM --> SupplierId$SupplierZoneId$SupplierCode$SupplierCodeSourceId
        /// </summary>
        /// <param name="supplierCodeMatchesWithRate"></param>
        /// <returns></returns>
        //private string SerializeSupplierCodeMatches(List<SupplierCodeMatchWithRate> supplierCodeMatchesWithRate)
        //{
        //    StringBuilder str = new StringBuilder();

        //    foreach (var item in supplierCodeMatchesWithRate)
        //    {
        //        if (str.Length > 0)
        //            str.Append(SupplierCodeMatchesWithRateSeparator);

        //        SupplierCodeMatch supplierCodeMatch = item.CodeMatch;
        //        string serializedSupplierCodeMatch = string.Format("{1}{0}{2}{0}{3}{0}{4}", SupplierCodeMatchPropertiesSeparator, supplierCodeMatch.SupplierId,
        //                                                supplierCodeMatch.SupplierZoneId, supplierCodeMatch.SupplierCode, supplierCodeMatch.SupplierCodeSourceId);

        //        string serializedSupplierServiceIds = string.Empty;
        //        if (item.SupplierServiceIds != null)
        //            serializedSupplierServiceIds = string.Join(SupplierServicesSeparatorAsString, item.SupplierServiceIds);

        //        string serializedExactSupplierServiceIds = string.Empty;
        //        if (item.ExactSupplierServiceIds != null)
        //            serializedExactSupplierServiceIds = string.Join(ExactSupplierServicesSeparatorAsString, item.ExactSupplierServiceIds);

        //        str.AppendFormat("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", SupplierCodeMatchWithRatePropertiesSeparator, serializedSupplierCodeMatch, item.RateValue,
        //            serializedSupplierServiceIds, serializedExactSupplierServiceIds, item.SupplierServiceWeight, item.SupplierRateId, item.SupplierRateEED.HasValue ? item.SupplierRateEED.Value.ToString(DateTimeFormat) : string.Empty);
        //    }
        //    return str.ToString();
        //}

        //public List<SupplierCodeMatchWithRate> DeserializeSupplierCodeMatches(string serializedSupplierCodeMatches)
        //{
        //    if (string.IsNullOrEmpty(serializedSupplierCodeMatches))
        //        return null;

        //    List<SupplierCodeMatchWithRate> supplierCodeMatches = new List<SupplierCodeMatchWithRate>();

        //    string[] lines = serializedSupplierCodeMatches.Split(SupplierCodeMatchesWithRateSeparator);

        //    foreach (var line in lines)
        //    {
        //        string[] parts = line.Split(SupplierCodeMatchWithRatePropertiesSeparator);

        //        var supplierCodeMatchWithRate = new SupplierCodeMatchWithRate();
        //        supplierCodeMatchWithRate.RateValue = decimal.Parse(parts[1]);
        //        supplierCodeMatchWithRate.SupplierServiceWeight = int.Parse(parts[4]);
        //        supplierCodeMatchWithRate.SupplierRateId = long.Parse(parts[5]);
        //        supplierCodeMatchWithRate.SupplierRateEED = !string.IsNullOrEmpty(parts[6]) ? DateTime.ParseExact(parts[6], DateTimeFormat, CultureInfo.InvariantCulture) : default(DateTime?);

        //        string supplierCodeMatchAsString = parts[0];
        //        if (!string.IsNullOrEmpty(supplierCodeMatchAsString))
        //        {
        //            string[] supplierCodeMatchZones = supplierCodeMatchAsString.Split(SupplierCodeMatchPropertiesSeparator);

        //            SupplierCodeMatch supplierCodeMatch = new SupplierCodeMatch();
        //            supplierCodeMatch.SupplierId = int.Parse(supplierCodeMatchZones[0]);
        //            supplierCodeMatch.SupplierZoneId = long.Parse(supplierCodeMatchZones[1]);
        //            supplierCodeMatch.SupplierCode = supplierCodeMatchZones[2] as string;
        //            supplierCodeMatch.SupplierCodeSourceId = supplierCodeMatchZones[3] as string;

        //            supplierCodeMatchWithRate.CodeMatch = supplierCodeMatch;
        //        }

        //        if (!string.IsNullOrEmpty(parts[2]))
        //            supplierCodeMatchWithRate.SupplierServiceIds = new HashSet<int>(parts[2].Split(SupplierServicesSeparator).Select(itm => int.Parse(itm)));

        //        if (!string.IsNullOrEmpty(parts[3]))
        //            supplierCodeMatchWithRate.ExactSupplierServiceIds = new HashSet<int>(parts[3].Split(ExactSupplierServicesSeparator).Select(itm => int.Parse(itm)));

        //        supplierCodeMatches.Add(supplierCodeMatchWithRate);
        //    }

        //    return supplierCodeMatches;
        //}

        #endregion

        #region Private Classes

        private class CodeMatchBulkInsert
        {
            //public StreamForBulkInsert CodeMatchStream { get; set; }
            public StreamForBulkInsert CodeSaleZoneMatchStream { get; set; }
            public StreamForBulkInsert CodeSupplierZoneMatchStream { get; set; }
        }

        private class CodeMatchBulkInsertInfo
        {
            //public StreamBulkInsertInfo CodeMatchesBulkInsertInfo { get; set; }
            public StreamBulkInsertInfo CodeSaleZoneMatchBulkInsertInfo { get; set; }
            public StreamBulkInsertInfo CodeSupplierZoneMatchBulkInsertInfo { get; set; }
        }

        #endregion

        #region Queries

        const string query_GetRPCodeMatchesByZone = @"SELECT DISTINCT cszm.[Code]
                                                            ,cszm.[SupplierID]
                                                            ,cszm.[SupplierZoneID]
                                                            ,sz.SaleZoneID
		                                                    ,null as SupplierCode
		                                                    ,szd.SupplierServiceIds
		                                                    ,szd.ExactSupplierServiceIds
		                                                    ,szd.EffectiveRateValue
		                                                    ,szd.SupplierServiceWeight
		                                                    ,szd.SupplierRateId
		                                                    ,szd.SupplierRateEED
                                                    FROM    [dbo].[CodeSupplierZoneMatch] cszm with(nolock)
                                                    JOIN    [dbo].[CodeSaleZone] sz on sz.code = cszm.code
                                                    JOIN    [dbo].[SupplierZoneDetail] szd on szd.SupplierZoneID = cszm.SupplierZoneID  
                                                    WHERE   sz.SaleZoneId between @FromZoneId and @ToZoneId";

        private StringBuilder query_GetCodeMatchesByCode = new StringBuilder(@"SELECT  cszm.[Code]
                                                                                       ,cszm.[SupplierID]
                                                                                       ,cszm.[SupplierZoneID]
		                                                                               ,cszm.CodeMatch as SupplierCode
		                                                                               ,szd.SupplierServiceIds
		                                                                               ,szd.ExactSupplierServiceIds
		                                                                               ,szd.EffectiveRateValue
		                                                                               ,szd.SupplierServiceWeight
		                                                                               ,szd.SupplierRateId
		                                                                               ,szd.SupplierRateEED
                                                                               FROM    [dbo].[CodeSupplierZoneMatch] cszm with(nolock)
                                                                               JOIN    [dbo].[SupplierZoneDetail] szd on szd.SupplierZoneID = cszm.SupplierZoneID  
                                                                               JOIN    @Codes c ON c.Code = cszm.Code");

        #endregion
    }
}
