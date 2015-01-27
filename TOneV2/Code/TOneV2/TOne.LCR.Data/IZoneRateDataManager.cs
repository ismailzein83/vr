using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCR.Data
{
    public interface IZoneRateDataManager : IDataManager
    {
        void CreateAndFillTable(bool isFuture, DateTime effectiveOn);

        void SwapTableWithTemp(bool isFuture);
    }
}
