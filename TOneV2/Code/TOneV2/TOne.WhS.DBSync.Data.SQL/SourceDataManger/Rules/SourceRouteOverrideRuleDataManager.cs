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
    public class SourceRouteOverrideRuleDataManager : BaseSQLDataManager
    {
        bool _getEffectiveOnly;
        public SourceRouteOverrideRuleDataManager(string connectionString, bool getEffectiveOnly)
            : base(connectionString, false)
        {
            _getEffectiveOnly = getEffectiveOnly;
        }

        public IEnumerable<SourceRouteOverrideRule> GetRouteOverrideRules()
        {
            return GetItemsText(query_getRouteOverrideRules, SourceRouteOverrideRuleMapper, (cmd) =>
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
                SupplierOptions = GetSupplierOptions(reader["SupplierID"] as string),
                SupplierOptionsString = reader["SupplierID"] as string,
                Code = reader["Code"] as string,
                SaleZoneId = GetReaderValue<int>(reader, "ZoneID"),
                BED = (DateTime)reader["BeginEffectiveDate"],
                EED = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                ExcludedCodes = reader["ExcludedCodes"] as string,
                IncludeSubCode = (reader["IncludeSubCodes"] as string).Equals("Y"),
                Reason = reader["Reason"] as string
            };
        }

        private IEnumerable<SupplierOption> GetSupplierOptions(string option)
        {
            if (string.IsNullOrEmpty(option))
                return null;
            List<SupplierOption> supplierOptions = new List<SupplierOption>();
            string[] options = option.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            short index = 0;
            foreach (var optionString in options)
            {
                string[] supplierOptionEntries = optionString.Split(',');
                SupplierOption supplierOption = new SupplierOption
                {
                    SupplierId = supplierOptionEntries[0],
                    IsLoss = bool.Parse(supplierOptionEntries[1]),
                    Percentage = short.Parse(supplierOptionEntries[2]),
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
		                                                        ro.BeginEffectiveDate,
		                                                        ro.EndEffectiveDate,
		                                                        ro.ExcludedCodes,
		                                                        ro.Reason
	                                                    FROM    RouteOverride ro 
	                                                    WHERE   ro.RouteOptions != 'BLK' AND ro.RouteOptions IS NOT NULL
		                                                        and (@GetEffectiveOnly is null or (@GetEffectiveOnly = 1 and ro.IsEffective = 'Y'))";
    }
}
