using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVREventHandlerDataManager:IDataManager
    {
        List<VREventHandler> GetVREventHandlers();
        bool AreVREventHandlerUpdated(ref object updateHandle);
        bool Insert(VREventHandler vREventHandler);
        bool Update(VREventHandler vREventHandler);
    
    }
}
