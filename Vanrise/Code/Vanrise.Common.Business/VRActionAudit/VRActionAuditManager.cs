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
    public class VRActionAuditManager
    {
        static VRActionAuditLKUPManager s_lkupManager = new VRActionAuditLKUPManager();
        static IVRActionAuditDataManager s_dataManager = CommonDataManagerFactory.GetDataManager<IVRActionAuditDataManager>();

        public void AuditAction(string url, string module, string entity, string action, string objectId, string objectName, string actionDescription)
        {
            int? userId;
            ContextFactory.GetContext().TryGetLoggedInUserId(out userId);
            int urlId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.URL, url);
            int moduleId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.Module, module);
            int entityId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.Entity, entity);
            int actionId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.Action, action);
            s_dataManager.Insert(userId, urlId, moduleId, entityId, actionId, objectId, objectName, actionDescription);
        }
    }
}
