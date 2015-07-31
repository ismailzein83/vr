using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using Vanrise.Entities;
namespace TOne.Analytics.Data
{
    public interface ICDRDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<BillingCDR> GetCDRData(Vanrise.Entities.DataRetrievalInput<CDRSummaryInput> input);
        //CDRBigResult GetCDRData(string tempTableKey, int nRecords);
    }
}
