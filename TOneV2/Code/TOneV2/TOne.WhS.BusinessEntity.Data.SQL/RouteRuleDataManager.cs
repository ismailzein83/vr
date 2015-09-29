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
                Description = reader["Description"] as string
            };

            return routeRule;
        }
    }
}
