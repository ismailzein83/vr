using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Security.Data.SQL
{
    public class OrgChartLinkedEntityDataManager : BaseSQLDataManager, IOrgChartLinkedEntityDataManager
    {

        public OrgChartLinkedEntityDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        public int? GetLinkedOrgChartId(string linkedEntityIdentifier)
        {
            int? orgChartId = null;

            ExecuteReaderSP("sec.sp_OrgChartLinkedEntity_GetLinkedOrgChartId", (reader) =>
                {
                    while (reader.Read())
                    {
                        orgChartId = (int)reader["OrgChartID"];
                    }

                }, linkedEntityIdentifier);

            return orgChartId;
        }

        public void InsertOrUpdate(int orgChartId, string linkedEntityIdentifier)
        {
            ExecuteNonQuerySP("sec.sp_OrgChartLinkedEntity_Insert", orgChartId, linkedEntityIdentifier);
        }
    }
}
