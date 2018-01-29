using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRActionLogger : IVRActionLogger
    {
        #region Singleton

        static VRActionLogger s_current = new VRActionLogger();
        public static VRActionLogger Current
        {
            get
            {
                return s_current;
            }
        }

        #endregion

        #region Public Methods

        static VRActionAuditManager s_actionAuditManager = new VRActionAuditManager();
        static VRObjectTrackingManager s_objectTrackingManager = new VRObjectTrackingManager();

        public void LogGetFilteredAction(VRLoggableEntityBase loggableEntity, DataRetrievalInput dataRetrievalInput)
        {
            string action = dataRetrievalInput.DataRetrievalResultType == DataRetrievalResultType.Normal ? "Search" : "Export";
            LogAction(loggableEntity, action, null, null, null, null);
        }

        public void LogObjectCustomAction(VRLoggableEntityBase loggableEntity, string action, bool isObjectUpdated, Object obj, string actionDescription = null, Object technicalInformation = null)
        {
            if (isObjectUpdated)
            {
                TrackAndLogObjectAction(loggableEntity, action, obj, technicalInformation, actionDescription, false,null);
            }
            else
            {
                LogAction(loggableEntity, action, GetObjectId(loggableEntity, obj), GetObjectName(loggableEntity, obj), actionDescription, null);
            }
        }
        public void LogObjectViewed(VRLoggableEntityBase loggableEntity, Object obj)
        {
            LogAction(loggableEntity, "View Item Detail", GetObjectId(loggableEntity, obj), GetObjectName(loggableEntity, obj), null, null);
        }

        public void TrackAndLogObjectAdded(VRLoggableEntityBase loggableEntity, Object obj)
        {
            TrackAndLogObjectAction(loggableEntity, "Add", obj,null, null, true,null);
        }

        public void TrackAndLogObjectUpdated(VRLoggableEntityBase loggableEntity, Object obj)
        {
            TrackAndLogObjectAction(loggableEntity, "Update", obj,null, null, true,null);
        }

        public void TrackAndLogObjectUpdated(VRLoggableEntityBase loggableEntity, Object newObjectValue, Object oldObjectValue)
        {

            var changeInfoDefinition = loggableEntity.GetChangeInfoDefinition(new VRLoggableEntityGetChangeInfoDefinitionContext());
            changeInfoDefinition.ThrowIfNull("changeInfoDefinition");

            var vrActionAuditChangeInfoResolveChangeInfoContext = new VRActionAuditChangeInfoResolveChangeInfoContext
            {
                NewObjectValue = newObjectValue,
                OldObjectValue = oldObjectValue
            };
            var vrActionAuditChangeInfo = changeInfoDefinition.ResolveChangeInfo(vrActionAuditChangeInfoResolveChangeInfoContext);

            if (vrActionAuditChangeInfoResolveChangeInfoContext.NothingChanged)
                return;
              
            TrackAndLogObjectAction(loggableEntity, "Update", oldObjectValue, null, vrActionAuditChangeInfoResolveChangeInfoContext.ChangeSummary, true, vrActionAuditChangeInfo);
        
        }

        public void TrackAndLogObjectDeleted(VRLoggableEntityBase loggableEntity, Object obj)
        {
            TrackAndLogObjectAction(loggableEntity, "Delete", obj,null, null, true,null);
        }

        #endregion

        #region Private Methods

        private void TrackAndLogObjectAction(VRLoggableEntityBase loggableEntity, string action, Object obj, Object technicalInformation, string actionDescription, bool saveObject, VRActionAuditChangeInfo vrActionAuditChangeInfo)
        {
            action.ThrowIfNull("action"); 
            string objectId = GetObjectId(loggableEntity, obj);
            string objectName = GetObjectName(loggableEntity, obj);

            long objectTrackingId = s_objectTrackingManager.TrackObjectAction(loggableEntity, objectId, saveObject ? obj : null, action, actionDescription, technicalInformation, vrActionAuditChangeInfo);

            LogAction(loggableEntity, action, objectId, objectName, actionDescription, saveObject ? objectTrackingId : default(long?));
        }

        private void LogAction(VRLoggableEntityBase loggableEntity, string action, string objectId, string objectName, string actionDescription, long? objectTrackingId)
        {
            action.ThrowIfNull("action");
            string moduleName = loggableEntity.ModuleName;
            string entityName = loggableEntity.EntityDisplayName;
            moduleName.ThrowIfNull("moduleName", loggableEntity.EntityUniqueName);
            entityName.ThrowIfNull("entityName", loggableEntity.EntityUniqueName);
            string baseURL = null;
            if (HttpContext.Current != null)
            {
                var requestURI = HttpContext.Current.Request.Url;
                baseURL = requestURI.GetLeftPart(UriPartial.Authority);
            }
            s_actionAuditManager.AuditAction(baseURL, moduleName, loggableEntity.EntityDisplayName, action, objectId, objectName, actionDescription, objectTrackingId);
        }

        private string GetObjectId(VRLoggableEntityBase loggableEntity, Object obj)
        {
            Object objectIdAsObj = loggableEntity.GetObjectId(new VRLoggableEntityGetObjectIdContext { Object = obj });
            objectIdAsObj.ThrowIfNull("objectIdAsObj");
            string objectId = objectIdAsObj.ToString();
            return objectId;
        }

        private string GetObjectName(VRLoggableEntityBase loggableEntity, Object obj)
        {
            return loggableEntity.GetObjectName(new VRLoggableEntityGetObjectNameContext { Object = obj });
        }

        #endregion

        #region Private Classes

        public class VRLoggableEntityGetObjectIdContext : IVRLoggableEntityGetObjectIdContext
        {
            public Object Object { get; set; }
        }

        public class VRLoggableEntityGetObjectNameContext : IVRLoggableEntityGetObjectNameContext
        {
            public Object Object { get; set; }
        }

        #endregion
    }
}

