using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
namespace Vanrise.Common.Data
{
    public interface IVRObjectTrackingDataManager : IDataManager
    {
        long Insert(int userId, Guid loggableEntityId, string objectId, object obj, int actionId, string actionDescription, Object technicalInformation, VRActionAuditChangeInfo vrActionAuditChangeInfo);
        List<VRObjectTrackingMetaData> GetAll(Guid loggableEntityId, string objectId);
        object GetObjectDetailById(int VRObjectTrackingId);
        VRActionAuditChangeInfo GetVRActionAuditChangeInfoDetailById(int vrObjectTrackingId);

    }
}
