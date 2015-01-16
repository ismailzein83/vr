using System.Data;

namespace TABS.DAL
{
    public partial class ObjectAssembler
    {
        public static IDataReader Ex_Sp_GetAlerts(int topN, string showHiddenAlerts, object alertLevel, string filter, string source, int? user)
        {
            string sql = QueryBuilder.Ex_Sp_GetAlertsQuery();
            return DataHelper.ExecuteReader(sql, topN, showHiddenAlerts, alertLevel, filter, source, user);
        }
    }
}
