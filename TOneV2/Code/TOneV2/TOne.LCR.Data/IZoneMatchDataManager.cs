using System;
namespace TOne.LCR.Data
{
    public interface IZoneMatchDataManager : IDataManager, IRoutingDataManager
    {
        void UpdateAll();

        void CreateIndexesOnTable();
    }
}
