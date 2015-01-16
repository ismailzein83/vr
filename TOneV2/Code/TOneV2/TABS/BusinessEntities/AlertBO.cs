

namespace TABS.BusinessEntities
{
    public class AlertBO
    {

        public static object GetAlerts(bool showHiddenAlerts,
                                bool currentUserOnly,
                                TABS.AlertLevel? alertLevel,
                                string tag,
                                string source,
                                int? topFilers)
        {
            string queryString = string.Format("select A from Alert A, PersistedAlertCriteria C where Source like 'PersistedAlertCriteria%' and A.Source = 'PersistedAlertCriteria:' + cast(C.ID as char) {0} {1} {2} {3} {4} order by A.Created desc",
                showHiddenAlerts ? "" : " AND IsVisible='Y' ",
                currentUserOnly ? "AND C.User = :User" : "",
                alertLevel != null ? string.Format(" AND Level={0} ", (byte)alertLevel) : "",
                string.IsNullOrEmpty(tag) ? "" : string.Format(" AND A.Tag LIKE '%{0}%' ", tag.Trim()),
                string.IsNullOrEmpty(source) ? "" : string.Format(" AND Lower(LTrim(RTrim(Source))) = '{0}' ", source.ToString().Trim().ToLower()));
            var query = TABS.DataConfiguration.CurrentSession.CreateQuery(queryString);
            if (currentUserOnly)
                query.SetParameter("User", SecurityEssentials.Web.Helper.CurrentWebUser);
            query.SetMaxResults(topFilers.Value);
            return query;
        }
    }
}
