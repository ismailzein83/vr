using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.Entities;
using TOne.LCR.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;

namespace TOne.LCR.Data.SQL
{
    public class RouteRulesDataManager : BaseTOneDataManager, IRouteRulesDataManager
    {
        public List<RouteRule> GetRouteRules(DateTime effectiveOn, bool isFuture, string codePrefix, IEnumerable<int> lstZoneIds)
        {
            List<RouteRule> routeRules = new List<RouteRule>();
            routeRules.AddRange(GetBlockRules(effectiveOn, isFuture, codePrefix, lstZoneIds));
            routeRules.AddRange(GetOverrideRules(effectiveOn, isFuture, codePrefix, lstZoneIds));
            routeRules.AddRange(GetPriorityRules(effectiveOn, isFuture, codePrefix, lstZoneIds));

            return routeRules;
        }

        private List<RouteRule> GetBlockRules(DateTime effectiveOn, bool isFuture, string codePrefix, IEnumerable<int> lstZoneIds)
        {
            List<RouteRule> routeRules = new List<RouteRule>();
            DataTable dtZoneIds = BuildZoneIdsTable(lstZoneIds);
            ExecuteReaderSPCmd("[LCR].[sp_RoutingRules_GetBlockRules]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        RouteRule rule = new RouteRule();
                        string customerID = reader["CustomerID"] as string;
                        string code = reader["Code"] as string;
                        int zoneID = reader["ZoneID"] != DBNull.Value ? Convert.ToInt32(reader["ZoneID"]) : 0;
                        bool includeSubCodes = reader["IncludeSubCodes"] as string == "Y" ? true : false;
                        string excludedCodes = reader["ExcludedCodes"] as string;
                        MultipleSelection<string> customers = new MultipleSelection<string>();
                        customers.SelectionOption = MultipleSelectionOption.OnlyItems;
                        customers.SelectedValues = new List<string>();
                        customers.SelectedValues.Add(customerID);
                        rule.CarrierAccountSet = new CustomerSelectionSet() { Customers = customers };
                        rule.BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate");
                        rule.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate");
                        rule.Reason = reader["Reason"] as string;
                        rule.Type = RouteRuleType.RouteRule;
                        rule.ActionData = new BlockRouteActionData();
                        rule.CodeSet = GetBaseCodeSet(code, zoneID, includeSubCodes, excludedCodes);
                        routeRules.Add(rule);
                    }
                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ZoneIds", SqlDbType.Structured);
                    dtPrm.TypeName = "LCR.IntIDType";
                    dtPrm.Value = dtZoneIds;
                    cmd.Parameters.Add(dtPrm);
                    cmd.Parameters.Add(new SqlParameter("@CodePrefix", codePrefix));
                    cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                    cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                }
                );
            return routeRules;
        }
        public void SaveRouteRule(RouteRule rule)
        {
            object RouteRuleId;
            ExecuteNonQuerySP("[LCR].[sp_RouteRuleDefinition_Insert]", out RouteRuleId,
                rule.CarrierAccountSet != null ? Serializer.Serialize(rule.CarrierAccountSet) : null,
                rule.CodeSet != null ? Serializer.Serialize(rule.CodeSet) : null,
                (int)rule.Type,
                rule.BeginEffectiveDate,
                rule.EndEffectiveDate,
                rule.Reason,
                rule.ActionData != null ? Serializer.Serialize(rule.ActionData) : null
                );
        }

        public List<RouteRule> GetAllRouteRule()
        {
            return GetItemsSP("[LCR].[sp_RouteRuleDefinition_GetAll]", RouteRuleMapper);
        }
        public RouteRule GetRouteRuleDetails(int RouteRuleId)
        {
            return GetItemSP("[LCR].[sp_RouteRuleDefinition_GetByRouteRuleId]", RouteRuleMapper, RouteRuleId);
        }
        private List<RouteRule> GetPriorityRules(DateTime effectiveOn, bool isFuture, string codePrefix, IEnumerable<int> lstZoneIds)
        {
            List<RouteRule> routeRules = new List<RouteRule>();
            DataTable dtZoneIds = BuildZoneIdsTable(lstZoneIds);
            ExecuteReaderSPCmd("[LCR].[sp_RoutingRules_GetPriorityRules]",
                (reader) =>
                {
                    string currentCustomer = null;
                    string currentCode = null;
                    List<PriorityOption> priorityOptions = null;
                    while (reader.Read())
                    {
                        PriorityOption priorityOption = new PriorityOption()
                        {
                            SupplierId = reader["SupplierID"] as string,
                            Percentage = GetReaderValue<byte>(reader, "Percentage"),
                            Priority = GetReaderValue<byte>(reader, "Priority"),
                            Force = GetReaderValue<byte>(reader, "Type") == 1 ? true : false
                        };

                        string customerID = reader["CustomerID"] as string;
                        string code = reader["Code"] as string;

                        if (currentCode != code || currentCustomer != customerID)
                        {

                            //TODO Create RouteRule

                            currentCode = code;
                            currentCustomer = customerID;
                            RouteRule rule = new RouteRule();
                            MultipleSelection<string> customers = new MultipleSelection<string>();
                            customers.SelectionOption = MultipleSelectionOption.OnlyItems;
                            customers.SelectedValues = new List<string>();
                            customers.SelectedValues.Add(currentCustomer);
                            rule.CarrierAccountSet = new CustomerSelectionSet() { Customers = customers };
                            rule.BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate");
                            rule.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate");
                            priorityOptions = new List<PriorityOption>();
                            rule.ActionData = new PriorityRouteActionData() { Options = priorityOptions };
                            rule.CodeSet = GetBaseCodeSet(currentCode, 0, (reader["IncludeSubCodes"] as string) == "Y", reader["ExcludedCodes"] as string);
                            rule.Type = RouteRuleType.RouteRule;
                            rule.Reason = reader["Reason"] as string;
                            routeRules.Add(rule);

                        }

                        priorityOptions.Add(priorityOption);

                    }
                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ZoneIds", SqlDbType.Structured);
                    dtPrm.TypeName = "LCR.IntIDType";
                    dtPrm.Value = dtZoneIds;
                    cmd.Parameters.Add(dtPrm);
                    cmd.Parameters.Add(new SqlParameter("@CodePrefix", codePrefix));
                    cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                    cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                }
                );
            return routeRules;
        }

        private List<RouteRule> GetOverrideRules(DateTime effectiveOn, bool isFuture, string codePrefix, IEnumerable<int> lstZoneIds)
        {
            List<RouteRule> routeRules = new List<RouteRule>();
            DataTable dtZoneIds = BuildZoneIdsTable(lstZoneIds);
            ExecuteReaderSPCmd("[LCR].[sp_RoutingRules_GetOverrideRules]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        RouteRule rule = new RouteRule();
                        string customerID = reader["CustomerID"] as string;
                        string code = reader["Code"] as string;
                        int zoneID = GetReaderValue<int>(reader, "ZoneID");
                        bool includeSubCodes = reader["IncludeSubCodes"] as string == "Y" ? true : false;
                        string options = reader["RouteOptions"] as string;
                        string excludedCodes = reader["ExcludedCodes"] as string;
                        MultipleSelection<string> customers = new MultipleSelection<string>();
                        customers.SelectionOption = MultipleSelectionOption.OnlyItems;
                        customers.SelectedValues = new List<string>();
                        customers.SelectedValues.Add(customerID);
                        rule.CarrierAccountSet = new CustomerSelectionSet() { Customers = customers };
                        rule.BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate");
                        rule.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate");
                        rule.Reason = reader["Reason"] as string;
                        rule.Type = RouteRuleType.RouteRule;
                        rule.ActionData = GetOverrideActionData(options);

                        rule.CodeSet = GetBaseCodeSet(code, zoneID, includeSubCodes, excludedCodes);
                        routeRules.Add(rule);
                    }
                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ZoneIds", SqlDbType.Structured);
                    dtPrm.TypeName = "LCR.IntIDType";
                    dtPrm.Value = dtZoneIds;
                    cmd.Parameters.Add(dtPrm);
                    cmd.Parameters.Add(new SqlParameter("@CodePrefix", codePrefix));
                    cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                    cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                }
                );
            return routeRules;
        }

        private OverrideRouteActionData GetOverrideActionData(string options)
        {
            OverrideRouteActionData optionData = new OverrideRouteActionData();
            optionData.Options = new List<OverrideOption>();
            string[] supplierOptions = options.Split('|');
            foreach (string option in supplierOptions)
            {
                string[] supplierDetail = option.Split(',');
                string supplierid = supplierDetail[0];
                short percentage = 0;
                if (supplierDetail.Length > 1)
                    percentage = string.IsNullOrEmpty(supplierDetail[1]) ? (short)0 : Convert.ToInt16(supplierDetail[1]);
                OverrideOption overrideOption = new OverrideOption() { SupplierId = supplierid, Percentage = percentage };
                optionData.Options.Add(overrideOption);
            }

            return optionData;
        }

        private BaseCodeSet GetBaseCodeSet(string code, int zoneID, bool includeSubCodes, string excludedCodes)
        {

            if (!string.IsNullOrEmpty(code))
            {
                CodeSelectionSet codeSet = new CodeSelectionSet();
                codeSet.Code = code;
                codeSet.ExcludedCodes = !string.IsNullOrEmpty(excludedCodes) ? excludedCodes.Split(',').ToList() : new List<string>();
                codeSet.WithSubCodes = includeSubCodes;
                return codeSet;
            }
            else
            {
                ZoneSelectionSet zoneSet = new ZoneSelectionSet();
                zoneSet.ZoneIds = new MultipleSelection<int>();
                zoneSet.ZoneIds.SelectionOption = MultipleSelectionOption.OnlyItems;
                zoneSet.ZoneIds.SelectedValues = new List<int>() { zoneID };
                return zoneSet;
            }
        }

        private DataTable BuildZoneIdsTable(IEnumerable<int> zoneIds)
        {
            DataTable dtZoneInfo = new DataTable();
            dtZoneInfo.Columns.Add("ZoneID", typeof(Int32));
            dtZoneInfo.BeginLoadData();
            foreach (var z in zoneIds)
            {
                DataRow dr = dtZoneInfo.NewRow();
                dr["ZoneID"] = z;
                dtZoneInfo.Rows.Add(dr);
            }
            dtZoneInfo.EndLoadData();
            return dtZoneInfo;
        }

        RouteRule RouteRuleMapper(IDataReader reader)
        {
            var routeRule = new RouteRule
            {
                RouteRuleId = (int)reader["RouteRuleId"],
                CarrierAccountSet = reader["CarrierAccountSet"] != null ? (BaseCarrierAccountSet)Serializer.Deserialize(reader["CarrierAccountSet"] as string) : null,
                CodeSet = reader["CodeSet"] != null ? (BaseCodeSet)Serializer.Deserialize(reader["CodeSet"] as string) : null,
                ActionData = (reader["ActionData"] == System.DBNull.Value) ? null : (Object)Serializer.Deserialize(reader["ActionData"] as string),
                Type = (RouteRuleType)((int)reader["Type"]),
                BeginEffectiveDate = (DateTime)reader["BeginEffectiveDate"],
                EndEffectiveDate = (reader["EndEffectiveDate"] == System.DBNull.Value) ? (DateTime?)null : (DateTime)reader["EndEffectiveDate"],
                Reason = reader["Reason"] as string

            };

            return routeRule;
        }
    }
}
