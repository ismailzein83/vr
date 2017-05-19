using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IRegionDataManager : IDataManager
    {
        List<Region> GetRegions ();
        bool Update (Region Region);
        bool Insert (Region Region, out int insertedId);
        bool AreRegionsUpdated ( ref object updateHandle);
    }
}
