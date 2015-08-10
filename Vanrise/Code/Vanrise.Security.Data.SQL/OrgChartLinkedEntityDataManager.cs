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
            return (int?)ExecuteScalarSP("sec.sp_OrgChartLinkedEntity_GetLinkedOrgChartId", linkedEntityIdentifier);
        }

        public bool InsertOrUpdate(int orgChartId, string linkedEntityIdentifier)
        {
            int recordsEffected = ExecuteNonQuerySP("sec.sp_OrgChartLinkedEntity_Insert", orgChartId, linkedEntityIdentifier);
            return recordsEffected > 0;
        }
    }
}
