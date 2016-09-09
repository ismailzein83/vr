using System.Collections.Generic;
using Vanrise.Entities;


namespace Vanrise.Common.Data
{
    public interface IVRTimeZoneDataManager : IDataManager
    {
        List<VRTimeZone> GetVRTimeZones();
        bool Insert(VRTimeZone timeZone, out int insertedId);
        bool Update(VRTimeZone timeZone);
        bool AreVRTimeZonesUpdated(ref object updateHandle);
       
    }
}
