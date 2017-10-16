using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class VRExclusiveSessionDataManager : BaseSQLDataManager, IVRExclusiveSessionDataManager
    {
        public VRExclusiveSessionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        public void InsertIfNotExists(Guid sessionTypeId, string targetId)
        {
            ExecuteNonQuerySP("[common].[sp_VRExclusiveSession_InsertIfNotExists]", sessionTypeId, targetId);
        }

        public void TryTakeSession(Guid sessionTypeId, string targetId, int userId, int timeoutInSeconds, out int takenByUserId)
        {
            var takenByUserIdObject = ExecuteScalarSP("[common].[sp_VRExclusiveSession_TryTake]", sessionTypeId, targetId, userId, timeoutInSeconds);
            takenByUserIdObject.ThrowIfNull("takenByUserIdObject");
            takenByUserId = (int)takenByUserIdObject;
        }

        public void ReleaseSession(Guid sessionTypeId, string targetId, int userId)
        {
            ExecuteNonQuerySP("[common].[sp_VRExclusiveSession_Release]", sessionTypeId, targetId, userId);
        }
    }
}
