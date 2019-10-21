using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.Routing.Data.RDB
{
    public class CodeMatchesDataManager : RoutingDataManager, ICodeMatchesDataManager
    {
        public bool ShouldApplyCodeZoneMatch { get; set; }

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            var codeMatchBulkInsert = new CodeMatchBulkInsert()
            {
                CodeSaleZoneMatchStream = new CodeSaleZoneMatchDataManager().InitialiazeStreamForDBApply(),
                CodeSupplierZoneMatchStream = new CodeSupplierZoneMatchDataManager().InitialiazeStreamForDBApply()
            };

            return codeMatchBulkInsert;
        }

        public void WriteRecordToStream(Entities.CodeMatches record, object dbApplyStream)
        {
            CodeSaleZoneMatchDataManager codeSaleZoneMatchDataManager = new CodeSaleZoneMatchDataManager();
            CodeSupplierZoneMatchDataManager codeSupplierZoneMatchDataManager = new CodeSupplierZoneMatchDataManager();

            var codeMatchBulkInsert = dbApplyStream as CodeMatchBulkInsert;

            if (record.SaleCodeMatches != null)
            {
                foreach (SaleCodeMatch saleCodeMatch in record.SaleCodeMatches)
                {
                    CodeSaleZoneMatch codeSaleZoneMatch = new CodeSaleZoneMatch()
                    {
                        Code = record.Code,
                        SellingNumberPlanId = saleCodeMatch.SellingNumberPlanId,
                        SaleZoneId = saleCodeMatch.SaleZoneId,
                        CodeMatch = saleCodeMatch.SaleCode
                    };
                    codeSaleZoneMatchDataManager.WriteRecordToStream(codeSaleZoneMatch, codeMatchBulkInsert.CodeSaleZoneMatchStream);
                }
            }

            if (record.SupplierCodeMatches != null)
            {
                foreach (SupplierCodeMatchWithRate supplierCodeMatch in record.SupplierCodeMatches)
                {
                    CodeSupplierZoneMatch codeSupplierZoneMatch = new CodeSupplierZoneMatch()
                    {
                        Code = record.Code,
                        SupplierId = supplierCodeMatch.CodeMatch.SupplierId,
                        SupplierZoneId = supplierCodeMatch.CodeMatch.SupplierZoneId,
                        CodeMatch = supplierCodeMatch.CodeMatch.SupplierCode
                    };
                    codeSupplierZoneMatchDataManager.WriteRecordToStream(codeSupplierZoneMatch, codeMatchBulkInsert.CodeSaleZoneMatchStream);
                }
            }
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            var codeMatchBulkInsert = dbApplyStream as CodeMatchBulkInsert;
            new CodeSaleZoneMatchDataManager().FinishDBApplyStream(codeMatchBulkInsert.CodeSaleZoneMatchStream);
            new CodeSupplierZoneMatchDataManager().FinishDBApplyStream(codeMatchBulkInsert.CodeSupplierZoneMatchStream);
            return codeMatchBulkInsert;
        }

        public void ApplyCodeMatchesForDB(object preparedData)
        {
            var codeMatchBulkInsert = preparedData as CodeMatchBulkInsert;
            int count = 2;

            Parallel.For(0, count, (i) =>
            {
                switch (i)
                {
                    case 0: new CodeSaleZoneMatchDataManager().ApplySaleZoneMatchesToTable(codeMatchBulkInsert.CodeSaleZoneMatchStream); break;
                    case 1: new CodeSupplierZoneMatchDataManager().ApplySupplierZoneMatchesToTable(codeMatchBulkInsert.CodeSupplierZoneMatchStream); break;
                }
            });
        }

        public Dictionary<long, RPCodeMatches> GetRPCodeMatchesBySaleZone(long fromZoneId, long toZoneId, Func<bool> shouldStop)
        {
            return new CodeSaleZoneDataManager().GetRPCodeMatchesBySaleZone(fromZoneId, toZoneId, SupplierCodeMatchWithRateMapper, shouldStop);
        }

        public List<PartialCodeMatches> GetPartialCodeMatchesByRouteCodes(HashSet<string> routeCodes)
        {
            return new CodeSupplierZoneMatchDataManager().GetPartialCodeMatchesByRouteCodes(routeCodes, SupplierCodeMatchWithRateMapper);
        }

        #endregion

        #region Private Methods

        private SupplierCodeMatchWithRate SupplierCodeMatchWithRateMapper(IRDBDataReader reader)
        {
            long? supplierZoneId = reader.GetNullableLong("SupplierZoneID");
            if (!supplierZoneId.HasValue)
                return null;

            string supplierServiceIds = reader.GetString("SupplierServiceIds");
            string exactSupplierServiceIds = reader.GetString("ExactSupplierServiceIds");

            return new SupplierCodeMatchWithRate()
            {
                CodeMatch = new SupplierCodeMatch()
                {
                    SupplierCode = reader.GetString("SupplierCode"),
                    SupplierId = reader.GetInt("SupplierID"),
                    SupplierZoneId = supplierZoneId.Value
                },
                DealId = reader.GetNullableInt("DealId"),
                SupplierServiceIds = !string.IsNullOrEmpty(supplierServiceIds) ? Vanrise.Common.ExtensionMethods.ToHashSet(supplierServiceIds.Split(',').Select(itm => int.Parse(itm))) : null,
                ExactSupplierServiceIds = !string.IsNullOrEmpty(exactSupplierServiceIds) ? Vanrise.Common.ExtensionMethods.ToHashSet(exactSupplierServiceIds.Split(',').Select(itm => int.Parse(itm))) : null,
                RateValue = reader.GetDecimal("EffectiveRateValue"),
                SupplierRateEED = reader.GetNullableDateTime("SupplierRateEED"),
                SupplierRateId = reader.GetNullableLong("SupplierRateId"),
                SupplierServiceWeight = reader.GetInt("SupplierServiceWeight")
            };
        }

        #endregion

        #region Private Classes

        private class CodeMatchBulkInsert
        {
            public object CodeSaleZoneMatchStream { get; set; }
            public object CodeSupplierZoneMatchStream { get; set; }
        }

        #endregion
    }
}