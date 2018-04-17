using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRTempPayloadDataManager:IDataManager
    {
        bool Insert(VRTempPayload vrTempPayload);
        VRTempPayload GetVRTempPayload(Guid vrTempPayloadId);
        bool DeleteVRTempPayload(Guid vrTempPayloadId);
    }
}
