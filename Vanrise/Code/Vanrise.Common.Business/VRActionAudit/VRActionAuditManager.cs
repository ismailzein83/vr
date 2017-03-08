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

        #region Public Methods
        public void AuditAction(string url, string module, string entity, string action, string objectId, string objectName, string actionDescription, long? objectTrackingId)
        {
            int? userId;
            ContextFactory.GetContext().TryGetLoggedInUserId(out userId);
            int? urlId = null;
            if (url != null)
                urlId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.URL, url);
            int moduleId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.Module, module);
            int entityId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.Entity, entity);
            int actionId = s_lkupManager.GetLKUPId(VRActionAuditLKUPType.Action, action);
            s_dataManager.Insert(userId, urlId, moduleId, entityId, actionId, objectId, objectName, objectTrackingId, actionDescription);
        }
        public Vanrise.Entities.IDataRetrievalResult<VRActionAuditDetail> GetFilteredActionAudits(Vanrise.Entities.DataRetrievalInput<VRActionAuditQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ActionAuditRequestHandler());

        }
        #endregion

        #region private Class
        private class ActionAuditRequestHandler : BigDataRequestHandler<VRActionAuditQuery, VRActionAudit, VRActionAuditDetail>
        {
            public override VRActionAuditDetail EntityDetailMapper(VRActionAudit entity)
            {
                VRActionAuditManager manager = new VRActionAuditManager();
                return manager.VRActionAuditDetailMapper(entity);
            }

            public override IEnumerable<VRActionAudit> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<VRActionAuditQuery> input)
            {
                IVRActionAuditDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRActionAuditDataManager>();
                return dataManager.GetAll(input.Query);
            }
        }
        #endregion

        #region private Method

        private VRActionAuditDetail VRActionAuditDetailMapper(VRActionAudit ActionAudit)
        {
            VRActionAuditLKUPManager VRLKUPM = new VRActionAuditLKUPManager();
            VRActionAuditDetail ActionAuditDetail = new VRActionAuditDetail();
            ActionAuditDetail.Entity = ActionAudit;
            if (ActionAudit.UserId.HasValue)
                ActionAuditDetail.UserName = BEManagerFactory.GetManager<IUserManager>().GetUserName(ActionAudit.UserId.Value);
            ActionAuditDetail.ModuleName = VRLKUPM.GetVRActionAuditLKUPName(ActionAudit.ModuleId);
            ActionAuditDetail.EntityName = VRLKUPM.GetVRActionAuditLKUPName(ActionAudit.EntityId);
            ActionAuditDetail.ActionName = VRLKUPM.GetVRActionAuditLKUPName(ActionAudit.ActionId);
            ActionAuditDetail.URLName = VRLKUPM.GetVRActionAuditLKUPName(ActionAudit.UrlId);

            return ActionAuditDetail;
        }
        #endregion
    }
}
