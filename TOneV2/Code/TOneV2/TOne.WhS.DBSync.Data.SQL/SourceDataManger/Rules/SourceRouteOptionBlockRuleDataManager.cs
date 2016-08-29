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
        public SourceRouteOptionBlockRuleDataManager(string connectionString)
            : base(connectionString, false)
        {

        }

        IEnumerable<SourceRouteOptionBlockRule> GetRouteOptionBlockRules(bool getEffectiveOnly)
        {
            return GetItemsText(query_getRouteOptionBlockRules, SourceRouteOptionBlockRuleMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@GetEffectiveOnly", getEffectiveOnly));
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
                IncludeSubCode = (reader["IncludeSubCode"] as string).Equals("Y"),
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
	                                                        where (@GetEffectiveOnly = 0 or (@GetEffectiveOnly = 1 and rb.IsEffective = 'Y'))";
    }
}
