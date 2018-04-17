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
    public class VRTempPayloadManager
    {
        public VRTempPayload GetVRTempPayload(Guid vrTempPayloadId)
        {
            IVRTempPayloadDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRTempPayloadDataManager>();
            var vrTempPayload = dataManager.GetVRTempPayload(vrTempPayloadId);

            DeleteVRTempPayload(vrTempPayloadId);

            return vrTempPayload;
        }
        private bool DeleteVRTempPayload(Guid vrTempPayloadId)
        {
            IVRTempPayloadDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRTempPayloadDataManager>();
            return dataManager.DeleteVRTempPayload(vrTempPayloadId);
        }
        public Vanrise.Entities.InsertOperationOutput<Guid> AddVRTempPayload(VRTempPayload vrTempPayload)
        {
            Vanrise.Entities.InsertOperationOutput<Guid> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<Guid>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;

            IVRTempPayloadDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRTempPayloadDataManager>();

            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            vrTempPayload.CreatedBy = loggedInUserId;
            vrTempPayload.VRTempPayloadId = Guid.NewGuid();

            bool insertActionSucc = dataManager.Insert(vrTempPayload);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = vrTempPayload.VRTempPayloadId;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
    }
}
