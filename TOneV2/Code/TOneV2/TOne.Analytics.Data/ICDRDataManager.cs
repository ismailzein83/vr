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
        CDRBigResult GetCDRData(string tempTableKey, CDRFilter filter, DateTime fromDate, DateTime toDate, int fromRow, int toRow, int nRecords, BillingCDROptionMeasures CDROption, BillingCDRMeasures orderBy, bool isDescending);
        CDRBigResult GetCDRData(string tempTableKey, int nRecords);
    }
}
