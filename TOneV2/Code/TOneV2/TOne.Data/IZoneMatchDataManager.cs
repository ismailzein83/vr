using System;
namespace TOne.Data
{
    public interface IZoneMatchDataManager
    {
        int UpdateAll(bool isFuture);
        //int UpdateByCodeDigit(bool isFuture, char firstDigit);
        void CreateTempTable(bool isFuture);

        void SwapTableWithTemp(bool isFuture);
        void CreateIndexesOnTable(bool isFuture);
    }
}
