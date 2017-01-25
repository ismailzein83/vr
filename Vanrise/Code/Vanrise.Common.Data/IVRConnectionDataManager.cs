using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRConnectionDataManager : IDataManager
    {
        bool Update(VRConnection connection);
        
        bool Insert(VRConnection connection);
        
        List<VRConnection> GetVRConnections();

        bool AreVRConnectionsUpdated(ref object updateHandle);
    }
}