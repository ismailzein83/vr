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
    public class SourceRouteOptionBlockRuleDataManager : BaseSQLDataManager
    {
        readonly bool _getEffectiveOnly;
        DateTime? _effectiveAfter;
        public SourceRouteOptionBlockRuleDataManager(string connectionString, DateTime? effectiveAfter, bool getEffectiveOnly)
            : base(connectionString, false)
        {
            _effectiveAfter = effectiveAfter;
            _getEffectiveOnly = getEffectiveOnly;
        }

        public IEnumerable<SourceRouteOptionBlockRule> GetRouteOptionBlockRules()
        {
            return GetItemsText(string.Format(query_getRouteOptionBlockRules, MigrationUtils.GetEffectiveQuery("rb", _getEffectiveOnly, _effectiveAfter)), SourceRouteOptionBlockRuleMapper, null);
        }

        SourceRouteOptionBlockRule SourceRouteOptionBlockRuleMapper(IDataReader reader)
        {
            return new SourceRouteOptionBlockRule
            {
                SourceId = reader["RouteBlockID"].ToString(),
                CustomerId = reader["CustomerID"] as string,
                SupplierId = reader["SupplierID"] as string,
                Code = reader["Code"] as string,
                SupplierZoneId = GetReaderValue<int?>(reader, "ZoneID"),
                BED = (DateTime)reader["BeginEffectiveDate"],
                EED = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                ExcludedCodes = reader["ExcludedCodes"] as string,
                ExcludedCodesList = GetExcludedCodes(reader["ExcludedCodes"] as string),
                IncludeSubCode = string.IsNullOrEmpty((reader["IncludeSubCodes"] as string)) ? false : (reader["IncludeSubCodes"] as string).Equals("Y"),
                Reason = reader["Reason"] as string
            };
        }
        HashSet<string> GetExcludedCodes(string codes)
        {
            if (string.IsNullOrEmpty(codes))
                return null;
            return new HashSet<string>(codes.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).OrderBy(s => s));

        }

        const string query_getRouteOptionBlockRules = @"SELECT
		                                                    rb.RouteBlockID,
		                                                    rb.CustomerID,
		                                                    rb.Code,
		                                                    rb.IncludeSubCodes,
		                                                    rb.ZoneID,
		                                                    rb.SupplierID,
		                                                    rb.BeginEffectiveDate,
		                                                    rb.EndEffectiveDate,
		                                                    rb.IsEffective,
		                                                    rb.ExcludedCodes,
		                                                    rb.Reason
	                                                    FROM
		                                                    RouteBlock rb 
	                                                        where 1=1 {0}";
    }
}
