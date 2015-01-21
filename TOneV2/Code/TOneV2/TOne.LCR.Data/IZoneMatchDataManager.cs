using System;
namespace TOne.LCR.Data
{
    public interface IZoneMatchDataManager : IDataManager
    {
        int UpdateAll(bool isFuture);
        //int UpdateByCodeDigit(bool isFuture, char firstDigit);
        void CreateTempTable(bool isFuture);

        void SwapTableWithTemp(bool isFuture);
        void CreateIndexesOnTable(bool isFuture);
    }
}
