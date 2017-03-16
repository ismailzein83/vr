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
            Guid loggableEntityId = s_loggableEntityManager.GetLoggableEntityId(loggableEntity);
            int actionId = s_actionAuditLKUPManager.GetLKUPId(VRActionAuditLKUPType.Action, action);
            return s_dataManager.Insert(userId, loggableEntityId, objectId, obj, actionId, actionDescription);
        }

        public IDataRetrievalResult<VRObjectTrackingMetaDataDetail> GetFilteredObjectTracking(Vanrise.Entities.DataRetrievalInput<VRLoggableEntityQuery> input)
        {
           return BigDataManager.Instance.RetrieveData(input, new VRObjectTrackingHandler());
            
        }


        private class VRObjectTrackingHandler : BigDataRequestHandler<VRLoggableEntityQuery, VRObjectTrackingMetaData, VRObjectTrackingMetaDataDetail>
        {
            public override VRObjectTrackingMetaDataDetail EntityDetailMapper(VRObjectTrackingMetaData entity)
            {
                VRObjectTrackingManager manager = new VRObjectTrackingManager();
                return manager.VRObjectTrackingDetailMapper(entity);
            }

            public override IEnumerable<VRObjectTrackingMetaData> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<VRLoggableEntityQuery> input)
            {
                IVRObjectTrackingDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTrackingDataManager>();
                VRLoggableEntityManager manager = new VRLoggableEntityManager();
                Guid loggableEntityId = manager.GetLoggableEntityId(input.Query.EntityUniqueName);
                return dataManager.GetAll(loggableEntityId, input.Query.ObjectId);
            }
        }

        private VRObjectTrackingMetaDataDetail VRObjectTrackingDetailMapper(VRObjectTrackingMetaData objectTrackingMetaData)
        {
            VRActionAuditLKUPManager vrLKUPM = new VRActionAuditLKUPManager();
            VRObjectTrackingMetaDataDetail objectTrackingMetaDataDetail = new VRObjectTrackingMetaDataDetail();
            objectTrackingMetaDataDetail.Entity = objectTrackingMetaData;
            objectTrackingMetaDataDetail.UserName = BEManagerFactory.GetManager<IUserManager>().GetUserName(objectTrackingMetaData.UserId);
            objectTrackingMetaDataDetail.ActionName = vrLKUPM.GetVRActionAuditLKUPName(objectTrackingMetaData.ActionId);



            return objectTrackingMetaDataDetail;
        }
      
    }
}
