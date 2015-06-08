using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class CDRManager
    {
        private readonly ICDRDataManager _datamanager;
        public CDRManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<ICDRDataManager>();
        }
        public CDRBigResult GetCDRData(string tempTableKey, CDRFilter filter, DateTime fromDate, DateTime toDate, int fromRow, int toRow, int nRecords, string CDROption, BillingCDRMeasures orderBy, bool isDescending)
        {

            return _datamanager.GetCDRData( tempTableKey,  filter,  fromDate,  toDate,  fromRow,  toRow,  nRecords,  CDROption, orderBy,  isDescending);
        }
    }

}
