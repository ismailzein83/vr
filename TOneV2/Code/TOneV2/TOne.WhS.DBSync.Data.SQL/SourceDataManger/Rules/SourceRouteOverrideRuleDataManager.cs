using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL.SourceDataManger
{
    public class SourceRouteOverrideRuleDataManager : BaseSQLDataManager
    {
        readonly bool _getEffectiveOnly;
        DateTime? _effectiveAfter;
        public SourceRouteOverrideRuleDataManager(string connectionString, DateTime? effectiveAfter, bool getEffectiveOnly)
            : base(connectionString, false)
        {
            _effectiveAfter = effectiveAfter;
            _getEffectiveOnly = getEffectiveOnly;
        }

        public IEnumerable<SourceRouteOverrideRule> GetRouteOverrideRules()
        {
            return GetItemsText(string.Format(query_getRouteOverrideRules, MigrationUtils.GetEffectiveQuery("ro", _getEffectiveOnly, _effectiveAfter)), SourceRouteOverrideRuleMapper, null);
        }

        public IEnumerable<SourceRouteOverrideRule> GetRouteOverrideOptionBlockRules()
        {
            return GetItemsText(string.Format(query_getRouteOverrideRules_BlockedSuppliers, MigrationUtils.GetEffectiveQuery("ro", _getEffectiveOnly, _effectiveAfter)), SourceRouteOverrideRuleMapper, null);

        }

        SourceRouteOverrideRule SourceRouteOverrideRuleMapper(IDataReader reader)
        {
            return new SourceRouteOverrideRule
            {
                SourceId = reader["RouteOverrideID"].ToString(),
                CustomerId = reader["CustomerID"] as string,
                SupplierOptions = GetSupplierOptions(reader["RouteOptions"] as string),
                SupplierOptionsString = reader["RouteOptions"] as string,
                Code = reader["Code"] as string,
                SaleZoneId = GetReaderValue<int?>(reader, "ZoneID"),
                BED = (DateTime)reader["BeginEffectiveDate"],
                EED = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                ExcludedCodes = reader["ExcludedCodes"] as string,
                IncludeSubCode = string.IsNullOrEmpty((reader["IncludeSubCodes"] as string)) ? false : (reader["IncludeSubCodes"] as string).Equals("Y"),
                Reason = reader["Reason"] as string,
                ExcludedCodesList = GetExcludedCodes(reader["ExcludedCodes"] as string),
                BlockedOptions = GetBlockedOptions(reader["BlockedSuppliers"] as string),
                BlockedOptionsString = reader["BlockedSuppliers"] as string
            };
        }

        private IEnumerable<BlockedOption> GetBlockedOptions(string option)
        {
            if (string.IsNullOrEmpty(option))
                return null;
            List<BlockedOption> blockedOptions = new List<BlockedOption>();
            string[] options = option.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var optionString in options)
            {
                BlockedOption bOption = new BlockedOption { SupplierId = optionString };
                blockedOptions.Add(bOption);
            }
            return blockedOptions;
        }
        private HashSet<string> GetExcludedCodes(string codes)
        {
            if (string.IsNullOrEmpty(codes))
                return null;
            return new HashSet<string>(codes.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).OrderBy(s => s));

        }

        private IEnumerable<SupplierOption> GetSupplierOptions(string option)
        {
            if (string.IsNullOrEmpty(option))
                return null;
            List<SupplierOption> supplierOptions = new List<SupplierOption>();
            string[] options = option.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            short index = 0;
            foreach (var optionString in options)
            {

                string[] supplierOptionEntries = optionString.Split(',');
                string percentage = supplierOptionEntries.Length == 3 ? supplierOptionEntries[2] : null;
                int? calculatedPercentage = null;
                if (!string.IsNullOrEmpty(percentage))
                {
                    int tempPercentage = Convert.ToInt32(percentage);
                    if (tempPercentage > 0)
                        calculatedPercentage = tempPercentage;
                }

                SupplierOption supplierOption = new SupplierOption
                {
                    SupplierId = supplierOptionEntries[0],
                    IsLoss = supplierOptionEntries.Length > 1 && supplierOptionEntries[1].Equals("1"),
                    Percentage = calculatedPercentage,
                    Priority = index++
                };

                supplierOptions.Add(supplierOption);
            }
            return supplierOptions;
        }

        const string query_getRouteOverrideRules = @"	SELECT  ro.RouteOverrideID,
		                                                        ro.CustomerID,
		                                                        CASE WHEN ro.Code = '*ALL*' THEN NULL ELSE ro.Code END Code,
		                                                        ro.IncludeSubCodes,
		                                                        CASE WHEN ro.Code != '*ALL*' THEN NULL ELSE ro.OurZoneID END ZoneID,
		                                                        ro.RouteOptions,
                                                                ro.BlockedSuppliers,
		                                                        ro.BeginEffectiveDate,
		                                                        ro.EndEffectiveDate,
		                                                        ro.ExcludedCodes,
		                                                        ro.Reason
	                                                    FROM    RouteOverride ro 
	                                                    WHERE   ro.RouteOptions != 'BLK' AND ro.RouteOptions IS NOT NULL AND ro.RouteOptions <> '' {0}";

        const string query_getRouteOverrideRules_BlockedSuppliers = @"	SELECT  ro.RouteOverrideID,
		                                                        ro.CustomerID,
		                                                        CASE WHEN ro.Code = '*ALL*' THEN NULL ELSE ro.Code END Code,
		                                                        ro.IncludeSubCodes,
		                                                        CASE WHEN ro.Code != '*ALL*' THEN NULL ELSE ro.OurZoneID END ZoneID,
		                                                        ro.RouteOptions,
                                                                ro.BlockedSuppliers,
		                                                        ro.BeginEffectiveDate,
		                                                        ro.EndEffectiveDate,
		                                                        ro.ExcludedCodes,
		                                                        ro.Reason
	                                                    FROM    RouteOverride ro 
	                                                    WHERE   ro.BlockedSuppliers IS NOT NULL and  ro.BlockedSuppliers<> '' {0}";
    }
}
