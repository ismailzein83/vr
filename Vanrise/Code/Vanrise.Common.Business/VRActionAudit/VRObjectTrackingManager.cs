using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectTrackingManager
    {
        static IVRObjectTrackingDataManager s_dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTrackingDataManager>();
        static VRLoggableEntityManager s_loggableEntityManager = new VRLoggableEntityManager();
        static VRActionAuditLKUPManager s_actionAuditLKUPManager = new VRActionAuditLKUPManager();
        internal long TrackObjectAction(VRLoggableEntityBase loggableEntity, string objectId, Object obj, string action, string actionDescription)
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            int loggableEntityId = s_loggableEntityManager.GetLoggableEntityId(loggableEntity);
            int actionId = s_actionAuditLKUPManager.GetLKUPId(VRActionAuditLKUPType.Action, action);
            return s_dataManager.Insert(userId, loggableEntityId, objectId, obj, actionId, actionDescription);
        }
    }
}
