using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL.SourceDataManger
{
    public class SourceRouteBlockRuleDataManager : BaseSQLDataManager
    {
        readonly bool _getEffectiveOnly;
        public SourceRouteBlockRuleDataManager(string connectionString, bool getEffectiveOnly)
            : base(connectionString, false)
        {
            _getEffectiveOnly = getEffectiveOnly;
        }
        public IEnumerable<SourceRouteOverrideRule> GetRouteblockRules()
        {
            return GetItemsText(query_getRouteBlockRules, SourceRouteOverrideRuleMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@GetEffectiveOnly", _getEffectiveOnly));
            });
        }

        SourceRouteOverrideRule SourceRouteOverrideRuleMapper(IDataReader reader)
        {
            return new SourceRouteOverrideRule
            {
                SourceId = reader["RouteOverrideID"].ToString(),
                CustomerId = reader["CustomerID"] as string,
                SupplierOptions = new List<SupplierOption> { new SupplierOption { SupplierId = "BLK" } },
                SupplierOptionsString = reader["RouteOptions"] as string,
                Code = reader["Code"] as string,
                SaleZoneId = GetReaderValue<int?>(reader, "ZoneID"),
                BED = (DateTime)reader["BeginEffectiveDate"],
                EED = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                ExcludedCodes = reader["ExcludedCodes"] as string,
                IncludeSubCode = string.IsNullOrEmpty((reader["IncludeSubCodes"] as string)) ? false : (reader["IncludeSubCodes"] as string).Equals("Y"),
                Reason = reader["Reason"] as string,
                ExcludedCodesList = GetExcludedCodes(reader["ExcludedCodes"] as string)
            };
        }
        HashSet<string> GetExcludedCodes(string codes)
        {
            if (string.IsNullOrEmpty(codes))
                return null;
            return new HashSet<string>(codes.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).OrderBy(s => s));

        }
        const string query_getRouteBlockRules = @"SELECT  ro.RouteOverrideID,
		                                                        ro.CustomerID,
		                                                        CASE WHEN ro.Code = '*ALL*' THEN NULL ELSE ro.Code END Code,
		                                                        ro.IncludeSubCodes,
		                                                        CASE WHEN ro.Code != '*ALL*' THEN NULL ELSE ro.OurZoneID END ZoneID,
		                                                        ro.RouteOptions,
		                                                        ro.BeginEffectiveDate,
		                                                        ro.EndEffectiveDate,
		                                                        ro.ExcludedCodes,
		                                                        ro.Reason
	                                                    FROM    RouteOverride ro 
	                                                    WHERE   ro.RouteOptions = 'BLK' 
		                                                        and ((@GetEffectiveOnly = 0 and ro.BeginEffectiveDate <= getdate()) or (@GetEffectiveOnly = 1 and ro.IsEffective = 'Y'))";
    }
}
