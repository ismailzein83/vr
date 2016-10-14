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
        public SourceRouteOptionBlockRuleDataManager(string connectionString, bool getEffectiveOnly)
            : base(connectionString, false)
        {
            _getEffectiveOnly = getEffectiveOnly;
        }

        public IEnumerable<SourceRouteOptionBlockRule> GetRouteOptionBlockRules()
        {
            return GetItemsText(query_getRouteOptionBlockRules, SourceRouteOptionBlockRuleMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@GetEffectiveOnly", _getEffectiveOnly));
            });
        }

        SourceRouteOptionBlockRule SourceRouteOptionBlockRuleMapper(IDataReader reader)
        {
            return new SourceRouteOptionBlockRule
            {
                SourceId = reader["RouteBlockID"].ToString(),
                CustomerId = reader["CustomerID"] as string,
                SupplierId = reader["SupplierID"] as string,
                Code = reader["Code"] as string,
                SupplierZoneId = GetReaderValue<int>(reader, "ZoneID"),
                BED = (DateTime)reader["BeginEffectiveDate"],
                EED = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                ExcludedCodes = reader["ExcludedCodes"] as string,
                IncludeSubCode = string.IsNullOrEmpty((reader["IncludeSubCodes"] as string)) ? false : (reader["IncludeSubCodes"] as string).Equals("Y"),
                Reason = reader["Reason"] as string
            };
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
	                                                        where ((@GetEffectiveOnly = 0 and rb.BeginEffectiveDate <= getdate())or (@GetEffectiveOnly = 1 and rb.IsEffective = 'Y'))";
    }
}
