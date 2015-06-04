using System;
using TOne.LCR.Entities;
namespace TOne.LCR.Data
{
    public interface IZoneMatchDataManager : IDataManager, IRoutingDataManager
    {
        void UpdateAll();

        void CreateIndexesOnTable();

        void ApplyZoneMatchesToTempTable(SaleZoneMatches preparedZoneMatches);
    }
}
