using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface ICDRDataManager : IDataManager
    {
        List<CDR> GetCDRData(DateTime fromDate, DateTime toDate, int nRecords, string CDROption);
    }
}
