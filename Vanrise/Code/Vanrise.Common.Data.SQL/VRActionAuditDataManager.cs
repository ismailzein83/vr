using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class VRActionAuditDataManager : BaseSQLDataManager, IVRActionAuditDataManager
    {
        public VRActionAuditDataManager()
            : base(GetConnectionStringName("LoggingDBConnStringKey", "LogDBConnString"))
        {

        }

        public void Insert(int? userId, int urlId, int moduleId, int entityId, int actionId, string objectId, string actionDescription)
        {
            ExecuteNonQuerySP("[logging].[sp_ActionAudit_Insert]", userId, urlId, moduleId, entityId, actionId, objectId, actionDescription);
        }
    }
}
