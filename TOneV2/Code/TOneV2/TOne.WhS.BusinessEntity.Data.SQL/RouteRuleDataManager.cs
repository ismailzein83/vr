using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class RouteRuleDataManager : BaseSQLDataManager, IRouteRuleDataManager
    {
         private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

         static RouteRuleDataManager()
        {
            _columnMapper.Add("RouteRuleId", "ID");
        }


        public RouteRuleDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public Vanrise.Entities.BigResult<Entities.RouteRule> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<Entities.RouteRuleQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("TOneWhS_BE.sp_RouteRule_CreateTempByFiltered", tempTableName);
            };

            return RetrieveData(input, createTempTableAction, RouteRuleMapper, _columnMapper);
        }

        public Entities.RouteRule GetRouteRule(int routeRuleId)
        {
            return GetItemSP("TOneWhS_BE.sp_RouteRule_Get", RouteRuleMapper, routeRuleId);
        }

        public bool Insert(Entities.RouteRule routeRule, out int insertedId)
        {
            object routeRuleId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_RouteRule_Insert", out routeRuleId, Vanrise.Common.Serializer.Serialize(routeRule.RouteCriteria),
                0, null, null, DateTime.Now, null, null);

            insertedId = (int)routeRuleId;
            return (recordsEffected > 0);
        }

        public bool Update(Entities.RouteRule routeRule)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_RouteRule_Update", routeRule.RouteRuleId, Vanrise.Common.Serializer.Serialize(routeRule.RouteCriteria),
                0, null, null, DateTime.Now, null, null);
            return (recordsEffected > 0);
        }

        public bool Delete(int routeRuleId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_RouteRule_Delete", routeRuleId);
            return (recordesEffected > 0);
        }

        Entities.RouteRule RouteRuleMapper(IDataReader reader)
        {
            Entities.RouteRule routeRule = new Entities.RouteRule
            {
                RouteRuleId = (int)reader["ID"],
                RouteCriteria = Vanrise.Common.Serializer.Deserialize<Entities.RouteCriteria>(reader["Criteria"] as string),
                //TypeConfigId = (int)reader["TypeConfigID"],
                Settings = (reader["RuleSettings"] as string) != null ? Vanrise.Common.Serializer.Deserialize<Entities.RouteRuleSettings>(reader["RuleSettings"] as string): null,
                Description = reader["Description"] as string,
                BeginEffectiveTime = (DateTime)reader["BED"],
                EndEffectiveTime = GetReaderValue<DateTime>(reader, "EED")
            };

            return routeRule;
        }
    }
}
